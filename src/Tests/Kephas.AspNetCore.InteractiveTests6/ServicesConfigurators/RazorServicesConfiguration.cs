// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RazorServicesConfiguration.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore.InteractiveTests6.ServicesConfigurators
{
    using Kephas.Serialization.Json;
    using Kephas.Services;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Configuration for Razor services.
    /// </summary>
    [OverridePriority(Priority.High)]
    public class RazorServicesConfiguration : IServicesConfigurator
    {
        /// <summary>
        /// Configure the services.
        /// </summary>
        /// <param name="services">The services to configure.</param>
        /// <param name="ambientServices">The ambient services.</param>
        public void ConfigureServices(IServiceCollection services, IAmbientServices ambientServices)
        {
            services
                .AddControllersWithViews()
                .AddNewtonsoftJson(
                    options =>
                    {
                        var jsonSettingsProvider = ambientServices.Injector
                            .Resolve<IJsonSerializerSettingsProvider>();
                        jsonSettingsProvider.ConfigureJsonSerializerSettings(options.SerializerSettings);
                    });
        }
    }
}
