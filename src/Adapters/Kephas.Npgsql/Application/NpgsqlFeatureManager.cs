// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NpgsqlFeatureManager.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
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
    using Kephas.Npgsql.Logging;
    using Kephas.Services;

    using global::Npgsql.Logging;

    /// <summary>
    /// A Npgsql feature manager.
    /// </summary>
    [ProcessingPriority(Priority.Highest)]
    [FeatureInfo(FeatureName, isRequired: true)]
    public class NpgsqlFeatureManager : FeatureManagerBase
    {
        /// <summary>
        /// Name of the feature.
        /// </summary>
        public const string FeatureName = "npgsql";

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
            NpgsqlLogManager.Provider = new NpgsqlLoggingProviderAdapter(appContext.AmbientServices.LogManager);

            return Task.FromResult(0);
        }
    }
}