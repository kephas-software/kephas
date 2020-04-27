// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MefCompositionTestBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Base class for tests using composition.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Testing.Composition
{
    using System;
    using System.Collections.Generic;
    using System.Composition.Hosting;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;
    using System.Text;
    using Kephas.Application;
    using Kephas.Composition;
    using Kephas.Composition.Hosting;
    using Kephas.Composition.Mef.ExportProviders;
    using Kephas.Composition.Mef.Hosting;
    using Kephas.Diagnostics.Logging;
    using Kephas.Logging;
    using Kephas.Reflection;
    using Kephas.Services;

    /// <summary>
    /// Base class for tests using composition.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public class MefCompositionTestBase : TestBase
    {
        public virtual ContainerConfiguration WithEmptyConfiguration()
        {
            var configuration = new ContainerConfiguration();
            return configuration;
        }

        public virtual ContainerConfiguration WithExportProviders(ContainerConfiguration configuration)
        {
            configuration.WithProvider(new ExportFactoryExportDescriptorProvider());
            configuration.WithProvider(new ExportFactoryWithMetadataExportDescriptorProvider());
            return configuration;
        }

        public virtual SystemCompositionContainerBuilder WithContainerBuilder(IAmbientServices ambientServices = null, ILogManager logManager = null, IAppRuntime appRuntime = null)
        {
            var log = new StringBuilder();
            logManager = logManager ?? new DebugLogManager((logger, level, message, ex) => log.AppendLine($"[{logger}] [{level}] {message}{ex}"));
            appRuntime = appRuntime ?? this.CreateDefaultAppRuntime(logManager);

            ambientServices = ambientServices ?? new AmbientServices();
            ambientServices
                .Register(logManager)
                .WithAppRuntime(appRuntime)
                .Register(log);
            return new SystemCompositionContainerBuilder(new CompositionRegistrationContext(ambientServices));
        }

        public ICompositionContext CreateContainer(params Assembly[] assemblies)
        {
            return CreateContainer(assemblies: (IEnumerable<Assembly>)assemblies);
        }

        public virtual ICompositionContext CreateContainer(
            IAmbientServices ambientServices = null,
            IEnumerable<Assembly> assemblies = null,
            IEnumerable<Type> parts = null,
            Action<SystemCompositionContainerBuilder> config = null)
        {
            ambientServices = ambientServices ?? new AmbientServices();
            var containerBuilder = WithContainerBuilder(ambientServices)
                    .WithAssemblies(GetDefaultConventionAssemblies())
                    .WithAssemblies(assemblies ?? new Assembly[0])
                    .WithParts(parts ?? new Type[0]);

            config?.Invoke(containerBuilder);

            var container = containerBuilder.CreateContainer();
            ambientServices.Register(container);
            return container;
        }

        public ICompositionContext CreateContainerWithBuilder(params Type[] types)
        {
            var configuration = WithEmptyConfiguration().WithParts(types);
            return WithContainerBuilder()
                .WithAssembly(typeof(ICompositionContext).GetTypeInfo().Assembly)
                .WithConfiguration(configuration)
                .CreateContainer();
        }

        public ICompositionContext CreateContainerWithBuilder(IAmbientServices ambientServices, params Type[] types)
        {
            var configuration = WithEmptyConfiguration().WithParts(types);
            return WithContainerBuilder(ambientServices)
                .WithAssembly(typeof(ICompositionContext).GetTypeInfo().Assembly)
                .WithConfiguration(configuration)
                .CreateContainer();
        }

        public virtual IEnumerable<Assembly> GetDefaultConventionAssemblies()
        {
            return new List<Assembly>
                       {
                           typeof(ICompositionContext).GetTypeInfo().Assembly,     /* Kephas.Core*/
                           typeof(SystemCompositionContainer).GetTypeInfo().Assembly, /* Kephas.Composition.Mef */
                       };
        }

        public virtual IEnumerable<Type> GetDefaultParts()
        {
            return new List<Type>();
        }
    }
}