// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultAppShutdownAwaiter.cs" company="Kephas Software SRL">
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
    using Kephas.Operations;
    using Kephas.Services;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// A default application shutdown awaiter.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class DefaultAppShutdownAwaiter : IAppShutdownAwaiter
    {
        private readonly CancellationTokenSource cancellationTokenSource;
        private readonly IEventSubscription shutdownSubscription;
        private bool unattendedCompletion = false;
        private TaskCompletionSource<IOperationResult> completionSource;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultAppShutdownAwaiter"/> class.
        /// </summary>
        /// <param name="eventHub">The event hub.</param>
        public DefaultAppShutdownAwaiter(IEventHub eventHub)
        {
            this.shutdownSubscription = eventHub.Subscribe<ShutdownSignal>((e, ctx) => this.HandleShutdownSignal());
            this.cancellationTokenSource = new CancellationTokenSource();
        }

        /// <summary>
        /// Gets or sets a value indicating whether the application is attended/interactive.
        /// </summary>
        /// <value>
        /// True if the application is attended/interactive, false if not.
        /// </value>
        public bool IsAttended { get; internal protected set; } = true;

        /// <summary>
        /// Waits for the shutdown signal asynchronously.
        /// </summary>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the shutdown result.
        /// </returns>
        public async Task<(IOperationResult result, AppShutdownInstruction instruction)> WaitForShutdownSignalAsync(CancellationToken cancellationToken = default)
        {
            this.completionSource = new TaskCompletionSource<IOperationResult>();

            var stopwatch = Stopwatch.StartNew();

            using (this.cancellationTokenSource)
            using (this.cancellationTokenSource.Token.Register(() => this.completionSource.TrySetResult(this.GetUnattendedResult())))
            using (this.shutdownSubscription)
            {
                if (this.IsAttended)
                {
                    try
                    {
                        await this.RunAttendedAsync(this.cancellationTokenSource.Token).PreserveThreadContext();

                        this.cancellationTokenSource.Token.ThrowIfCancellationRequested();

                        return (this.GetAttendedResult(stopwatch.Elapsed), AppShutdownInstruction.Shutdown);
                    }
                    catch (OperationCanceledException)
                    {
                        return (this.unattendedCompletion
                                    ? this.GetUnattendedResult(stopwatch.Elapsed)
                                    : this.GetAttendedResult(stopwatch.Elapsed),
                                AppShutdownInstruction.Shutdown);
                    }
                    finally
                    {
                        stopwatch.Stop();
                    }
                }

                try
                {
                    await this.RunUnattendedAsync(this.cancellationTokenSource.Token).PreserveThreadContext();

                    this.cancellationTokenSource.Token.ThrowIfCancellationRequested();

                    return (this.GetUnattendedResult(stopwatch.Elapsed), AppShutdownInstruction.Shutdown);
                }
                catch (OperationCanceledException)
                {
                    return (this.GetUnattendedResult(stopwatch.Elapsed), AppShutdownInstruction.Shutdown);
                }
                finally
                {
                    stopwatch.Stop();
                }
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
            return this.completionSource.Task;
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
        /// <param name="elapsed">The elapsed time.</param>
        /// <returns>
        /// The unattended result.
        /// </returns>
        protected virtual IOperationResult GetUnattendedResult(TimeSpan? elapsed = null)
            => new OperationResult()
                    .Complete(elapsed ?? TimeSpan.Zero, OperationState.Canceled);

        /// <summary>
        /// Gets the attended result.
        /// </summary>
        /// <param name="elapsed">Optional. The elapsed time.</param>
        /// <returns>
        /// The attended result.
        /// </returns>
        protected virtual IOperationResult GetAttendedResult(TimeSpan? elapsed = null)
            => new OperationResult()
                    .Complete(elapsed ?? TimeSpan.Zero, OperationState.Completed);
    }
}
