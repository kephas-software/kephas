// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StartupCommandsAppLifecycleBehavior.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Commands.Application
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Application.Interaction;
    using Kephas.Interaction;
    using Kephas.Operations;
    using Kephas.Services;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Application lifecycle behavior executing startup commands.
    /// </summary>
    [ProcessingPriority(Priority.Highest)]
    public class StartupCommandsAppLifecycleBehavior : IAppLifecycleBehavior
    {
        private readonly ICommandProcessor commandProcessor;
        private readonly IEventSubscription subscription;

        /// <summary>
        /// Initializes a new instance of the <see cref="StartupCommandsAppLifecycleBehavior"/> class.
        /// </summary>
        /// <param name="commandProcessor">The command processor.</param>
        /// <param name="eventHub">The event hub.</param>
        public StartupCommandsAppLifecycleBehavior(ICommandProcessor commandProcessor, IEventHub eventHub)
        {
            this.commandProcessor = commandProcessor;
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
            if (signal.Command is not CommandInfo command)
            {
                if (signal.Command is not string cmdLine)
                {
                    return Task.FromResult<object?>(null);
                }

                command = CommandInfo.Parse(cmdLine);
            }

            var result = await this.commandProcessor
                .ProcessAsync(command.Name, command.Args, signal.ExecutionContext, cancellationToken)
                .PreserveThreadContext();

            throw new InterruptSignal(result: result);
        }
    }
}