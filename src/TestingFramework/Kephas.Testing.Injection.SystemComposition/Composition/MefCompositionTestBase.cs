// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MefCompositionTestBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Base class for tests using composition.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Injection;
using Kephas.Injection.Hosting;

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
    using Kephas.Composition.Mef.ExportProviders;
    using Kephas.Composition.Mef.Hosting;
    using Kephas.Diagnostics.Logging;
    using Kephas.Logging;
    using Kephas.Reflection;
    using Kephas.Runtime;
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

        /// <summary>
        /// Creates a container builder for further configuration.
        /// </summary>
        /// <param name="ambientServices">Optional. The ambient services. If not provided, a new instance
        ///                               will be created as linked to the newly created container.</param>
        /// <param name="logManager">Optional. Manager for log.</param>
        /// <param name="appRuntime">Optional. The application runtime.</param>
        /// <returns>
        /// A SystemInjectorBuilder.
        /// </returns>
        public virtual SystemInjectorBuilder WithContainerBuilder(
            IAmbientServices? ambientServices = null,
            ILogManager? logManager = null,
            IAppRuntime? appRuntime = null)
        {
            var log = new StringBuilder();
            logManager ??= new DebugLogManager(log);
            appRuntime ??= this.CreateDefaultAppRuntime(logManager);

            ambientServices ??= new AmbientServices(typeRegistry: new RuntimeTypeRegistry());
            ambientServices
                .Register(logManager)
                .WithAppRuntime(appRuntime)
                .Register(log);
            return new SystemInjectorBuilder(new InjectionRegistrationContext(ambientServices));
        }

        public IInjector CreateContainer(params Assembly[] assemblies)
        {
            return this.CreateContainer(assemblies: (IEnumerable<Assembly>)assemblies);
        }

        /// <summary>
        /// Creates a container for the provided convention assemblies and parts.
        /// </summary>
        /// <param name="ambientServices">Optional. The ambient services. If not provided, a new instance will be created as linked to the newly created container.</param>
        /// <param name="assemblies">Optional. A variable-length parameters list containing assemblies.</param>
        /// <param name="parts">Optional. The parts.</param>
        /// <param name="config">Optional. The configuration.</param>
        /// <param name="logManager">Optional. The log manager.</param>
        /// <param name="appRuntime">Optional. The application runtime.</param>
        /// <returns>
        /// The new container.
        /// </returns>
        public virtual IInjector CreateContainer(
            IAmbientServices? ambientServices = null,
            IEnumerable<Assembly>? assemblies = null,
            IEnumerable<Type>? parts = null,
            Action<SystemInjectorBuilder>? config = null,
            ILogManager? logManager = null,
            IAppRuntime? appRuntime = null)
        {
            ambientServices ??= new AmbientServices(typeRegistry: new RuntimeTypeRegistry());
            var containerBuilder = this.WithContainerBuilder(ambientServices, logManager, appRuntime)
                    .WithAssemblies(this.GetDefaultConventionAssemblies())
                    .WithAssemblies(assemblies ?? new Assembly[0])
                    .WithParts(parts ?? new Type[0]);

            config?.Invoke(containerBuilder);

            var container = containerBuilder.CreateInjector();
            ambientServices.Register(container);
            return container;
        }

        public IInjector CreateContainerWithBuilder(params Type[] types)
        {
            var configuration = WithEmptyConfiguration().WithParts(types);
            return WithContainerBuilder()
                .WithAssembly(typeof(IInjector).GetTypeInfo().Assembly)
                .WithConfiguration(configuration)
                .CreateInjector();
        }

        public IInjector CreateContainerWithBuilder(IAmbientServices ambientServices, params Type[] types)
        {
            var configuration = WithEmptyConfiguration().WithParts(types);
            return WithContainerBuilder(ambientServices)
                .WithAssembly(typeof(IInjector).GetTypeInfo().Assembly)
                .WithConfiguration(configuration)
                .CreateInjector();
        }

        public virtual IEnumerable<Assembly> GetDefaultConventionAssemblies()
        {
            return new List<Assembly>
                       {
                           typeof(IInjector).GetTypeInfo().Assembly,     /* Kephas.Core*/
                           typeof(SystemCompositionContainer).GetTypeInfo().Assembly, /* Kephas.Composition.Mef */
                       };
        }

        public virtual IEnumerable<Type> GetDefaultParts()
        {
            return new List<Type>();
        }
    }
}