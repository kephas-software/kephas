// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServerThreadContextAppInitializer.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   A server application initializer.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Services;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// A server application initializer.
    /// </summary>
    [ProcessingPriority(Priority.Highest)]
    public class ServerThreadContextAppInitializer : AppInitializerBase
    {
        /// <summary>
        /// Initializes the application asynchronously.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        protected override Task InitializeCoreAsync(IAppContext appContext, CancellationToken cancellationToken)
        {
            new ServerThreadContextBuilder(appContext.AmbientServices)
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