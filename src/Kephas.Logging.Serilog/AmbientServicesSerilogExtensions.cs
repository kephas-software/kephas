// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AmbientServicesSerilogExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Extension methods for the AmbientServicesBuilder.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Serilog.Core;

namespace Kephas.Logging.Serilog
{
    using global::Serilog;

    using Kephas.Diagnostics.Contracts;

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
        /// <param name="levelSwitch">Optional.The logging level switch.</param>
        /// <param name="replaceDefault">Optional. True to replace <see cref="LoggingHelper.DefaultLogManager"/>.</param>
        /// <returns>
        /// This <paramref name="ambientServices"/>.
        /// </returns>
        public static IAmbientServices WithSerilogManager(this IAmbientServices ambientServices, LoggerConfiguration? configuration = null, LoggingLevelSwitch? levelSwitch = null, bool replaceDefault = true)
        {
            Requires.NotNull(ambientServices, nameof(ambientServices));

            return ambientServices.WithLogManager(new SerilogManager(configuration, levelSwitch), replaceDefault);
        }
    }
}