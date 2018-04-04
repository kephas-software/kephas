// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PreserveCultureThreadContextFeatureManager.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the preserve culture thread context feature manager class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Services;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Feature manager configuring the <see cref="ThreadContextAwaiter"/> to preserve the thread culture.
    /// </summary>
    [ProcessingPriority(Priority.Highest)]
    [FeatureInfo(FeatureName, isRequired: true)]
    public class PreserveCultureThreadContextFeatureManager : FeatureManagerBase
    {
        /// <summary>
        /// Name of the feature.
        /// </summary>
        public const string FeatureName = "PreserveCultureThreadContext";

        /// <summary>
        /// Initializes the <see cref="ThreadContextAwaiter"/> asynchronously.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        protected override Task InitializeCoreAsync(IAppContext appContext, CancellationToken cancellationToken)
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
            threadContext.CurrentCulture = Thread.CurrentThread.CurrentCulture;
            threadContext.CurrentUICulture = Thread.CurrentThread.CurrentUICulture;
        }

        /// <summary>
        /// Restores the current culture from the threading context.
        /// </summary>
        /// <param name="threadContext">Context for the server threading.</param>
        private static void RestoreThreadCulture(ThreadContext threadContext)
        {
            if (threadContext.CurrentCulture != null)
            {
                Thread.CurrentThread.CurrentCulture = threadContext.CurrentCulture;
            }

            if (threadContext.CurrentUICulture != null)
            {
                Thread.CurrentThread.CurrentUICulture = threadContext.CurrentUICulture;
            }
        }
    }
}