// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Log4NetServicesExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the log 4 net ambient services builder extensions class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas
{
    using System;

    using Kephas.Logging;
    using Kephas.Logging.Log4Net;
    using Kephas.Services.Builder;

    /// <summary>
    /// Extension methods for the <see cref="IAppServiceCollectionBuilder"/>.
    /// </summary>
    public static class Log4NetServicesExtensions
    {
        /// <summary>
        /// Sets the NLog log manager to The application services.
        /// </summary>
        /// <param name="servicesBuilder">The services builder.</param>
        /// <param name="replaceDefault">Optional. True to replace <see cref="LoggingHelper.DefaultLogManager"/>.</param>
        /// <returns>
        /// This <paramref name="servicesBuilder"/>.
        /// </returns>
        public static IAppServiceCollectionBuilder WithLog4NetManager(this IAppServiceCollectionBuilder servicesBuilder, bool replaceDefault = true)
        {
            servicesBuilder = servicesBuilder ?? throw new ArgumentNullException(nameof(servicesBuilder));

            return servicesBuilder.WithLogManager(new Log4NetLogManager(), replaceDefault);
        }
    }
}