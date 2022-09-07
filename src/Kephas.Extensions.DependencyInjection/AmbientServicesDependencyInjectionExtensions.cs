// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AmbientServicesDependencyInjectionExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the dependency injection ambient services builder extensions class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Kephas.Application;
    using Kephas.Extensions.DependencyInjection.Hosting;
    using Kephas.Injection;
    using Kephas.Injection.AttributedModel;
    using Kephas.Injection.Builder;
    using Kephas.Model.AttributedModel;
    using Kephas.Reflection;
    using Kephas.Runtime;
    using Kephas.Services;

    /// <summary>
    /// Microsoft.Extensions.DependencyInjection related ambient services extensions.
    /// </summary>
    public static class AmbientServicesDependencyInjectionExtensions
    {
        /// <summary>
        /// Sets the injector to the ambient services.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="builderOptions">The injector builder configuration.</param>
        /// <returns>The provided ambient services builder.</returns>
        public static IAmbientServices BuildWithDependencyInjection(this IAmbientServices ambientServices, Action<DependencyInjectionInjectorBuilder>? builderOptions = null)
        {
            ambientServices = ambientServices ?? throw new ArgumentNullException(nameof(ambientServices));

            var injectorBuilder = new DependencyInjectionInjectorBuilder(new InjectionBuildContext(ambientServices));

            builderOptions?.Invoke(injectorBuilder);

            var container = injectorBuilder.Build();
            return ambientServices.WithInjector(container);
        }

        /// <summary>
        /// Gets the services configurators.
        /// </summary>
        /// <param name="appRuntime">The application runtime.</param>
        /// <returns>An enumeration of <see cref="IServicesConfigurator"/>.</returns>
        public static IEnumerable<IServicesConfigurator> GetServicesConfigurators(this IAppRuntime appRuntime)
        {
            var appAssemblies = appRuntime.GetAppAssemblies();
            var configuratorTypes = ServiceHelper.GetAppServiceInfosProviders(appAssemblies)
                .SelectMany(p => p.GetAppServices(null))
                .Where(t => t.ContractDeclarationType == typeof(IServicesConfigurator))
                .Select(t => t.ServiceType)
                .ToList();
            var orderedConfiguratorTypes = configuratorTypes
                .Select(t => new Lazy<IServicesConfigurator, AppServiceMetadata>(
                    () => (IServicesConfigurator)Activator.CreateInstance(t),
                    new AppServiceMetadata(ServiceHelper.GetServiceMetadata(t, typeof(IServicesConfigurator)))))
                .Order();
            return orderedConfiguratorTypes.Select(f => f.Value);
        }
    }
}