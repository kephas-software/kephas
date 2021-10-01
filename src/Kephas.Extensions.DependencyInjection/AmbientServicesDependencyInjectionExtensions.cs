﻿// --------------------------------------------------------------------------------------------------------------------
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

    using Kephas.Diagnostics.Contracts;
    using Kephas.Extensions.DependencyInjection.Hosting;
    using Kephas.Injection;
    using Kephas.Injection.AttributedModel;
    using Kephas.Injection.Builder;
    using Kephas.Injection.ExportFactories;
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
        /// <param name="injectorBuilderConfig">The injector builder configuration.</param>
        /// <returns>The provided ambient services builder.</returns>
        public static IAmbientServices BuildWithDependencyInjection(this IAmbientServices ambientServices, Action<DependencyInjectionInjectorBuilder>? injectorBuilderConfig = null)
        {
            Requires.NotNull(ambientServices, nameof(ambientServices));

            var injectorBuilder = new DependencyInjectionInjectorBuilder(new InjectionBuildContext(ambientServices));

            injectorBuilderConfig?.Invoke(injectorBuilder);

            var container = injectorBuilder.Build();
            return ambientServices.WithInjector(container);
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
                var processingPriority = t.GetAttribute<ProcessingPriorityAttribute>()?.Value ?? Priority.Normal;
                var isOverride = t.GetAttribute<OverrideAttribute>() != null;
                var serviceName = t.GetAttribute<ServiceNameAttribute>()?.Value;
                return new AppServiceMetadata(processingPriority, overridePriority, serviceName, isOverride)
                {
                    ServiceType = t.Type,
                };
            }

            var configuratorTypes = ambientServices!.AppRuntime.GetAppAssemblies()
                .SelectMany(a => DefaultTypeLoader.Instance.GetExportedTypes(a)
                    .Where(t => typeof(IServicesConfigurator).IsAssignableFrom(t)
                                && t.IsClass
                                && !t.IsAbstract
                                && t.GetCustomAttribute<ExcludeFromInjectionAttribute>() == null))
                .Select(t => ambientServices.TypeRegistry.GetTypeInfo(t));
            var orderedConfiguratorTypes = configuratorTypes
                .Select(t => new ExportFactory<IServicesConfigurator, AppServiceMetadata>(
                    () => (IServicesConfigurator)t.CreateInstance(),
                    GetAppServiceMetadata(t)))
                .Order();
            return orderedConfiguratorTypes.Select(f => f.CreateExportedValue());
        }
    }
}