// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServerAppInitializer.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   A server application initializer.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application
{
    using System.Globalization;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Threading.Tasks;

    /// <summary>
    /// A server application initializer.
    /// </summary>
    public class ServerAppInitializer : IAppInitializer
    {
        /// <summary>
        /// Initializes the application asynchronously.
        /// </summary>
        /// <param name="appContext">       Context for the application.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        public Task InitializeAsync(IAppContext appContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            new ServerThreadContextBuilder(appContext.AmbientServices)
                .WithStoreAction(StoreThreadCulture)
                .WithRestoreAction(RestoreThreadCulture);

            return CompletedTask.Value;
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
        /// Restores thread culture.
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