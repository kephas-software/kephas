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

        private ConcurrentDictionary<string, PeerChannel> peerOutChannels
            = new ConcurrentDictionary<string, PeerChannel>();

        private ConcurrentDictionary<string, ServerChannel> serverInChannels
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
                await this.PublishAsync(
                        new JoinPeerMessage(),
                        rootOutChannel.Stream!,
                        rootOutChannel.ChannelName)
                    .PreserveThreadContext();
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
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The server stream.</returns>
        protected virtual async Task<NamedPipeClientStream> OpenNamedPipeClientStreamAsync(string? serverName, string channelName, CancellationToken cancellationToken)
        {
            serverName ??= ".";
            var stream = new NamedPipeClientStream(
                serverName,
                channelName,
                PipeDirection.InOut,
                PipeOptions.Asynchronous);

            try
            {
                this.Logger.Debug("Connecting to pipe {server}/{channel} (client mode)...", serverName, channelName);

                await stream.ConnectAsync((int)this.pipesConfiguration.Settings.ConnectionTimeout.TotalMilliseconds, cancellationToken).PreserveThreadContext();
                stream.ReadMode = PipeTransmissionMode.Message;

                this.Logger.Debug("Connected to pipe {server}/{channel} (client mode).", serverName, channelName);
                return stream;
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex, "Error while connecting to {server}/{channel} (client mode).", serverName, channelName);
                throw;
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

            // dispose the server output channels
            foreach (var serverInChannel in this.serverInChannels.Values)
            {
                serverInChannel.Stream?.Dispose();
                this.Logger.Debug("Pipe {channel} disposed.", serverInChannel.ChannelName);
            }

            this.serverInChannels.Clear();

            // dispose the peer output channels
            foreach (var peerOutChannel in this.peerOutChannels.Values)
            {
                peerOutChannel.Stream?.Dispose();
                this.Logger.Debug("Pipe {server}/{channel} disposed.", peerOutChannel.ServerName, peerOutChannel.ChannelName);
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
                    ? string.Join("\t", this.peerOutChannels.Keys)
                    : string.Join("\t", this.peerOutChannels.Keys.Where(p => p.StartsWith(recipient.AppId, StringComparison.OrdinalIgnoreCase)))
                : recipient.AppInstanceId;
        }

        private async Task<(RoutingInstruction action, IMessage? reply)> PublishAsync(
            IBrokeredMessage message,
            IDispatchingContext context,
            string appInstanceId,
            CancellationToken cancellationToken)
        {
            if (this.peerOutChannels.TryGetValue(appInstanceId, out var peerChannel))
            {
                if (peerChannel.IsSelf)
                {
                    return await base.RouteOutputAsync(message, context, cancellationToken).PreserveThreadContext();
                }

                var serializedMessage = await this.serializationService
                    .SerializeAsync(message, ctx => ctx.IncludeTypeInfo(true), cancellationToken)
                    .PreserveThreadContext();
                var messageBytes = Encoding.UTF8.GetBytes(serializedMessage);
                await this.PublishAsync(messageBytes, peerChannel.Stream!, peerChannel.ChannelName).PreserveThreadContext();
            }
            else
            {
                this.Logger.Warn("Peer '{peer}' not registered.", appInstanceId);
            }

            return (RoutingInstruction.None, null);
        }

        private async Task PublishAsync(object message, Stream peerOutput, string channelName)
        {
            var messageString = await this.serializationService
                .SerializeAsync(message, ctx => ctx.IncludeTypeInfo(true))
                .PreserveThreadContext();
            var messageBytes = Encoding.UTF8.GetBytes(messageString);
            await this.PublishAsync(messageBytes, peerOutput, channelName).PreserveThreadContext();
        }

        private async Task PublishAsync(byte[] messageBytes, Stream peerOutput, string channelName)
        {
            // TODO optimize to write in parallel to multiple outs, do not block with this global async lock.
            await this.outWriteLock!.EnterAsync(async () =>
            {
                this.Logger.Trace("Writing message ({messageLength}) to {channel}...", messageBytes.Length, channelName);

                await peerOutput.WriteAsync(messageBytes, 0, messageBytes.Length).PreserveThreadContext();
                await peerOutput.FlushAsync().PreserveThreadContext();

                this.Logger.Trace("Written message ({messageLength}) to {channel}.", messageBytes.Length, channelName);
            }).PreserveThreadContext();
        }

        private async Task StartListeningAsync(
            string peerAppInstanceId,
            NamedPipeServerStream channel,
            string channelName,
            CancellationToken cancellationToken)
        {
            await Task.Yield();

            this.Logger.Debug("Waiting for connection on {channel} (server mode)...", channelName);

            await channel.WaitForConnectionAsync(cancellationToken).PreserveThreadContext();

            this.Logger.Debug("Client connected on {channel} (server mode).", channelName);

            var (peerChannel, existing) = await this.EnsureOutChannelAsync(peerAppInstanceId, cancellationToken).PreserveThreadContext();

            if (!existing)
            {
                this.Logger.Debug("Created output channel {channel} (server mode).", channelName);
            }

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var messageString = await this.ReadMessageAsync(channel, cancellationToken).PreserveThreadContext();

                    if (this.Logger.IsDebugEnabled())
                    {
                        this.Logger.Debug("Message arrived on {channel} (server mode): '{message}'.", channelName, messageString.Substring(0, 50) + "...");
                    }

                    var message = await this.serializationService
                        .DeserializeAsync(messageString, cancellationToken: cancellationToken)
                        .PreserveThreadContext();

                    switch (message)
                    {
                        case JoinPeerMessage joinPeerMessage:
                            await this.HandleJoinPeerMessageAsync(peerAppInstanceId, joinPeerMessage, cancellationToken).PreserveThreadContext();
                            break;
                        case PeersChangedMessage peersChangedMessage:
                            await this.HandlePeersChangedMessageAsync(peerAppInstanceId, peersChangedMessage, cancellationToken).PreserveThreadContext();
                            break;
                        case HandshakeMessage handshakeMessage:
                            await this.HandleHandshakeMessageAsync(peerAppInstanceId, handshakeMessage, cancellationToken).PreserveThreadContext();
                            break;
                        case IBrokeredMessage brokeredMessage:
                            await this.HandleBrokeredMessageAsync(peerAppInstanceId, brokeredMessage, cancellationToken).PreserveThreadContext();
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
                if (!this.peerOutChannels.TryGetValue(handshakeMessage.Target, out var targetChannel))
                {
                    this.Logger.Warn("Disconnected from {peer} while trying to redirect handshake request from {source}.", handshakeMessage.Target, peerAppInstanceId);
                    return;
                }

                try
                {
                    await this.PublishAsync(handshakeMessage, targetChannel.Stream!, targetChannel.ChannelName).PreserveThreadContext();
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

                if (!this.peerOutChannels.TryGetValue(peerAppInstanceId, out var senderChannel))
                {
                    this.Logger.Warn("Disconnected from {peer} when trying to request handshake.", peerAppInstanceId);
                    continue;
                }

                try
                {
                    await this.PublishAsync(
                        new HandshakeMessage { Target = appInfo.AppInstanceId },
                        senderChannel.Stream!,
                        senderChannel.ChannelName).PreserveThreadContext();
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
                await this.RouteInputAsync(brokeredMessage, this.AppContext, cancellationToken)
                    .PreserveThreadContext();
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex, "Error while routing from input '{message}'.", brokeredMessage);
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
                    peerChannel.Stream!,
                    peerChannel.ChannelName).PreserveThreadContext();
            }
            catch (Exception ex)
            {
                this.Logger.Error(
                    ex,
                    "Error responding to initialize peer message from '{peer}'.",
                    peerAppInstanceId);
            }
        }

        private async Task<string> ReadMessageAsync(NamedPipeServerStream channel, CancellationToken cancellationToken)
        {
            var buffer = new byte[256];
            using var memStream = new MemoryStream(1000);
            do
            {
                cancellationToken.ThrowIfCancellationRequested();

                var numBytes = await channel.ReadAsync(buffer, 0, buffer.Length, cancellationToken).PreserveThreadContext();
                memStream.Write(buffer, 0, numBytes);
            }
            while (!cancellationToken.IsCancellationRequested && !channel.IsMessageComplete);

            memStream.Position = 0;
            var bytes = memStream.ReadAllBytes();

            return Encoding.UTF8.GetString(bytes);
        }

        private void HandleAppStoppedEvent(AppStoppedEvent e, IContext context)
        {
            var peerAppInstanceId = e.AppInfo.AppInstanceId;
            if (e.AppInfo.AppId == this.AppRuntime.GetAppId() &&
                peerAppInstanceId == this.AppRuntime.GetAppInstanceId())
            {
                return;
            }

            if (this.serverInChannels.TryRemove(peerAppInstanceId, out var serverChannel))
            {
                serverChannel.Stream?.Dispose();
            }
            else
            {
                this.Logger.Warn("Server channel not found for {peer}.", peerAppInstanceId);
            }

            if (this.peerOutChannels.TryRemove(peerAppInstanceId, out var peerOutChannel))
            {
                peerOutChannel.Stream?.Dispose();
            }
            else
            {
                this.Logger.Warn("Peer input channel not found for {peer}.", peerAppInstanceId);
            }
        }

        private async Task<(PeerChannel channel, bool existing)> EnsureOutChannelAsync(string peerAppInstanceId, CancellationToken cancellationToken)
        {
            if (this.peerOutChannels.TryGetValue(peerAppInstanceId, out var peerChannel))
            {
                return (peerChannel, true);
            }

            var channelName = this.GetOutChannelName(peerAppInstanceId);
            var channel = peerAppInstanceId == this.AppRuntime.GetAppInstanceId()
                ? null
                : await this.OpenNamedPipeClientStreamAsync(".", channelName, cancellationToken).PreserveThreadContext();
            this.peerOutChannels.TryAdd(
                peerAppInstanceId,
                peerChannel = new PeerChannel(peerAppInstanceId, ".", channelName, channel));
            return (peerChannel, false);
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
            if (this.serverInChannels.TryGetValue(peerAppInstanceId, out var serverChannel))
            {
                return (serverChannel, true);
            }

            var channelName = this.GetInChannelName(peerAppInstanceId);
            var channel = this.OpenNamedPipeServerStream(channelName);
            var listenTask = this.StartListeningAsync(peerAppInstanceId, channel, channelName, this.disposeSource!.Token);

            this.serverInChannels.TryAdd(
                peerAppInstanceId,
                serverChannel = new ServerChannel(peerAppInstanceId, channelName, channel, listenTask));
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

        private struct ServerChannel
        {
            public ServerChannel(string appInstanceId, string channelName, NamedPipeServerStream stream, Task listenTask)
            {
                this.AppInstanceId = appInstanceId;
                this.ChannelName = channelName;
                this.Stream = stream;
                this.ListenTask = listenTask;
            }

            public string AppInstanceId { get; }

            public string ChannelName { get; }

            public NamedPipeServerStream Stream { get; }

            public Task ListenTask { get; }
        }

        private struct PeerChannel
        {
            public PeerChannel(string appInstanceId, string serverName, string channelName, NamedPipeClientStream? stream)
            {
                this.AppInstanceId = appInstanceId;
                this.ServerName = serverName;
                this.ChannelName = channelName;
                this.Stream = stream;
                this.IsSelf = stream == null;
            }

            public string AppInstanceId { get; set; }

            public string ServerName { get; set; }

            public string ChannelName { get; set; }

            public bool IsSelf { get; set; }

            public NamedPipeClientStream? Stream { get; set; }
        }
    }
}