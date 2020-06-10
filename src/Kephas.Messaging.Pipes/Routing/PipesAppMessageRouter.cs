// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PipesAppMessageRouter.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Pipes.Routing
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Pipes;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Configuration;
    using Kephas.IO;
    using Kephas.Logging;
    using Kephas.Messaging;
    using Kephas.Messaging.Distributed;
    using Kephas.Messaging.Distributed.Routing;
    using Kephas.Messaging.Pipes.Configuration;
    using Kephas.Messaging.Pipes.Endpoints;
    using Kephas.Serialization;
    using Kephas.Services;
    using Kephas.Threading;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// A message router over named pipes.
    /// </summary>
    [ProcessingPriority(Priority.Low + 1000)]
    [MessageRouter(ReceiverMatch = ChannelType + ":.*", IsFallback = true)]
    public class PipesAppMessageRouter : InProcessAppMessageRouter
    {
        private readonly IConfiguration<PipesSettings> pipesConfiguration;
        private readonly ISerializationService serializationService;
        private bool pipesInitialized;

        private string? rootInChannelName;

        private string inChannelName;
        private NamedPipeServerStream? inChannel;
        private Task? inListenTask;

        private CancellationTokenSource? disposeSource;

        private ConcurrentDictionary<string, PeerChannel> peerOutChannels
            = new ConcurrentDictionary<string, PeerChannel>();

        private Lock? outWriteLock;

        /// <summary>
        /// Initializes a new instance of the <see cref="PipesAppMessageRouter"/> class.
        /// </summary>
        /// <param name="contextFactory">The context factory.</param>
        /// <param name="appRuntime">The application runtime.</param>
        /// <param name="messageProcessor">The message processor.</param>
        /// <param name="pipesConfiguration">The configuration for pipes.</param>
        /// <param name="serializationService">The serialization service.</param>
        public PipesAppMessageRouter(
            IContextFactory contextFactory,
            IAppRuntime appRuntime,
            IMessageProcessor messageProcessor,
            IConfiguration<PipesSettings> pipesConfiguration,
            ISerializationService serializationService)
            : base(contextFactory, appRuntime, messageProcessor)
        {
            this.pipesConfiguration = pipesConfiguration;
            this.serializationService = serializationService;
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
            this.outWriteLock = new Lock();
            this.disposeSource = new CancellationTokenSource();

            var pipesNS = this.pipesConfiguration.Settings.Namespace;
            this.rootInChannelName = string.IsNullOrEmpty(pipesNS) ? ChannelType : $"{pipesNS}_{ChannelType}";

            this.inChannelName = this.AppRuntime.IsRoot()
                ? this.rootInChannelName
                : $"{this.rootInChannelName}_{this.AppRuntime.GetAppInstanceId()}";
            this.inChannel = this.OpenNamedPipeServerStream(this.inChannelName);
            this.inListenTask = this.StartListeningAsync(this.inChannel, this.disposeSource.Token);

            var selfPeerChannel = new PeerChannel(
                this.AppRuntime.GetAppInstanceId(),
                ".",
                this.inChannelName,
                this.OpenNamedPipeClientStream(".", this.inChannelName));
            this.peerOutChannels.TryAdd(selfPeerChannel.AppInstanceId, selfPeerChannel);

            if (!this.AppRuntime.IsRoot())
            {
                // create a channel to root only for the initial RegisterPeer invocation.
                // afterwards, the root will answer with a list of peers.
                using var rootOutChannel = this.OpenNamedPipeClientStream(
                    this.pipesConfiguration.Settings.ServerName,
                    this.rootInChannelName);

                var registerMessage = new RegisterPeerMessage
                {
                    AppInstanceId = this.AppRuntime.GetAppInstanceId(),
                    ServerName = this.pipesConfiguration.Settings.ServerName,
                    ChannelName = this.inChannelName,
                };

                await this.PublishAsync(registerMessage, rootOutChannel).PreserveThreadContext();
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
                .SelectMany(r => this.GetChannelName(r).Split('\t').Select(c => (appInstanceId: c, recipient: r)))
                .GroupBy(cr => cr.appInstanceId)
                .Select(g => (appInstanceId: g.Key, recipients: g.Select(gr => gr.recipient).Distinct().ToList()))
                .ToList();

            if (groups.Count == 1)
            {
                await this.PublishAsync(brokeredMessage, new[] { groups[0].appInstanceId })
                    .PreserveThreadContext();
            }
            else
            {
                foreach (var (appInstanceId, recipients) in groups)
                {
                    await this.PublishAsync(brokeredMessage.Clone(recipients), new[] { appInstanceId })
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
        /// <param name="channelName">The channel name.</param>
        /// <returns>The server stream.</returns>
        protected virtual NamedPipeServerStream OpenNamedPipeServerStream(string channelName)
        {
            var stream = new NamedPipeServerStream(
                channelName,
                PipeDirection.InOut,
                10,
                PipeTransmissionMode.Message,
                PipeOptions.Asynchronous);

            return stream;
        }

        /// <summary>
        /// Opens the named pipe client stream.
        /// </summary>
        /// <param name="serverName">The server name.</param>
        /// <param name="channelName">The channel name.</param>
        /// <returns>The server stream.</returns>
        protected virtual NamedPipeClientStream OpenNamedPipeClientStream(string? serverName, string channelName)
        {
            serverName ??= ".";
            var stream = new NamedPipeClientStream(
                serverName,
                channelName,
                PipeDirection.InOut,
                PipeOptions.Asynchronous);

            stream.Connect();
            stream.ReadMode = PipeTransmissionMode.Message;
            return stream;
        }

        /// <summary>
        /// Disposes the pipes and the whole infrastructure.
        /// </summary>
        protected virtual void DisposePipes()
        {
            if (!this.pipesInitialized)
            {
                return;
            }

            // cancel the dispose source to start cancelling the reading.
            this.disposeSource?.Cancel();
            this.disposeSource = null;

            // dispose the input channel and listening task
            // this.inListenTask?.Dispose();
            this.inChannel?.Dispose();
            this.inChannel = null;

            // notify peers that it is closing
            var unregisterMessage = new UnregisterPeerMessage
            {
                AppInstanceId = this.AppRuntime.GetAppInstanceId(),
                ChannelName = this.inChannelName,
                ServerName = this.pipesConfiguration.Settings.ServerName,
            };
            this.PublishAsync(
                    unregisterMessage,
                    this.peerOutChannels.Values
                        .Where(c => c.ChannelName != this.inChannelName)
                        .Select(c => c.AppInstanceId)
                        .ToArray())
                .WaitNonLocking();

            // dispose the peer output channels
            foreach (var peerOutChannel in this.peerOutChannels.Values)
            {
                peerOutChannel.Stream.Dispose();
            }

            this.peerOutChannels.Clear();

            // dispose the write lock
            this.outWriteLock?.Dispose();
            this.outWriteLock = null;

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
                    ? string.Join("\t", this.peerOutChannels.Values.Select(c => c.AppInstanceId))
                    : string.Join("\t", this.peerOutChannels.Values.Where(p => p.AppInstanceId.StartsWith(recipient.AppId)).Select(c => c.AppInstanceId))
                : recipient.AppInstanceId;
        }

        private async Task PublishAsync(object message, IEnumerable<string> appInstanceIds)
        {
            var serializedMessage = await this.serializationService
                .SerializeAsync(message, ctx => ctx.IncludeTypeInfo(true))
                .PreserveThreadContext();
            var messageBytes = Encoding.UTF8.GetBytes(serializedMessage);
            foreach (var appInstanceId in appInstanceIds)
            {
                if (this.peerOutChannels.TryGetValue(appInstanceId, out var peerChannel))
                {
                    await this.PublishAsync(messageBytes, peerChannel.Stream).PreserveThreadContext();
                }
                else
                {
                    this.Logger.Warn("Peer '{peer}' not registered.", appInstanceId);
                }
            }
        }

        private async Task PublishAsync(object message, Stream peerOutput)
        {
            var messageString = await this.serializationService
                .SerializeAsync(message, ctx => ctx.IncludeTypeInfo(true))
                .PreserveThreadContext();
            var messageBytes = Encoding.UTF8.GetBytes(messageString);
            await this.PublishAsync(messageBytes, peerOutput).PreserveThreadContext();
        }

        private async Task PublishAsync(byte[] messageBytes, Stream peerOutput)
        {
            await this.outWriteLock!.EnterAsync(async () =>
            {
                await peerOutput.WriteAsync(messageBytes, 0, messageBytes.Length).PreserveThreadContext();
                await peerOutput.FlushAsync().PreserveThreadContext();
            }).PreserveThreadContext();
        }

        private async Task HandleRegisterPeerAsync(RegisterPeerMessage message, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(message.ChannelName))
            {
                this.Logger.Error("Received a peer registration without an input pipe name for '{peer}'", message.AppInstanceId);
                return;
            }

            if (this.peerOutChannels.Values.Any(c => c.AppInstanceId == message.AppInstanceId))
            {
                this.Logger.Warn("Peer '{peer}' already registered with channel '{channel}'", message.AppInstanceId, $"{message.ServerName}/{message.ChannelName}");
                return;
            }

            var peerChannel = new PeerChannel(
                message.AppInstanceId,
                message.ServerName,
                message.ChannelName,
                this.OpenNamedPipeClientStream(message.ServerName, message.ChannelName!));
            this.peerOutChannels.TryAdd(peerChannel.AppInstanceId, peerChannel);

            this.Logger.Info("Received a peer registration with input pipe '{channel}' for '{peer}'", $"{message.ServerName}/{message.ChannelName}", message.AppInstanceId);

            if (this.AppRuntime.IsRoot())
            {
                var response = new RegisterPeerResponseMessage
                {
                    Peers = this.peerOutChannels.Values
                        .Select(c => new RegisterPeerMessage
                            {
                                AppInstanceId = c.AppInstanceId,
                                ServerName = c.ServerName,
                                ChannelName = c.ChannelName,
                            }).ToArray(),
                };

                await this.PublishAsync(response, peerChannel.Stream).PreserveThreadContext();
            }

            return;
        }

        private async Task HandleRegisterPeerResponseAsync(RegisterPeerResponseMessage message, CancellationToken cancellationToken)
        {
            var newPeers = new HashSet<string>(message.Peers.Select(p => p.AppInstanceId));
            newPeers.ExceptWith(this.peerOutChannels.Keys);

            foreach (var registerMessage in message.Peers.Where(p => newPeers.Contains(p.AppInstanceId)))
            {
                await this.HandleRegisterPeerAsync(registerMessage, cancellationToken).PreserveThreadContext();
            }
        }

        private void HandleUnregisterPeerMessage(UnregisterPeerMessage message)
        {
            if (this.peerOutChannels.TryRemove(message.AppInstanceId, out var channel))
            {
                channel.Stream.Dispose();
            }
        }

        private async Task StartListeningAsync(NamedPipeServerStream channel, CancellationToken cancellationToken)
        {
            await Task.Yield();
            await channel.WaitForConnectionAsync(cancellationToken).PreserveThreadContext();

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var messageString = await this.ReadMessageAsync(channel, cancellationToken).PreserveThreadContext();
                    var message = await this.serializationService
                        .DeserializeAsync(messageString, cancellationToken: cancellationToken)
                        .PreserveThreadContext();

                    switch (message)
                    {
                        case RegisterPeerResponseMessage registerResponse:
                            await this.HandleRegisterPeerResponseAsync(registerResponse, cancellationToken)
                                .PreserveThreadContext();
                            break;
                        case RegisterPeerMessage registerMessage:
                            await this.HandleRegisterPeerAsync(registerMessage, cancellationToken)
                                .PreserveThreadContext();
                            break;
                        case UnregisterPeerMessage unregisterMessage:
                            this.HandleUnregisterPeerMessage(unregisterMessage);
                            break;
                        case IBrokeredMessage brokeredMessage:
                            this.RouteInputAsync(brokeredMessage, this.AppContext, cancellationToken)
                                .ContinueWith(
                                    t => this.Logger.Error(t.Exception, "Error while routing from input '{message}'.", brokeredMessage),
                                    TaskContinuationOptions.OnlyOnFaulted);
                            break;
                        default:
                            this.Logger.Warn("Unsupported message '{message}'.", messageString);
                            break;
                    }
                }
                catch (OperationCanceledException)
                    when (cancellationToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception ex)
                {
                    this.Logger.Error(ex, "Error while processing the request from client.");
                }
            }
        }

        private async Task<string> ReadMessageAsync(NamedPipeServerStream channel, CancellationToken cancellationToken)
        {
            var buffer = new byte[256];
            using var memStream = new MemoryStream(1000);
            do
            {
                var numBytes = await channel.ReadAsync(buffer, 0, buffer.Length, cancellationToken).PreserveThreadContext();
                memStream.Write(buffer, 0, numBytes);
            }
            while (!channel.IsMessageComplete);

            memStream.Position = 0;
            var bytes = memStream.ReadAllBytes();

            return Encoding.UTF8.GetString(bytes);
        }

        private struct PeerChannel
        {
            public PeerChannel(string appInstanceId, string serverName, string channelName, NamedPipeClientStream stream)
            {
                this.AppInstanceId = appInstanceId;
                this.ServerName = serverName;
                this.ChannelName = channelName;
                this.Stream = stream;
            }

            public string AppInstanceId { get; set; }

            public string ServerName { get; set; }

            public string ChannelName { get; set; }

            public NamedPipeClientStream Stream { get; set; }
        }
    }
}