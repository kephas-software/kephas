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
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Configuration;
    using Kephas.Interaction;
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

        private readonly ConcurrentDictionary<string, ClientChannel> outChannels
            = new ConcurrentDictionary<string, ClientChannel>();

        private CancellationTokenSource? disposeSource;
        private ServerChannel? inChannel;
        private bool channelsInitialized;

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

                    this.appStoppedSubscription = this.eventHub.Subscribe<AppStoppedEvent>(this.HandleAppStoppedEvent);

                    await this.InitializeChannelsAsync(cancellationToken).PreserveThreadContext();

                    this.appStartedSubscription?.Dispose();
                    this.appStartedSubscription = null;
                });
        }

        /// <summary>
        /// Initializes the channels for reading and writing.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The asynchronous result.</returns>
        protected virtual async Task InitializeChannelsAsync(CancellationToken cancellationToken)
        {
            this.disposeSource = new CancellationTokenSource();
            var thisAppInstanceId = this.AppRuntime.GetAppInstanceId()!;

            // first create an input channel for connecting clients, including the root.
            this.EnsureInChannel(thisAppInstanceId);

            // make sure we have an out dummy channel to self.
            this.EnsureOutChannel(thisAppInstanceId);

            if (!this.AppRuntime.IsRoot())
            {
                // then create an output channel to root.
                // the root created the IN when starting the child process.
                var (rootOutChannel, _) = this.EnsureOutChannel(this.lazyOrchestrationManager.Value.GetRootAppInstanceId());

                // send a join message
                await this.PublishAsync(new JoinPeerMessage { AppInstanceId = this.AppRuntime.GetAppInstanceId()! }, rootOutChannel, cancellationToken).PreserveThreadContext();
            }

            this.channelsInitialized = true;
        }

        /// <summary>
        /// Calculates the root channel name.
        /// </summary>
        /// <returns>
        /// The calculated root channel name.
        /// </returns>
        protected override string ComputeRootChannelName() => $"channel-{base.ComputeRootChannelName()}";

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

            if (!this.channelsInitialized)
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
                this.DisposeChannels();
            }
            finally
            {
                this.appStoppedSubscription?.Dispose();
                this.appStoppedSubscription = null;

                base.Dispose(disposing);
            }
        }

        /// <summary>
        /// Disposes the channels and the whole infrastructure.
        /// </summary>
        protected virtual void DisposeChannels()
        {
            if (!this.channelsInitialized)
            {
                return;
            }

            // cancel the dispose source to start cancelling the reading.
            this.disposeSource?.Cancel();
            this.disposeSource = null;

            // clear the channels
            this.inChannel = null;
            this.outChannels.Clear();

            this.channelsInitialized = false;
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
            if (!this.channelsInitialized)
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
            var (clientChannel, _) = this.EnsureOutChannel(appInstanceId);
            if (clientChannel.IsSelf)
            {
                return await base.RouteOutputAsync(message, context, cancellationToken).PreserveThreadContext();
            }

            var serializedMessage = await this.serializationService
                .SerializeAsync(message, ctx => ctx.IncludeTypeInfo(true), cancellationToken)
                .PreserveThreadContext();
            await clientChannel.WriteAsync(serializedMessage!, cancellationToken).PreserveThreadContext();

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
                    await this.HandleJoinPeerMessageAsync(joinPeerMessage, cancellationToken)
                        .PreserveThreadContext();
                    break;
                case PeersChangedMessage peersChangedMessage:
                    await this.HandlePeersChangedMessageAsync(peersChangedMessage, cancellationToken)
                        .PreserveThreadContext();
                    break;
                case IBrokeredMessage brokeredMessage:
                    await this.HandleBrokeredMessageAsync(brokeredMessage, cancellationToken)
                        .PreserveThreadContext();
                    break;
                default:
                    this.Logger.Warn("Unsupported message '{message}'.", messageString);
                    break;
            }
        }

        private async Task HandlePeersChangedMessageAsync(
            PeersChangedMessage peersChangedMessage,
            CancellationToken cancellationToken)
        {
            this.Logger.Info("Received peers changed notification from {peer}. Peers: {peers}.", peersChangedMessage.AppInstanceId, peersChangedMessage.Apps);

            if (peersChangedMessage.Apps == null)
            {
                return;
            }

            foreach (var appInfo in peersChangedMessage.Apps)
            {
                this.EnsureOutChannel(appInfo.AppInstanceId);
            }
        }

        private async Task HandleBrokeredMessageAsync(
            IBrokeredMessage brokeredMessage,
            CancellationToken cancellationToken)
        {
            try
            {
                this.Logger.Trace("Received brokered message: '{message}'.", brokeredMessage);

                await this.RouteInputAsync(brokeredMessage, this.AppContext, cancellationToken)
                    .PreserveThreadContext();
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex, "Error while routing message: '{message}'.", brokeredMessage);
            }
        }

        private async Task HandleJoinPeerMessageAsync(
            JoinPeerMessage joinPeerMessage,
            CancellationToken cancellationToken)
        {
            this.Logger.Info("Peer {peer} sent a join request.", joinPeerMessage.AppInstanceId);

            var (peerChannel, existing) = this.EnsureOutChannel(joinPeerMessage.AppInstanceId);
            try
            {
                await this.PublishAsync(
                    new PeersChangedMessage
                    {
                        AppInstanceId = this.AppRuntime.GetAppInstanceId()!,
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
                    joinPeerMessage.AppInstanceId);
            }
        }

        private void HandleAppStoppedEvent(AppStoppedEvent e, IContext? context)
        {
            var peerAppInstanceId = e.AppInfo.AppInstanceId;
            if (e.AppInfo.AppId == this.AppRuntime.GetAppId() &&
                peerAppInstanceId == this.AppRuntime.GetAppInstanceId())
            {
                return;
            }

            if (!this.outChannels.TryRemove(peerAppInstanceId, out var peerOutChannel))
            {
                this.Logger.Warn("Peer output channel not found for {peer}.", peerAppInstanceId);
            }
        }

        private (ClientChannel channel, bool existing) EnsureOutChannel(string peerAppInstanceId)
        {
            if (this.outChannels.TryGetValue(peerAppInstanceId, out var clientChannel))
            {
                return (clientChannel, true);
            }

            var channelName = this.GetOutChannelName(peerAppInstanceId);
            clientChannel = new ClientChannel(peerAppInstanceId, ".", channelName, this.pipesConfiguration, this.Logger, peerAppInstanceId == this.AppRuntime.GetAppInstanceId());
            this.outChannels.TryAdd(
                peerAppInstanceId,
                clientChannel);
            return (clientChannel, false);
        }

        private void EnsureInChannel(string thisAppInstanceId)
        {
            if (this.inChannel != null)
            {
                return;
            }

            var channelName = this.GetInChannelName();
            this.inChannel = new ServerChannel(channelName, this.Logger, this.OnMessageReceivedAsync, this.disposeSource!.Token);
            this.inChannel.Open();
        }

        private string GetOutChannelName(string peerAppInstanceId)
        {
            return this.GetChannelName(peerAppInstanceId);
        }

        private string GetInChannelName()
        {
            return this.GetChannelName(this.AppRuntime.GetAppInstanceId()!);
        }

        private string GetChannelName(string appInstanceId)
        {
            var pipesNS = this.pipesConfiguration.GetSettings().Namespace;
            var prefix = string.IsNullOrEmpty(pipesNS) ? ChannelType : $"{pipesNS}_{ChannelType}";

            var channelName = $"{prefix}.{appInstanceId}";
            return channelName;
        }
    }
}