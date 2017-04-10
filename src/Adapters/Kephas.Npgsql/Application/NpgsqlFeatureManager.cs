// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NpgsqlFeatureManager.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
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
    using Kephas.Application.AttributedModel;
    using Kephas.Npgsql.Logging;
    using Kephas.Services;

    using global::Npgsql.Logging;

    /// <summary>
    /// A Npgsql feature manager.
    /// </summary>
    [ProcessingPriority(Priority.Highest)]
    [FeatureInfo(NpgsqlFeature.Driver)]
    public class NpgsqlFeatureManager : FeatureManagerBase
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
            NpgsqlLogManager.Provider = new NpgsqlLoggingProviderAdapter(appContext.AmbientServices);

            return Task.FromResult(0);
        }
    }
}