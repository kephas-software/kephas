// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultAppMainLoop.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the null application termination awaiter class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Interaction;
    using Kephas.Logging;
    using Kephas.Operations;
    using Kephas.Services;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// A default application shutdown awaiter.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class DefaultAppMainLoop : Loggable, IAppMainLoop
    {
        private readonly CancellationTokenSource cancellationTokenSource;
        private readonly IEventSubscription shutdownSubscription;
        private bool unattendedCompletion = false;
        private TaskCompletionSource<IOperationResult>? completionSource;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultAppMainLoop"/> class.
        /// </summary>
        /// <param name="eventHub">The event hub.</param>
        /// <param name="logManager">Optional. The log manager.</param>
        public DefaultAppMainLoop(IEventHub eventHub, ILogManager? logManager = null)
            : base(logManager)
        {
            this.shutdownSubscription = eventHub.Subscribe<ShutdownSignal>((e, ctx) => this.HandleShutdownSignal());
            this.cancellationTokenSource = new CancellationTokenSource();
        }

        /// <summary>
        /// Gets a value indicating whether the application is attended/interactive.
        /// </summary>
        /// <value>
        /// True if the application is attended/interactive, false if not.
        /// </value>
        public virtual bool IsAttended => true;

        /// <summary>
        /// Executes the application's main loop asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The application lifetime token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the shutdown result.
        /// </returns>
        public virtual async Task<MainLoopResult> Main(CancellationToken cancellationToken)
        {
            this.completionSource = new TaskCompletionSource<IOperationResult>();

            var result = new OperationResult();

            using var source = this.cancellationTokenSource;
            using var registration = this.cancellationTokenSource.Token.Register(() => this.completionSource.TrySetResult(this.GetUnattendedResult(result)));
            using var register = cancellationToken.Register(() => this.completionSource.TrySetResult(this.GetUnattendedResult(result)));
            using var subscription = this.shutdownSubscription;
            if (this.IsAttended)
            {
                try
                {
                    await this.RunAttendedAsync(this.cancellationTokenSource.Token).PreserveThreadContext();

                    this.cancellationTokenSource.Token.ThrowIfCancellationRequested();

                    return new MainLoopResult(this.GetAttendedResult(result), AppShutdownInstruction.Shutdown);
                }
                catch (OperationCanceledException)
                {
                    return new MainLoopResult(
                        this.unattendedCompletion
                            ? this.GetUnattendedResult(result)
                            : this.GetAttendedResult(result),
                        AppShutdownInstruction.Shutdown);
                }
            }

            try
            {
                await this.RunUnattendedAsync(this.cancellationTokenSource.Token).PreserveThreadContext();

                this.cancellationTokenSource.Token.ThrowIfCancellationRequested();

                return new MainLoopResult(this.GetUnattendedResult(result), AppShutdownInstruction.Shutdown);
            }
            catch (OperationCanceledException)
            {
                return new MainLoopResult(this.GetUnattendedResult(result), AppShutdownInstruction.Shutdown);
            }
        }

        /// <summary>
        /// Executes the attended asynchronous operation.
        /// </summary>
        /// <param name="cancellationToken">A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        protected virtual Task RunAttendedAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Executes the unattended asynchronous operation.
        /// </summary>
        /// <param name="cancellationToken">A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        protected virtual Task RunUnattendedAsync(CancellationToken cancellationToken)
        {
            return this.completionSource?.Task ?? Task.CompletedTask;
        }

        /// <summary>
        /// Handles the shutdown signal.
        /// </summary>
        protected virtual void HandleShutdownSignal()
        {
            this.unattendedCompletion = true;
            this.cancellationTokenSource.Cancel();
        }

        /// <summary>
        /// Gets the unattended result.
        /// </summary>
        /// <param name="result">The result to be set as canceled.</param>
        /// <returns>
        /// The unattended result.
        /// </returns>
        protected virtual IOperationResult GetUnattendedResult(IOperationResult result)
            => result.Complete(operationState: OperationState.Canceled);

        /// <summary>
        /// Gets the attended result.
        /// </summary>
        /// <param name="result">The result to be set as completed.</param>
        /// <returns>
        /// The attended result.
        /// </returns>
        protected virtual IOperationResult GetAttendedResult(IOperationResult result)
            => result.Complete(operationState: OperationState.Completed);
    }
}
