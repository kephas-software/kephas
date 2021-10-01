// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AmbientServicesSerilogExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Extension methods for the AmbientServicesBuilder.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas
{
    using global::Serilog;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Logging;
    using Kephas.Logging.Serilog;
    using Serilog.Events;

    /// <summary>
    /// Extension methods for the <see cref="IAmbientServices"/>.
    /// </summary>
    public static class AmbientServicesSerilogExtensions
    {
        /// <summary>
        /// Sets the Serilog log manager to the ambient services.
        /// </summary>
        /// <param name="ambientServices">The ambient services builder.</param>
        /// <param name="configuration">Optional. The logger configuration.</param>
        /// <param name="minimumLevel">Optional. The minimum logging level.</param>
        /// <param name="dynamicMinimumLevel">
        /// Optional. Indicates whether to allow changing the minimum level at runtime
        /// (true) or leave it controlled by the configuration (false). If not provided (null),
        /// a dynamic minimum level will be used only if no configuration is provided.
        /// </param>
        /// <param name="replaceDefault">Optional. True to replace <see cref="LoggingHelper.DefaultLogManager"/>.</param>
        /// <returns>
        /// This <paramref name="ambientServices"/>.
        /// </returns>
        public static IAmbientServices WithSerilogManager(this IAmbientServices ambientServices, LoggerConfiguration? configuration = null, LogEventLevel? minimumLevel = null, bool? dynamicMinimumLevel = null, bool replaceDefault = true)
        {
            ambientServices = ambientServices ?? throw new ArgumentNullException(nameof(ambientServices));

            return ambientServices.WithLogManager(new SerilogManager(configuration, minimumLevel, dynamicMinimumLevel), replaceDefault);
        }
    }
}