// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AutofacInjectionTestBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Base class for tests using composition.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Testing.Injection
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;
    using System.Text;
    using Autofac;
    using Kephas.Application;
    using Kephas.Diagnostics.Logging;
    using Kephas.Injection;
    using Kephas.Injection.Autofac.Hosting;
    using Kephas.Injection.Autofac.Metadata;
    using Kephas.Injection.Hosting;
    using Kephas.Logging;
    using Kephas.Runtime;

    /// <summary>
    /// Base class for tests using composition.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public class AutofacInjectionTestBase : TestBase
    {
        public virtual ContainerBuilder WithEmptyConfiguration()
        {
            var configuration = new ContainerBuilder();
            return configuration;
        }

        public virtual ContainerBuilder WithExportProviders(ContainerBuilder configuration)
        {
            configuration.RegisterSource(new ExportFactoryRegistrationSource());
            configuration.RegisterSource(new ExportFactoryWithMetadataRegistrationSource(RuntimeTypeRegistry.Instance));
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
        /// An AutofacInjectorBuilder.
        /// </returns>
        public virtual AutofacInjectorBuilder WithContainerBuilder(
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
            return new AutofacInjectorBuilder(new InjectionRegistrationContext(ambientServices));
        }

        /// <summary>
        /// Creates a container for the provided convention assemblies and parts.
        /// </summary>
        /// <param name="assemblies">Optional. A variable-length parameters list containing assemblies.</param>
        /// <returns>
        /// The new container.
        /// </returns>
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
            Action<AutofacInjectorBuilder>? config = null,
            ILogManager? logManager = null,
            IAppRuntime? appRuntime = null)
        {
            ambientServices ??= new AmbientServices(typeRegistry: new RuntimeTypeRegistry());
            var containerBuilder = this.WithContainerBuilder(ambientServices, logManager, appRuntime)
                    .WithAssemblies(this.GetDefaultConventionAssemblies())
                    .WithAssemblies(assemblies ?? new Assembly[0])
                    .WithParts(parts ?? new Type[0]);

            config?.Invoke(containerBuilder);

            var container = containerBuilder.Build();
            ambientServices.Register(container);
            return container;
        }

        public IInjector CreateContainerWithBuilder(Action<AutofacInjectorBuilder> config = null)
        {
            var builder = WithContainerBuilder()
                .WithAssembly(typeof(IInjector).GetTypeInfo().Assembly);
            config?.Invoke(builder);
            return builder.Build();
        }

        public IInjector CreateContainerWithBuilder(IAmbientServices ambientServices, params Type[] types)
        {
            return WithContainerBuilder(ambientServices)
                .WithAssembly(typeof(IInjector).GetTypeInfo().Assembly)
                .WithParts(types)
                .Build();
        }

        public virtual IEnumerable<Assembly> GetDefaultConventionAssemblies()
        {
            return new List<Assembly>
                       {
                           typeof(IInjector).GetTypeInfo().Assembly,     /* Kephas.Core*/
                           typeof(AutofacInjector).GetTypeInfo().Assembly, /* Kephas.Composition.Autofac */
                       };
        }

        public virtual IEnumerable<Type> GetDefaultParts()
        {
            return new List<Type>();
        }
    }
}