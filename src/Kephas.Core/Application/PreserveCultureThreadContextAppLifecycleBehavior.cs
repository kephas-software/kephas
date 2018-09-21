// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PreserveCultureThreadContextFeatureManager.cs" company="Kephas Software SRL">
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
    public class PreserveCultureThreadContextAppLifecycleBehavior : AppLifecycleBehaviorBase
    {
        /// <summary>
        /// Interceptor called before the application starts its asynchronous initialization.
        /// </summary>
        /// <remarks>
        /// To interrupt the application initialization, simply throw an appropriate exception.
        /// </remarks>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A Task.
        /// </returns>
        public override Task BeforeAppInitializeAsync(IAppContext appContext, CancellationToken cancellationToken = default)
        {
            new ThreadContextBuilder(appContext.AmbientServices)
                .WithStoreAction(StoreThreadCulture)
                .WithRestoreAction(RestoreThreadCulture);

            return TaskHelper.CompletedTask;
        }

        /// <summary>
        /// Stores the current culture in the threading context.
        /// </summary>
        /// <param name="threadContext">Context for the server threading.</param>
        private static void StoreThreadCulture(ThreadContext threadContext)
        {
#if NETSTANDARD2_0
            threadContext.CurrentCulture = CultureInfo.CurrentCulture;
            threadContext.CurrentUICulture = CultureInfo.CurrentUICulture;
#else
            threadContext.CurrentCulture = Thread.CurrentThread.CurrentCulture;
            threadContext.CurrentUICulture = Thread.CurrentThread.CurrentUICulture;
#endif
        }

        /// <summary>
        /// Restores the current culture from the threading context.
        /// </summary>
        /// <param name="threadContext">Context for the server threading.</param>
        private static void RestoreThreadCulture(ThreadContext threadContext)
        {
#if NETSTANDARD2_0
            if (threadContext.CurrentCulture != null)
            {
                CultureInfo.CurrentCulture = threadContext.CurrentCulture;
            }

            if (threadContext.CurrentUICulture != null)
            {
                CultureInfo.CurrentUICulture = threadContext.CurrentUICulture;
            }
#else
            if (threadContext.CurrentCulture != null)
            {
                Thread.CurrentThread.CurrentCulture = threadContext.CurrentCulture;
            }

            if (threadContext.CurrentUICulture != null)
            {
                Thread.CurrentThread.CurrentUICulture = threadContext.CurrentUICulture;
            }
#endif
        }
    }
}