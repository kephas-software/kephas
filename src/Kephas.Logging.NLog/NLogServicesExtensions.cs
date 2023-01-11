// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NLogServicesExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas
{
    using System;
    using global::NLog.Config;

    using Kephas.Logging;
    using Kephas.Logging.NLog;
    using Kephas.Services.Builder;

    /// <summary>
    /// Extension methods for the <see cref="IAppServiceCollectionBuilder"/>.
    /// </summary>
    public static class NLogServicesExtensions
    {
        /// <summary>
        /// Sets the NLog log manager to The application services.
        /// </summary>
        /// <param name="servicesBuilder">The services builder.</param>
        /// <param name="configuration">Optional. The logging configuration.</param>
        /// <param name="replaceDefault">Optional. True to replace <see cref="LoggingHelper.DefaultLogManager"/>.</param>
        /// <returns>
        /// This <paramref name="servicesBuilder"/>.
        /// </returns>
        public static IAppServiceCollectionBuilder WithNLogManager(this IAppServiceCollectionBuilder servicesBuilder, LoggingConfiguration? configuration = null, bool replaceDefault = true)
        {
            servicesBuilder = servicesBuilder ?? throw new ArgumentNullException(nameof(servicesBuilder));

            return servicesBuilder.WithLogManager(new NLogManager(configuration), replaceDefault);
        }
    }
}