// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConsoleAppShutdownAwaiter.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the console application shutdown awaiter class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application.Console.Application
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Messaging.Events;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// A console application shutdown awaiter.
    /// </summary>
    public class ConsoleAppShutdownAwaiter : IAppShutdownAwaiter
    {
        private readonly CancellationTokenSource cancellationTokenSource;
        private readonly ICommandShell shell;
        private readonly IEventSubscription shutdownSubscription;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleAppShutdownAwaiter"/> class.
        /// </summary>
        /// <param name="shell">The shell.</param>
        /// <param name="eventHub">The event hub.</param>
        public ConsoleAppShutdownAwaiter(ICommandShell shell, IEventHub eventHub)
        {
            this.shell = shell;
            this.shutdownSubscription = eventHub.Subscribe<ShutdownSignal>((e, ctx) => this.StopShell());
            this.cancellationTokenSource = new CancellationTokenSource();
        }

        /// <summary>
        /// Gets or sets a value indicating whether this console is interactive.
        /// </summary>
        /// <value>
        /// True if this object is interactive, false if not.
        /// </value>
        public bool IsInteractive { get; set; } = true;

        /// <summary>
        /// Waits for the shutdown signal asynchronously.
        /// </summary>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the shutdown result.
        /// </returns>
        public async Task<(object result, AppShutdownInstruction instruction)> WaitForShutdownSignalAsync(CancellationToken cancellationToken = default)
        {
            var completionSource = new TaskCompletionSource<object>();

            using (this.cancellationTokenSource)
            using (this.cancellationTokenSource.Token.Register(() => completionSource.TrySetResult(this.GetUnattendedResult())))
            using (this.shutdownSubscription)
            {
                if (this.IsInteractive)
                {
                    try
                    {
                        await this.shell.StartAsync(this.cancellationTokenSource.Token).PreserveThreadContext();

                        return (this.GetAttendedResult(), AppShutdownInstruction.Shutdown);
                    }
                    catch (OperationCanceledException)
                    {
                        return (this.GetUnattendedResult(), AppShutdownInstruction.Shutdown);
                    }
                    catch (Exception ex)
                    {
                        return (this.GetExceptionResult(ex), AppShutdownInstruction.Shutdown);
                    }
                }

                return (await completionSource.Task.PreserveThreadContext(), AppShutdownInstruction.Shutdown);
            }
        }

        /// <summary>
        /// Stops the shell.
        /// </summary>
        protected virtual void StopShell()
        {
            this.cancellationTokenSource.Cancel();
        }

        /// <summary>
        /// Gets the unattended result.
        /// </summary>
        /// <returns>
        /// The unattended result.
        /// </returns>
        protected virtual object GetUnattendedResult() => null;

        /// <summary>
        /// Gets the attended result.
        /// </summary>
        /// <returns>
        /// The attended result.
        /// </returns>
        protected virtual object GetAttendedResult() => null;

        /// <summary>
        /// Gets the exception result.
        /// </summary>
        /// <param name="ex">The ex.</param>
        /// <returns>
        /// The exception result.
        /// </returns>
        protected virtual object GetExceptionResult(Exception ex) => ex;
    }
}
