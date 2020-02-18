// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PreserveCultureThreadContextAppLifecycleBehavior.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the preserve culture thread context feature manager class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application
{
    using System.Globalization;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Services;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Feature manager configuring the <see cref="ThreadContextAwaiter"/> to preserve the thread culture.
    /// </summary>
    [ProcessingPriority(Priority.Highest)]
    public class PreserveCultureThreadContextAppLifecycleBehavior : IAppLifecycleBehavior
    {
        /// <summary>
        /// Interceptor called before the application starts its asynchronous initialization.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// The asynchronous result.
        /// </returns>
        public Task BeforeAppInitializeAsync(IContext appContext, CancellationToken cancellationToken = default)
        {
            new ThreadContextBuilder()
                .WithStoreAction(StoreThreadCulture)
                .WithRestoreAction(RestoreThreadCulture);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Interceptor called after the application completes its asynchronous initialization.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// The asynchronous result.
        /// </returns>
        public Task AfterAppInitializeAsync(IContext appContext, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Interceptor called before the application starts its asynchronous finalization.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">Optional. The cancellation token (optional).</param>
        /// <returns>
        /// A Task.
        /// </returns>
        public Task BeforeAppFinalizeAsync(IContext appContext, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Interceptor called after the application completes its asynchronous finalization.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">Optional. The cancellation token (optional).</param>
        /// <returns>
        /// A Task.
        /// </returns>
        public Task AfterAppFinalizeAsync(IContext appContext, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Stores the current culture in the threading context.
        /// </summary>
        /// <param name="threadContext">Context for the server threading.</param>
        private static void StoreThreadCulture(ThreadContext threadContext)
        {
            threadContext.CurrentCulture = CultureInfo.CurrentCulture;
            threadContext.CurrentUICulture = CultureInfo.CurrentUICulture;
        }

        /// <summary>
        /// Restores the current culture from the threading context.
        /// </summary>
        /// <param name="threadContext">Context for the server threading.</param>
        private static void RestoreThreadCulture(ThreadContext threadContext)
        {
            if (threadContext.CurrentCulture != null)
            {
                CultureInfo.CurrentCulture = threadContext.CurrentCulture;
            }

            if (threadContext.CurrentUICulture != null)
            {
                CultureInfo.CurrentUICulture = threadContext.CurrentUICulture;
            }
        }
    }
}