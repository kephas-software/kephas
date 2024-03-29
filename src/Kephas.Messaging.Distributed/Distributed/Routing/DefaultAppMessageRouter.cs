﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultAppMessageRouter.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Distributed.Routing;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Kephas.Application;
using Kephas.Logging;
using Kephas.Messaging;
using Kephas.Messaging.Distributed;
using Kephas.Messaging.Distributed.Queues;
using Kephas.Services;
using Kephas.Threading.Tasks;

/// <summary>
/// An in process message router invoking the message processor.
/// </summary>
[ProcessingPriority(Priority.Lowest)]
[MessageRouter(ReceiverMatch = ChannelType + ":.*", IsFallback = true)]
public class DefaultAppMessageRouter : MessageRouterBase
{
    /// <summary>
    /// The channel type handled by the <see cref="DefaultAppMessageRouter"/>.
    /// </summary>
    public const string ChannelType = Endpoint.AppScheme;

    private readonly IMessageQueueStore queueStore;

    private IMessageQueue messageQueue;
    private IMessageQueue appMessageQueue;
    private IMessageQueue appInstanceMessageQueue;

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultAppMessageRouter"/> class.
    /// </summary>
    /// <param name="injectableFactory">The injectable factory.</param>
    /// <param name="appRuntime">The application runtime.</param>
    /// <param name="messageProcessor">The message processor.</param>
    /// <param name="queueStore">The message queue store.</param>
    public DefaultAppMessageRouter(
        IInjectableFactory injectableFactory,
        IAppRuntime appRuntime,
        IMessageProcessor messageProcessor,
        IMessageQueueStore queueStore)
        : base(injectableFactory, appRuntime, messageProcessor)
    {
        this.queueStore = queueStore;
    }

    /// <summary>
    /// Gets the name of the root channel.
    /// </summary>
    /// <value>
    /// The name of the root channel.
    /// </value>
    protected string RootChannelName { get; private set; }

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
        await Task.Yield();

        this.RootChannelName = this.ComputeRootChannelName();
        this.messageQueue = this.queueStore.GetMessageQueue(this.RootChannelName);
        this.messageQueue.MessageArrived += this.HandleMessageArrivedAsync;

        var appChannelName = $"{this.RootChannelName}:{this.AppRuntime.GetAppId()}";
        this.appMessageQueue = this.queueStore.GetMessageQueue(appChannelName);
        this.appMessageQueue.MessageArrived += this.HandleMessageArrivedAsync;

        var appInstanceChannelName = $"{this.RootChannelName}:{this.AppRuntime.GetAppInstanceId()}";
        this.appInstanceMessageQueue = this.queueStore.GetMessageQueue(appInstanceChannelName);
        this.appInstanceMessageQueue.MessageArrived += this.HandleMessageArrivedAsync;
    }

    /// <summary>
    /// Calculates the root channel name.
    /// </summary>
    /// <returns>
    /// The calculated root channel name.
    /// </returns>
    protected virtual string ComputeRootChannelName() => ChannelType;

    /// <summary>
    /// Handles the message arrived event.
    /// </summary>
    /// <param name="sender">Source of the event.</param>
    /// <param name="e">Message event information.</param>
    /// <returns>
    /// An asynchronous result.
    /// </returns>
    protected virtual async Task HandleMessageArrivedAsync(object sender, MessageEventArgs e)
    {
        try
        {
            var brokeredMessage = e.Message;
            await this.RouteInputAsync(brokeredMessage, this.AppContext, default).PreserveThreadContext();
        }
        catch (Exception ex)
        {
            this.Logger.Error(ex, "Error while handling message '{message}'.", e.Message);
        }
    }

    /// <summary>
    /// Processes the brokered message locally, asynchronously.
    /// </summary>
    /// <param name="brokeredMessage">The brokered message.</param>
    /// <param name="context">The routing context.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// An asynchronous result that yields the reply message.
    /// </returns>
    protected override Task<object?> ProcessAsync(IBrokeredMessage brokeredMessage, IContext context,
        CancellationToken cancellationToken)
    {
        var completionSource = new TaskCompletionSource<object?>();

        // make processing really async for in-process handling
        Task.Factory.StartNew(
            async () =>
            {
                try
                {
                    var result = await base.ProcessAsync(brokeredMessage, context, cancellationToken).PreserveThreadContext();
                    completionSource.SetResult(result);
                }
                catch (Exception ex)
                {
                    completionSource.SetException(ex);
                }
            },
            cancellationToken);

        return completionSource.Task;
    }

    /// <summary>
    /// Routes the brokered message asynchronously, typically over the physical medium.
    /// </summary>
    /// <param name="brokeredMessage">The brokered message.</param>
    /// <param name="context">The dispatching context.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// The asynchronous result yielding an action to take further and an optional reply.
    /// </returns>
    protected override async Task<(RoutingInstruction action, object? reply)> RouteOutputAsync(
        IBrokeredMessage brokeredMessage, IDispatchingContext context, CancellationToken cancellationToken)
    {
        this.InitializationMonitor.AssertIsCompletedSuccessfully();

        if (brokeredMessage.Recipients?.Any() ?? false)
        {
            var groups = brokeredMessage.Recipients
                .GroupBy(r => this.GetChannelName(r))
                .Select(g => (channelName: g.Key, recipients: g))
                .ToList();

            if (groups.Count == 1)
            {
                await this.PublishAsync(brokeredMessage, groups[0].channelName).PreserveThreadContext();
            }
            else
            {
                foreach (var (channelName, recipients) in groups)
                {
                    await this.PublishAsync(brokeredMessage.Clone(recipients.ToList()), channelName).PreserveThreadContext();
                }
            }
        }
        else
        {
            await this.PublishAsync(brokeredMessage, this.RootChannelName).PreserveThreadContext();
        }

        return (RoutingInstruction.None, null);
    }

    /// <summary>
    /// Gets the channel name for the provided recipient.
    /// </summary>
    /// <param name="recipient">The recipient.</param>
    /// <returns>
    /// The channel name.
    /// </returns>
    protected virtual string GetChannelName(IEndpoint recipient)
    {
        return string.IsNullOrEmpty(recipient.AppInstanceId)
            ? string.IsNullOrEmpty(recipient.AppId)
                ? this.RootChannelName
                : $"{this.RootChannelName}:{recipient.AppId}"
            : $"{this.RootChannelName}:{recipient.AppInstanceId}";
    }

    /// <summary>
    /// Releases the unmanaged resources used by the MessageRouterBase and optionally releases the
    /// managed resources.
    /// </summary>
    /// <param name="disposing">True to release both managed and unmanaged resources; false to
    ///                         release only unmanaged resources.</param>
    protected override void Dispose(bool disposing)
    {
        this.messageQueue.MessageArrived -= this.HandleMessageArrivedAsync;
        this.appMessageQueue.MessageArrived -= this.HandleMessageArrivedAsync;
        this.appInstanceMessageQueue.MessageArrived -= this.HandleMessageArrivedAsync;

        base.Dispose(disposing);
    }

    private async Task PublishAsync(IBrokeredMessage message, string channelName)
    {
        var queue = this.queueStore.GetMessageQueue(channelName);
        await queue.PublishAsync(message).PreserveThreadContext();
    }
}