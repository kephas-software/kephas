// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NpgsqlFeatureManager.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the Npgsql feature manager class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Npgsql.Application
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Logging;
    using Kephas.Npgsql.Logging;

    using global::Npgsql.Logging;

    /// <summary>
    /// The Npgsql application lifecycle behavior.
    /// </summary>
    public class NpgsqlAppLifecycleBehavior : AppLifecycleBehaviorBase
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
            NpgsqlLogManager.Provider = new NpgsqlLoggingProviderAdapter(appContext.CompositionContext.GetExport<ILogManager>());

            return Task.FromResult(0);
        }
    }
}