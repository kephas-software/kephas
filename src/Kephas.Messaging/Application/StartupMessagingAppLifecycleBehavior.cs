// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StartupMessagingAppLifecycleBehavior.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Application
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Application.Interaction;
    using Kephas.Dynamic;
    using Kephas.Interaction;
    using Kephas.Operations;
    using Kephas.Services;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Application lifecycle behavior executing startup commands.
    /// </summary>
    public class StartupMessagingAppLifecycleBehavior : IAppLifecycleBehavior
    {
        private readonly IMessageProcessor messageProcessor;
        private readonly IEventSubscription subscription;

        /// <summary>
        /// Initializes a new instance of the <see cref="StartupMessagingAppLifecycleBehavior"/> class.
        /// </summary>
        /// <param name="messageProcessor">The command processor.</param>
        /// <param name="eventHub">The event hub.</param>
        public StartupMessagingAppLifecycleBehavior(IMessageProcessor messageProcessor, IEventHub eventHub)
        {
            this.messageProcessor = messageProcessor;
            this.subscription = eventHub.Subscribe<ExecuteStartupCommandSignal>(this.ExecuteStartupCommandAsync);
        }

        /// <summary>
        /// Interceptor called after the application completes its asynchronous initialization.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result yielding the operation result.
        /// </returns>
        public Task<IOperationResult> AfterAppInitializeAsync(
            IAppContext appContext,
            CancellationToken cancellationToken = default)
        {
            this.subscription.Dispose();
            return Task.FromResult((IOperationResult)true.ToOperationResult());
        }

        private async Task<object?> ExecuteStartupCommandAsync(ExecuteStartupCommandSignal signal, IContext? context, CancellationToken cancellationToken)
        {
            if (signal.Command is string || signal.Command.GetType().IsValueType)
            {
                return Task.FromResult<object?>(null);
            }

            var result = await this.messageProcessor
                .ProcessAsync(signal.Command, ctx => ctx.Merge(signal.ExecutionContext), cancellationToken)
                .PreserveThreadContext();

            throw new InterruptSignal(result: result);
        }
    }
}
