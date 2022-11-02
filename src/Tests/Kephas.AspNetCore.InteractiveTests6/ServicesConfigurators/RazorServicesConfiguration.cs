// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RazorServicesConfiguration.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore.InteractiveTests6.ServicesConfigurators
{
    using Kephas.Extensions.DependencyInjection;
    using Kephas.Serialization.Json;
    using Kephas.Services;
    using Kephas.Services.Builder;
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
        /// <param name="context">The host builder context.</param>
        /// <param name="services">The service collection.</param>
        /// <param name="servicesBuilder">The services builder.</param>
        public void ConfigureServices(HostBuilderContext context, IServiceCollection services, IAppServiceCollectionBuilder servicesBuilder)
        {
            services
                .AddControllersWithViews()
                .AddNewtonsoftJson(
                    options =>
                    {
                        var container = services.BuildServiceProvider();
                        var jsonSettingsProvider = container.GetRequiredService<IJsonSerializerSettingsProvider>();
                        jsonSettingsProvider.ConfigureJsonSerializerSettings(options.SerializerSettings);
                    });
        }
    }
}
