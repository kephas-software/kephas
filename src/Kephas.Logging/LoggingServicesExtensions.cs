// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoggingServicesExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas
{
    using System;
    using System.Text;

    using Kephas.Diagnostics.Logging;
    using Kephas.Logging;
    using Kephas.Services.Builder;

    /// <summary>
    /// Logging extension methods for <see cref="IAppServiceCollectionBuilder"/>.
    /// </summary>
    public static class LoggingServicesExtensions
    {
        /// <summary>
        /// Sets the log manager to the ambient services.
        /// </summary>
        /// <param name="servicesBuilder">The services builder.</param>
        /// <param name="logManager">The log manager.</param>
        /// <param name="replaceDefault">Optional. True to replace the <see cref="LoggingHelper.DefaultLogManager"/>.</param>
        /// <returns>
        /// This <paramref name="servicesBuilder"/>.
        /// </returns>
        public static IAppServiceCollectionBuilder WithLogManager(this IAppServiceCollectionBuilder servicesBuilder, ILogManager logManager, bool replaceDefault = true)
        {
            servicesBuilder = servicesBuilder ?? throw new ArgumentNullException(nameof(servicesBuilder));
            logManager = logManager ?? throw new ArgumentNullException(nameof(logManager));

            if (replaceDefault)
            {
                LoggingHelper.DefaultLogManager = logManager;
            }

            servicesBuilder.AmbientServices.Add<ILogManager>(logManager);

            return servicesBuilder;
        }

        /// <summary>
        /// Sets the debug log manager to the ambient services.
        /// </summary>
        /// <param name="servicesBuilder">The services builder.</param>
        /// <param name="logCallback">Optional. The log callback.</param>
        /// <param name="replaceDefault">Optional. True to replace <see cref="LoggingHelper.DefaultLogManager"/>.</param>
        /// <returns>
        /// This <paramref name="servicesBuilder"/>.
        /// </returns>
        public static IAppServiceCollectionBuilder WithDebugLogManager(this IAppServiceCollectionBuilder servicesBuilder, Action<string, string, object?, object?[], Exception?>? logCallback = null, bool replaceDefault = true)
        {
            servicesBuilder = servicesBuilder ?? throw new ArgumentNullException(nameof(servicesBuilder));

            return servicesBuilder.WithLogManager(new DebugLogManager(logCallback), replaceDefault);
        }

        /// <summary>
        /// Sets the debug log manager to the ambient services.
        /// </summary>
        /// <param name="servicesBuilder">The services builder.</param>
        /// <param name="stringBuilder">The string builder.</param>
        /// <param name="replaceDefault">Optional. True to replace <see cref="LoggingHelper.DefaultLogManager"/>.</param>
        /// <returns>
        /// This <paramref name="servicesBuilder"/>.
        /// </returns>
        public static IAppServiceCollectionBuilder WithDebugLogManager(this IAppServiceCollectionBuilder servicesBuilder, StringBuilder stringBuilder, bool replaceDefault = true)
        {
            servicesBuilder = servicesBuilder ?? throw new ArgumentNullException(nameof(servicesBuilder));

            return servicesBuilder.WithLogManager(new DebugLogManager(stringBuilder), replaceDefault);
        }
    }
}