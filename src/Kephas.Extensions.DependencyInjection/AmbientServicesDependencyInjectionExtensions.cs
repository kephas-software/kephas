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

    using Kephas.Composition;
    using Kephas.Composition.AttributedModel;
    using Kephas.Composition.ExportFactories;
    using Kephas.Composition.Hosting;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Extensions.DependencyInjection.Hosting;
    using Kephas.Model.AttributedModel;
    using Kephas.Reflection;
    using Kephas.Runtime;
    using Kephas.Services;
    using Kephas.Services.Composition;

    /// <summary>
    /// Microsoft.Extensions.DependencyInjection related ambient services extensions.
    /// </summary>
    public static class AmbientServicesDependencyInjectionExtensions
    {
        /// <summary>
        /// Sets the composition container to the ambient services.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="containerBuilderConfig">The container builder configuration.</param>
        /// <returns>The provided ambient services builder.</returns>
        public static IAmbientServices WithDependencyInjectionCompositionContainer(this IAmbientServices ambientServices, Action<DependencyInjectionCompositionContainerBuilder> containerBuilderConfig = null)
        {
            Requires.NotNull(ambientServices, nameof(ambientServices));

            var containerBuilder = new DependencyInjectionCompositionContainerBuilder(new CompositionRegistrationContext(ambientServices));

            containerBuilderConfig?.Invoke(containerBuilder);

            var container = containerBuilder.CreateContainer();
            return ambientServices.WithCompositionContainer(container);
        }

        /// <summary>
        /// Gets the services configurators.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <returns>An enumeration of <see cref="IServicesConfigurator"/>.</returns>
        public static IEnumerable<IServicesConfigurator> GetServicesConfigurators(this IAmbientServices ambientServices)
        {
            AppServiceMetadata GetAppServiceMetadata(IRuntimeTypeInfo t)
            {
                var overridePriority = t.GetAttribute<OverridePriorityAttribute>()?.Value ?? 0;
                var processingPriority = t.GetAttribute<ProcessingPriorityAttribute>()?.Value ?? 0;
                var isOverride = t.GetAttribute<OverrideAttribute>() != null;
                var serviceName = t.GetAttribute<ServiceNameAttribute>()?.Value;
                return new AppServiceMetadata(processingPriority, overridePriority, serviceName, isOverride)
                {
                    ServiceInstanceType = t.Type,
                };
            }

            var configuratorTypes = ambientServices!.AppRuntime.GetAppAssemblies()
                .SelectMany(a => DefaultTypeLoader.Instance.GetExportedTypes(a)
                    .Where(t => typeof(IServicesConfigurator).IsAssignableFrom(t)
                                && t.IsClass
                                && !t.IsAbstract
                                && t.GetCustomAttribute<ExcludeFromCompositionAttribute>() == null))
                .Select(t => ambientServices.TypeRegistry.GetTypeInfo(t));
            var configurators = configuratorTypes
                .Select(t => new ExportFactory<IServicesConfigurator, AppServiceMetadata>(
                    () => (IServicesConfigurator) t.CreateInstance(),
                    GetAppServiceMetadata(t)))
                .Order()
                .Select(f => f.CreateExportedValue());
            return configurators;
        }
    }
}