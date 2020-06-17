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
    using System.IO;
    using System.IO.Pipes;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Configuration;
    using Kephas.Interaction;
    using Kephas.IO;
    using Kephas.Logging;
    using Kephas.Messaging;
    using Kephas.Messaging.Distributed;
    using Kephas.Messaging.Distributed.Routing;
    using Kephas.Messaging.Pipes.Configuration;
    using Kephas.Messaging.Pipes.Endpoints;
    using Kephas.Model.AttributedModel;
    using Kephas.Orchestration;
    using Kephas.Orchestration.Interaction;
    using Kephas.Serialization;
    using Kephas.Services;
    using Kephas.Threading;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// A message router over named pipes.
    /// </summary>
    [Override]
    [ProcessingPriority(Priority.Low + 1000)]
    [MessageRouter(ReceiverMatch = ChannelType + ":.*", IsFallback = true)]
    public class PipesAppMessageRouter : InProcessAppMessageRouter
    {
        private readonly IConfiguration<PipesSettings> pipesConfiguration;
        private readonly ISerializationService serializationService;
        private readonly IEventHub eventHub;
        private readonly Lazy<IOrchestrationManager> lazyOrchestrationManager;
        private bool pipesInitialized;

        private CancellationTokenSource? disposeSource;

        private ConcurrentDictionary<string, ClientChannel> outChannels
            = new ConcurrentDictionary<string, ClientChannel>();

        private ConcurrentDictionary<string, ServerChannel> inChannels
            = new ConcurrentDictionary<string, ServerChannel>();

        private Lock? outWriteLock;
        private IEventSubscription? appStartingSubscription;
        private IEventSubscription? appStoppedSubscription;
        private IEventSubscription? appStartedSubscription;

        /// <summary>
        /// Initializes a new instance of the <see cref="PipesAppMessageRouter"/> class.
        /// </summary>
        /// <param name="contextFactory">The context factory.</param>
        /// <param name="appRuntime">The application runtime.</param>
        /// <param name="messageProcessor">The message processor.</param>
        /// <param name="pipesConfiguration">The configuration for pipes.</param>
        /// <param name="serializationService">The serialization service.</param>
        /// <param name="eventHub">The event hub.</param>
        /// <param name="lazyOrchestrationManager">The lazy orchestration manager.</param>
        public PipesAppMessageRouter(
            IContextFactory contextFactory,
            IAppRuntime appRuntime,
            IMessageProcessor messageProcessor,
            IConfiguration<PipesSettings> pipesConfiguration,
            ISerializationService serializationService,
            IEventHub eventHub,
            Lazy<IOrchestrationManager> lazyOrchestrationManager)
            : base(contextFactory, appRuntime, messageProcessor)
        {
            this.pipesConfiguration = pipesConfiguration;
            this.serializationService = serializationService;
            this.eventHub = eventHub;
            this.lazyOrchestrationManager = lazyOrchestrationManager;
        }

        /// <summary>
        /// Actual initialization of the router.
        /// </summary>
        /// <param name="context">An optional context for initialization.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        protected override async Task InitializeCoreAsync(IContext? context, CancellationToken cancellationToken)
        {
            await base.InitializeCoreAsync(context, cancellationToken).PreserveThreadContext();

            this.appStartedSubscription = this.eventHub.Subscribe<AppStartedEvent>(
                async (e, ctx, token) =>
                {
                    if (e.AppInfo.AppInstanceId != this.AppRuntime.GetAppInstanceId())
                    {
                        return;
                    }

                    this.appStartingSubscription = this.eventHub.Subscribe<AppStartingEvent>(this.HandleAppStartingEvent);
                    this.appStoppedSubscription = this.eventHub.Subscribe<AppStoppedEvent>(this.HandleAppStoppedEvent);

                    await this.InitializePipesAsync(cancellationToken).PreserveThreadContext();

                    this.appStartedSubscription?.Dispose();
                    this.appStartedSubscription = null;
                });
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

            // make sure we have a channel to self.
            await this.EnsureOutChannelAsync(this.AppRuntime.GetAppInstanceId()!, cancellationToken).PreserveThreadContext();

            if (!this.AppRuntime.IsRoot())
            {
                // first create an input channel from root.
                // the root will create the OUT after successful client connect.
                this.EnsureInChannel(this.lazyOrchestrationManager.Value.GetRootAppInstanceId());

                // then create an output channel to root.
                // the root created the IN when starting the child process.
                var (rootOutChannel, _) = await this.EnsureOutChannelAsync(this.lazyOrchestrationManager.Value.GetRootAppInstanceId(), cancellationToken).PreserveThreadContext();

                // send a join message
                await this.PublishAsync(new JoinPeerMessage(), rootOutChannel, cancellationToken).PreserveThreadContext();
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
                return await this.PublishAsync(brokeredMessage, context, groups[0].appInstanceId, cancellationToken)
                    .PreserveThreadContext();
            }

            foreach (var (appInstanceId, recipients) in groups)
            {
                await this.PublishAsync(brokeredMessage.Clone(recipients), context, appInstanceId, cancellationToken)
                    .PreserveThreadContext();
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
                this.appStoppedSubscription?.Dispose();
                this.appStoppedSubscription = null;
                this.appStartingSubscription?.Dispose();
                this.appStartingSubscription = null;

                base.Dispose(disposing);
            }
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

            // dispose the server input channels
            foreach (var inChannel in this.inChannels.Values)
            {
                inChannel.Dispose();
                this.Logger.Debug("Pipe {channel} disposed.", inChannel.ChannelName);
            }

            this.inChannels.Clear();

            // dispose the peer output channels
            foreach (var outChannel in this.outChannels.Values)
            {
                outChannel.Dispose();
                this.Logger.Debug("Pipe {server}/{channel} disposed.", outChannel.ServerName, outChannel.ChannelName);
            }

            this.outChannels.Clear();

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
                    ? string.Join("\t", this.outChannels.Keys)
                    : string.Join("\t", this.outChannels.Keys.Where(p => p.StartsWith(recipient.AppId, StringComparison.OrdinalIgnoreCase)))
                : recipient.AppInstanceId;
        }

        private async Task<(RoutingInstruction action, IMessage? reply)> PublishAsync(
            IBrokeredMessage message,
            IDispatchingContext context,
            string appInstanceId,
            CancellationToken cancellationToken)
        {
            if (this.outChannels.TryGetValue(appInstanceId, out var clientChannel))
            {
                if (clientChannel.IsSelf)
                {
                    return await base.RouteOutputAsync(message, context, cancellationToken).PreserveThreadContext();
                }

                var serializedMessage = await this.serializationService
                    .SerializeAsync(message, ctx => ctx.IncludeTypeInfo(true), cancellationToken)
                    .PreserveThreadContext();
                await clientChannel.WriteAsync(serializedMessage!, cancellationToken).PreserveThreadContext();
            }
            else
            {
                this.Logger.Warn("Peer '{peer}' not registered.", appInstanceId);
            }

            return (RoutingInstruction.None, null);
        }

        private async Task PublishAsync(object message, ClientChannel clientChannel, CancellationToken cancellationToken)
        {
            var messageString = await this.serializationService
                .SerializeAsync(message, ctx => ctx.IncludeTypeInfo(true), cancellationToken)
                .PreserveThreadContext();
            await clientChannel.WriteAsync(messageString!, cancellationToken).PreserveThreadContext();
        }

        private async Task OnMessageReceivedAsync(ServerChannel channel, string messageString, CancellationToken cancellationToken)
        {
            var message = await this.serializationService
                .DeserializeAsync(messageString, cancellationToken: cancellationToken)
                .PreserveThreadContext();

            switch (message)
            {
                case JoinPeerMessage joinPeerMessage:
                    await this.HandleJoinPeerMessageAsync(channel.AppInstanceId, joinPeerMessage, cancellationToken)
                        .PreserveThreadContext();
                    break;
                case PeersChangedMessage peersChangedMessage:
                    await this.HandlePeersChangedMessageAsync(channel.AppInstanceId, peersChangedMessage, cancellationToken)
                        .PreserveThreadContext();
                    break;
                case HandshakeMessage handshakeMessage:
                    await this.HandleHandshakeMessageAsync(channel.AppInstanceId, handshakeMessage, cancellationToken)
                        .PreserveThreadContext();
                    break;
                case IBrokeredMessage brokeredMessage:
                    await this.HandleBrokeredMessageAsync(channel.AppInstanceId, brokeredMessage, cancellationToken)
                        .PreserveThreadContext();
                    break;
                default:
                    this.Logger.Warn("Unsupported message '{message}'.", messageString);
                    break;
            }
        }

        private async Task OnConnectionEstablishedAsync(ServerChannel channel, CancellationToken cancellationToken)
        {
            var (peerChannel, existing) =
                await this.EnsureOutChannelAsync(channel.AppInstanceId, cancellationToken).PreserveThreadContext();

            if (!existing)
            {
                this.Logger.Debug("Created output channel {channel} (server mode).", channel.ChannelName);
            }
        }

        private async Task HandleHandshakeMessageAsync(
            string peerAppInstanceId,
            HandshakeMessage handshakeMessage,
            CancellationToken cancellationToken)
        {
            this.Logger.Info("Received handshake notification from {peer}. Source: {sourcePeer}, target: {targetPeer}.", peerAppInstanceId, handshakeMessage.Source, handshakeMessage.Target);

            if (string.IsNullOrEmpty(handshakeMessage.Source))
            {
                // in this case, this app is only the middle man.
                handshakeMessage.Source = peerAppInstanceId;
                if (!this.outChannels.TryGetValue(handshakeMessage.Target, out var targetChannel))
                {
                    this.Logger.Warn("Disconnected from {peer} while trying to redirect handshake request from {source}.", handshakeMessage.Target, peerAppInstanceId);
                    return;
                }

                try
                {
                    await this.PublishAsync(handshakeMessage, targetChannel, cancellationToken).PreserveThreadContext();
                }
                catch (Exception ex)
                {
                    this.Logger.Warn("Error while trying to redirect handshake request from {source} to {peer}.", peerAppInstanceId, handshakeMessage.Target);
                }

                return;
            }

            if (handshakeMessage.Source.Equals(this.AppRuntime.GetAppInstanceId()))
            {
                // in this case, this app originally sent the handshake request.
                // it is now time to complete the circle.
                await this.EnsureOutChannelAsync(handshakeMessage.Target, cancellationToken).PreserveThreadContext();
                return;
            }

            var (serverChannel, existing) = this.EnsureInChannel(handshakeMessage.Source);
            if (existing)
            {
                this.Logger.Info("Channel to {peer} already open.", handshakeMessage.Source);
                return;
            }

            await this.EnsureOutChannelAsync(handshakeMessage.Source, cancellationToken).PreserveThreadContext();
        }

        private async Task HandlePeersChangedMessageAsync(
            string peerAppInstanceId,
            PeersChangedMessage peersChangedMessage,
            CancellationToken cancellationToken)
        {
            this.Logger.Info("Received peers changed notification from {peer}. Peers: {peers}.", peerAppInstanceId, peersChangedMessage.Apps);

            if (peersChangedMessage.Apps == null)
            {
                return;
            }

            foreach (var appInfo in peersChangedMessage.Apps)
            {
                var (serverChannel, existing) = this.EnsureInChannel(appInfo.AppInstanceId);
                if (existing)
                {
                    continue;
                }

                if (!this.outChannels.TryGetValue(peerAppInstanceId, out var senderChannel))
                {
                    this.Logger.Warn("Disconnected from {peer} when trying to request handshake.", peerAppInstanceId);
                    continue;
                }

                try
                {
                    await this.PublishAsync(
                        new HandshakeMessage { Target = appInfo.AppInstanceId },
                        senderChannel,
                        cancellationToken).PreserveThreadContext();
                }
                catch (Exception ex)
                {
                    this.Logger.Warn("Error while sending handshake request to {peer}.", peerAppInstanceId);
                }
            }
        }

        private async Task HandleBrokeredMessageAsync(
            string peerAppInstanceId,
            IBrokeredMessage brokeredMessage,
            CancellationToken cancellationToken)
        {
            try
            {
                this.Logger.Trace("Received brokered message from {peer}: '{message}'.", peerAppInstanceId, brokeredMessage);

                await this.RouteInputAsync(brokeredMessage, this.AppContext, cancellationToken)
                    .PreserveThreadContext();
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex, "Error while routing from peer {peer}: '{message}'.", peerAppInstanceId, brokeredMessage);
            }
        }

        private async Task HandleJoinPeerMessageAsync(
            string peerAppInstanceId,
            JoinPeerMessage joinPeerMessage,
            CancellationToken cancellationToken)
        {
            this.Logger.Info("Peer {peer} sent a join request.", peerAppInstanceId);

            var (peerChannel, existing) = await this.EnsureOutChannelAsync(peerAppInstanceId, cancellationToken).PreserveThreadContext();
            try
            {
                await this.PublishAsync(
                    new PeersChangedMessage
                    {
                        Apps = (await this.lazyOrchestrationManager.Value
                                .GetLiveAppsAsync(cancellationToken: cancellationToken)
                                .PreserveThreadContext())
                            .ToArray(),
                    },
                    peerChannel,
                    cancellationToken).PreserveThreadContext();
            }
            catch (Exception ex)
            {
                this.Logger.Error(
                    ex,
                    "Error responding to initialize peer message from '{peer}'.",
                    peerAppInstanceId);
            }
        }

        private void HandleAppStoppedEvent(AppStoppedEvent e, IContext context)
        {
            var peerAppInstanceId = e.AppInfo.AppInstanceId;
            if (e.AppInfo.AppId == this.AppRuntime.GetAppId() &&
                peerAppInstanceId == this.AppRuntime.GetAppInstanceId())
            {
                return;
            }

            if (this.inChannels.TryRemove(peerAppInstanceId, out var serverChannel))
            {
                serverChannel.Stream?.Dispose();
            }
            else
            {
                this.Logger.Warn("Server channel not found for {peer}.", peerAppInstanceId);
            }

            if (this.outChannels.TryRemove(peerAppInstanceId, out var peerOutChannel))
            {
                peerOutChannel.Stream?.Dispose();
            }
            else
            {
                this.Logger.Warn("Peer input channel not found for {peer}.", peerAppInstanceId);
            }
        }

        private async Task<(ClientChannel channel, bool existing)> EnsureOutChannelAsync(string peerAppInstanceId, CancellationToken cancellationToken)
        {
            if (this.outChannels.TryGetValue(peerAppInstanceId, out var clientChannel))
            {
                return (clientChannel, true);
            }

            var channelName = this.GetOutChannelName(peerAppInstanceId);
            clientChannel = peerAppInstanceId == this.AppRuntime.GetAppInstanceId()
                ? new SelfClientChannel(peerAppInstanceId, ".", channelName, this.pipesConfiguration, this.Logger)
                : new ClientChannel(peerAppInstanceId, ".", channelName, this.pipesConfiguration, this.Logger);
            await clientChannel.OpenAsync(cancellationToken).PreserveThreadContext();
            this.outChannels.TryAdd(
                peerAppInstanceId,
                clientChannel);
            return (clientChannel, false);
        }

        private void HandleAppStartingEvent(AppStartingEvent e, IContext context)
        {
            if (e.AppInfo.AppId == this.AppRuntime.GetAppId() &&
                e.AppInfo.AppInstanceId == this.AppRuntime.GetAppInstanceId())
            {
                return;
            }

            this.EnsureInChannel(e.AppInfo.AppInstanceId);
        }

        private (ServerChannel channel, bool existing) EnsureInChannel(string peerAppInstanceId)
        {
            if (this.inChannels.TryGetValue(peerAppInstanceId, out var serverChannel))
            {
                return (serverChannel, true);
            }

            var channelName = this.GetInChannelName(peerAppInstanceId);
            serverChannel = new ServerChannel(peerAppInstanceId, channelName, this.Logger, this.OnConnectionEstablishedAsync, this.OnMessageReceivedAsync, this.disposeSource!.Token);
            serverChannel.Open();
            this.inChannels.TryAdd(
                peerAppInstanceId,
                serverChannel);
            return (serverChannel, false);
        }

        private string GetOutChannelName(string to)
        {
            return this.GetChannelName(this.AppRuntime.GetAppInstanceId()!, to);
        }

        private string GetInChannelName(string from)
        {
            return this.GetChannelName(from, this.AppRuntime.GetAppInstanceId()!);
        }

        private string GetChannelName(string from, string to)
        {
            var pipesNS = this.pipesConfiguration.Settings.Namespace;
            var prefix = string.IsNullOrEmpty(pipesNS) ? ChannelType : $"{pipesNS}_{ChannelType}";

            var channelName = $"{prefix}.{to}.{from}";
            return channelName;
        }
    }
}