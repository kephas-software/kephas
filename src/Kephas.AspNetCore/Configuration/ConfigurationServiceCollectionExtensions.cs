// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigurationServiceCollectionExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the logging service collection extensions class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore.Configuration
{
    using Kephas.Configuration;
    using Kephas.Diagnostics.Contracts;

    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;

    /// <summary>
    /// Configuration extensions.
    /// </summary>
    public static class ConfigurationServiceCollectionExtensions
    {
        /// <summary>
        /// Configures the options extensions.
        /// </summary>
        /// <param name="ambientServices">The ambient services to act on.</param>
        /// <returns>
        /// The provided ambient services.
        /// </returns>
        public static IAmbientServices ConfigureOptionsExtensions(this IAmbientServices ambientServices)
        {
            Requires.NotNull(ambientServices, nameof(ambientServices));

            var serviceCollection = ambientServices.GetService<IServiceCollection>();

            serviceCollection.Replace(ServiceDescriptor.Scoped(typeof(Microsoft.Extensions.Options.IOptionsSnapshot<>), typeof(OptionsSnapshot<>)));

            return ambientServices;
        }

        /// <summary>
        /// Uses the extensions configuration.
        /// </summary>
        /// <param name="ambientServices">The ambient services to act on.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns>
        /// The provided ambient services.
        /// </returns>
        public static IAmbientServices UseConfiguration(this IAmbientServices ambientServices, IConfiguration configuration)
        {
            Requires.NotNull(ambientServices, nameof(ambientServices));

            ambientServices.Register<IConfigurationStore>(new AspNetConfigurationStore(configuration));

            return ambientServices;
        }
    }
}