// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PipeAppMessageRouter.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Pipes.Routing
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO.Pipes;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Collections;
    using Kephas.Configuration;
    using Kephas.Logging;
    using Kephas.Messaging;
    using Kephas.Messaging.Distributed;
    using Kephas.Messaging.Distributed.Routing;
    using Kephas.Messaging.Pipes.Configuration;
    using Kephas.Messaging.Pipes.Endpoints;
    using Kephas.Serialization;
    using Kephas.Services;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// A message router over named pipes.
    /// </summary>
    [ProcessingPriority(Priority.Low + 1000)]
    [MessageRouter(ReceiverMatch = ChannelType + ":.*", IsFallback = true)]
    public class PipeAppMessageRouter : InProcessAppMessageRouter
    {
        private readonly IConfiguration<PipesSettings> pipesConfiguration;
        private readonly ISerializationService serializationService;
        private bool pipesInitialized;

        private string inputChannelName;
        private string? rootInputChannelName;
        private NamedPipeServerStream? input;
        private NamedPipeServerStream? rootInput;
        private NamedPipeClientStream rootOutput;

        private ConcurrentDictionary<string, NamedPipeClientStream> peerOutputs
            = new ConcurrentDictionary<string, NamedPipeClientStream>();

        private IList<string> peers = new List<string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="PipeAppMessageRouter"/> class.
        /// </summary>
        /// <param name="contextFactory">The context factory.</param>
        /// <param name="appRuntime">The application runtime.</param>
        /// <param name="messageProcessor">The message processor.</param>
        /// <param name="handlerRegistry">The message handler registry.</param>
        /// <param name="pipesConfiguration">The configuration for pipes.</param>
        /// <param name="serializationService">The serialization service.</param>
        public PipeAppMessageRouter(
            IContextFactory contextFactory,
            IAppRuntime appRuntime,
            IMessageProcessor messageProcessor,
            IMessageHandlerRegistry handlerRegistry,
            IConfiguration<PipesSettings> pipesConfiguration,
            ISerializationService serializationService)
            : base(contextFactory, appRuntime, messageProcessor)
        {
            this.pipesConfiguration = pipesConfiguration;
            this.serializationService = serializationService;
            if (this.AppRuntime.IsRoot())
            {
                handlerRegistry.RegisterHandler<RegisterPeerMessage>(this.HandleRegisterPeerAsync);
                handlerRegistry.RegisterHandler<RegisterPeerResponseMessage>(this.HandleRegisterPeerResponse);
            }
        }

        /// <summary>
        /// Actual initialization of the router.
        /// </summary>
        /// <param name="context">An optional context for initialization.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        protected override async Task InitializeCoreAsync(IContext context, CancellationToken cancellationToken)
        {
            await base.InitializeCoreAsync(context, cancellationToken).PreserveThreadContext();

            await this.InitializePipesAsync(cancellationToken).PreserveThreadContext();
        }

        /// <summary>
        /// Initializes the pipes.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The asynchronous result.</returns>
        protected virtual async Task InitializePipesAsync(CancellationToken cancellationToken)
        {
            var pipesNS = this.pipesConfiguration.Settings.Namespace;
            this.rootInputChannelName = string.IsNullOrEmpty(pipesNS) ? ChannelType : $"{pipesNS}_{ChannelType}";

            this.inputChannelName = this.GetChannelName(new Endpoint(appInstanceId: this.AppRuntime.GetAppInstanceId()!));
            this.input = this.OpenNamedPipeServerStream(inputChannelName);

            this.peers.Add(inputChannelName);

            if (this.AppRuntime.IsRoot())
            {
                this.rootInput = this.OpenNamedPipeServerStream(this.rootInputChannelName);
            }
            else
            {
                this.rootOutput = this.OpenNamedPipeClientStream(
                    this.pipesConfiguration.Settings.ServerName,
                    this.rootInputChannelName);

                var registerMessage = new RegisterPeerMessage
                {
                    AppInstanceId = this.AppRuntime.GetAppInstanceId(),
                    ServerName = this.pipesConfiguration.Settings.ServerName,
                    InputPipeName = this.inputChannelName,
                };
                
                
            }

            this.pipesInitialized = true;
        }

        /// <summary>
        /// Calculates the root channel name.
        /// </summary>
        /// <returns>
        /// The calculated root channel name.
        /// </returns>
        protected override string ComputeRootChannelName() => $"pipe-{base.ComputeRootChannelName()}";

        /// <summary>
        /// Routes the brokered message asynchronously, typically over the physical medium.
        /// </summary>
        /// <param name="brokeredMessage">The brokered message.</param>
        /// <param name="context">The dispatching context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// The asynchronous result yielding an action to take further and an optional reply.
        /// </returns>
        protected override async Task<(RoutingInstruction action, IMessage? reply)> RouteOutputAsync(
            IBrokeredMessage brokeredMessage,
            IDispatchingContext context,
            CancellationToken cancellationToken)
        {
            this.InitializationMonitor.AssertIsCompletedSuccessfully();

            if (!this.pipesInitialized)
            {
                return await base.RouteOutputAsync(brokeredMessage, context, cancellationToken).PreserveThreadContext();
            }

            var msgRecipients = brokeredMessage.Recipients ?? Enumerable.Empty<IEndpoint>();
            if (!msgRecipients.Any())
            {
                msgRecipients = new[] { new Endpoint() };
            }

            var groups = msgRecipients
                .SelectMany(r => this.GetChannelName(r).Split('\t').Select(c => (channelName: c, recipient: r)))
                .GroupBy(cr => cr.channelName)
                .Select(g => (channelName: g.Key, recipients: g.Select(gr => gr.recipient).Distinct().ToList()))
                .ToList();

            if (groups.Count == 1)
            {
                var serializedMessage = await this.serializationService
                    .SerializeAsync(brokeredMessage, ctx => ctx.IncludeTypeInfo(true), cancellationToken)
                    .PreserveThreadContext();
                await this.PublishAsync(serializedMessage, groups[0].channelName, brokeredMessage.IsOneWay)
                    .PreserveThreadContext();
            }
            else
            {
                foreach (var (channelName, recipients) in groups)
                {
                    var serializedMessage = await this.serializationService
                        .SerializeAsync(brokeredMessage.Clone(recipients), ctx => ctx.IncludeTypeInfo(true), cancellationToken)
                        .PreserveThreadContext();
                    await this.PublishAsync(serializedMessage, channelName, brokeredMessage.IsOneWay)
                        .PreserveThreadContext();
                }
            }

            return (RoutingInstruction.None, null);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the MessageRouterBase and optionally releases the
        /// managed resources.
        /// </summary>
        /// <param name="disposing">True to release both managed and unmanaged resources; false to
        ///                         release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            try
            {
                this.DisposePipes();
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        /// <summary>
        /// Opens the named pipe server stream and starts listening to it.
        /// </summary>
        /// <param name="pipeName">The pipe name.</param>
        /// <returns>The server stream.</returns>
        protected virtual NamedPipeServerStream OpenNamedPipeServerStream(string pipeName)
        {
            var stream = new NamedPipeServerStream(
                pipeName,
                PipeDirection.InOut,
                1,
                PipeTransmissionMode.Message,
                PipeOptions.Asynchronous);
            return stream;
        }

        /// <summary>
        /// Opens the named pipe client stream.
        /// </summary>
        /// <param name="serverName">The server name.</param>
        /// <param name="pipeName">The pipe name.</param>
        /// <returns>The server stream.</returns>
        protected virtual NamedPipeClientStream OpenNamedPipeClientStream(string serverName, string pipeName)
        {
            var stream = new NamedPipeClientStream(
                serverName,
                pipeName,
                PipeDirection.Out,
                PipeOptions.Asynchronous);
            return stream;
        }

        /// <summary>
        /// Disposes the pipes.
        /// </summary>
        protected virtual void DisposePipes()
        {
            if (!this.pipesInitialized)
            {
                return;
            }

            this.input?.Dispose();
            this.input = null;

            if (this.AppRuntime.IsRoot())
            {
                this.rootInput?.Dispose();
                this.rootInput = null;
            }

            var unregisterMessage = new UnregisterPeerMessage
            {
                AppInstanceId = this.AppRuntime.GetAppInstanceId(),
                InputPipeName = this.inputChannelName,
                ServerName = this.pipesConfiguration.Settings.ServerName,
            };
            var brokeredMessage = new BrokeredMessage(unregisterMessage)
            {
                Sender = new Endpoint(appInstanceId: this.AppRuntime.GetAppInstanceId()),
                IsOneWay = true,
            };
            var unregisterMessageString = this.serializationService.Serialize(brokeredMessage, ctx => ctx.IncludeTypeInfo(true));

            foreach (var peer in this.peers)
            {
                if (peer != this.inputChannelName)
                {
                    this.PublishAsync(unregisterMessageString, peer, true).WaitNonLocking();
                }
            }

            this.pipesInitialized = false;
        }

        /// <summary>
        /// Gets the channel name for the provided recipient.
        /// </summary>
        /// <param name="recipient">The recipient.</param>
        /// <returns>
        /// The channel name.
        /// </returns>
        protected override string GetChannelName(IEndpoint recipient)
        {
            if (!this.pipesInitialized)
            {
                return base.GetChannelName(recipient);
            }

            return string.IsNullOrEmpty(recipient.AppInstanceId)
                ? string.IsNullOrEmpty(recipient.AppId)
                    ? string.Join("\t", this.peers)
                    : string.Join("\t", this.peers.Where(p => p.StartsWith($"{this.rootInputChannelName}_{recipient.AppId}")))
                : $"{this.rootInputChannelName}_{recipient.AppInstanceId}";
        }

        private async Task PublishAsync(string serializedMessage, string channelName, bool oneWay)
        {
            var message = Encoding.UTF8.GetBytes(serializedMessage);
            var pipes = channelName.Split('\t');
            foreach (var pipe in pipes)
            {
                if (this.peerOutputs.TryGetValue(pipe, out var peerOutput))
                {
                    await peerOutput.WriteAsync(message, 0, message.Length).PreserveThreadContext();
                }
                else
                {
                    // TODO - peer not found.
                }
            }
        }

        private async Task<IMessage?> HandleRegisterPeerAsync(RegisterPeerMessage message, IMessagingContext context, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(message.InputPipeName))
            {
                this.Logger.Error($"Received a peer registration without an input pipe name for '{message.AppInstanceId}'");
                return null;
            }

            this.peers.Add(message.InputPipeName);
            this.peerOutputs.TryAdd(
                message.InputPipeName,
                this.OpenNamedPipeClientStream(message.ServerName, message.InputPipeName));

            this.Logger.Info($"Received a peer registration with input pipe '{message.ServerName}/{message.InputPipeName}' for '{message.AppInstanceId}'");

            var response = new RegisterPeerResponseMessage { Peers = this.peers.ToArray() };
            var brokeredResponse = new BrokeredMessage(response)
            {
                Recipients = new[] { new Endpoint(appInstanceId: message.AppInstanceId) },
                Sender = new Endpoint(appInstanceId: this.AppRuntime.GetAppInstanceId()),
                IsOneWay = true,
            };

            var responseString = await this.serializationService
                .SerializeAsync(brokeredResponse, ctx => ctx.IncludeTypeInfo(true), cancellationToken)
                .PreserveThreadContext();
            await this.PublishAsync(responseString, message.InputPipeName, true).PreserveThreadContext();

            return null;
        }

        private IMessage? HandleRegisterPeerResponse(RegisterPeerResponseMessage message, IMessagingContext context)
        {
            var newPeers = new HashSet<string>(message.Peers);
            newPeers.AddRange(this.peers);
            newPeers.ExceptWith(this.peers);
            this.peers.AddRange(newPeers);

            foreach (var newPeer in newPeers)
            {
                // TODO get the server name, too
                this.peerOutputs.TryAdd(
                    newPeer,
                    this.OpenNamedPipeClientStream(".", newPeer));
            }

            return null;
        }
    }
}