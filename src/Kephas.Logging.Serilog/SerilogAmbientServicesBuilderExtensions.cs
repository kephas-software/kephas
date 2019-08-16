// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerilogAmbientServicesBuilderExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Extension methods for the AmbientServicesBuilder.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Logging.Serilog
{
    using global::Serilog;

    using Kephas.Diagnostics.Contracts;

    /// <summary>
    /// Extension methods for the <see cref="AmbientServicesBuilder"/>.
    /// </summary>
    public static class SerilogAmbientServicesBuilderExtensions
    {
        /// <summary>
        /// Sets the Serilog log manager to the ambient services.
        /// </summary>
        /// <param name="ambientServicesBuilder">The ambient services builder.</param>
        /// <param name="configuration">Optional. The logger configuration.</param>
        /// <returns>
        /// The provided ambient services builder.
        /// </returns>
        public static AmbientServicesBuilder WithSerilogManager(this AmbientServicesBuilder ambientServicesBuilder, LoggerConfiguration configuration = null)
        {
            Requires.NotNull(ambientServicesBuilder, nameof(ambientServicesBuilder));

            return ambientServicesBuilder.WithLogManager(new SerilogManager(configuration));
        }
    }
}