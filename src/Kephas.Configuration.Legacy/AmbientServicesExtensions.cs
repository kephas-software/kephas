// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AmbientServicesExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the ambient services extensions class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas
{
    using Kephas.Application.Configuration;
    using Kephas.Diagnostics.Contracts;

    /// <summary>
    /// An ambient services extensions.
    /// </summary>
    public static class AmbientServicesExtensions
    {
        /// <summary>
        /// Gets the application configuration.
        /// </summary>
        /// <param name="ambientServices">The ambientServices to act on.</param>
        /// <returns>
        /// An IAppConfiguration.
        /// </returns>
        public static IAppConfiguration AppConfiguration(this IAmbientServices ambientServices) =>
            ambientServices.GetService<IAppConfiguration>();

        /// <summary>
        /// Sets the application configuration to the ambient services.
        /// </summary>
        /// <param name="builder">The builder to act on.</param>
        /// <param name="appConfiguration">The application configuration.</param>
        /// <returns>
        /// The ambient services builder.
        /// </returns>
        public static AmbientServicesBuilder WithAppConfiguration(this AmbientServicesBuilder builder, IAppConfiguration appConfiguration)
        {
            Requires.NotNull(appConfiguration, nameof(appConfiguration));

            builder.AmbientServices.Register(appConfiguration);

            return builder;
        }
    }
}