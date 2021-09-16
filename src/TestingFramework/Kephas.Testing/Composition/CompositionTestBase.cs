// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompositionTestBase.cs" company="Kephas Software SRL">
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
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using Kephas.Application;
    using Kephas.Diagnostics.Logging;
    using Kephas.Injection;
    using Kephas.Injection.Hosting;
    using Kephas.Injection.Lite.Hosting;
    using Kephas.Logging;
    using Kephas.Runtime;
    using Kephas.Testing;

    /// <summary>
    /// Base class for tests using composition.
    /// </summary>
    /// <content>
    /// It includes:
    /// * Creating composition container in different configurations.
    /// </content>
    public class CompositionTestBase : TestBase
    {
        /// <summary>
        /// Creates a container builder for further configuration.
        /// </summary>
        /// <param name="ambientServices">Optional. The ambient services. If not provided, a new instance
        ///                               will be created as linked to the newly created container.</param>
        /// <param name="logManager">Optional. Manager for log.</param>
        /// <param name="appRuntime">Optional. The application runtime.</param>
        /// <returns>
        /// A LiteInjectorBuilder.
        /// </returns>
        public virtual LiteInjectorBuilder WithContainerBuilder(
            IAmbientServices? ambientServices = null,
            ILogManager? logManager = null,
            IAppRuntime? appRuntime = null)
        {
            var log = new StringBuilder();
            logManager ??= ambientServices?.LogManager ?? new DebugLogManager(log);
            appRuntime ??= this.CreateDefaultAppRuntime(logManager);

            ambientServices = (ambientServices ?? new AmbientServices(typeRegistry: new RuntimeTypeRegistry()))
                .Register(logManager)
                .WithAppRuntime(appRuntime)
                .Register(log);
            return new LiteInjectorBuilder(new InjectionRegistrationContext(ambientServices));
        }

        /// <summary>
        /// Creates a container for the provided convention assemblies.
        /// </summary>
        /// <param name="assemblies">A variable-length parameters list containing assemblies.</param>
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
            Action<LiteInjectorBuilder>? config = null,
            ILogManager? logManager = null,
            IAppRuntime? appRuntime = null)
        {
            ambientServices ??= new AmbientServices(typeRegistry: new RuntimeTypeRegistry());
            var allParts = this.GetDefaultParts().ToList();
            allParts.AddRange(parts ?? Type.EmptyTypes);
            var containerBuilder = this.WithContainerBuilder(ambientServices, logManager, appRuntime)
                    .WithAssemblies(this.GetDefaultConventionAssemblies())
                    .WithAssemblies(assemblies ?? Array.Empty<Assembly>())
                    .WithParts(allParts);

            config?.Invoke(containerBuilder);

            var container = containerBuilder.Build();
            ambientServices.Register(container);
            return container;
        }

        public IInjector CreateContainerWithBuilder(Action<LiteInjectorBuilder>? config = null)
        {
            var builder = this.WithContainerBuilder()
                .WithAssembly(typeof(IInjector).GetTypeInfo().Assembly);
            config?.Invoke(builder);
            return builder.Build();
        }

        public IInjector CreateContainerWithBuilder(IAmbientServices ambientServices, params Type[] types)
        {
            return this.WithContainerBuilder(ambientServices)
                .WithAssembly(typeof(IInjector).GetTypeInfo().Assembly)
                .WithParts(types)
                .Build();
        }

        /// <summary>
        /// Gets the default convention assemblies to be considered when building the container. By default it includes Kephas.Core.
        /// </summary>
        /// <returns>
        /// An enumerator that allows foreach to be used to process the default convention assemblies in
        /// this collection.
        /// </returns>
        public virtual IEnumerable<Assembly> GetDefaultConventionAssemblies()
        {
            return new List<Assembly>
                       {
                           typeof(IInjector).GetTypeInfo().Assembly,     /* Kephas.Core*/
                       };
        }

        /// <summary>
        /// Gets the default parts to be included in the container.
        /// </summary>
        /// <returns>
        /// An enumerator that allows foreach to be used to process the default parts in this collection.
        /// </returns>
        public virtual IEnumerable<Type> GetDefaultParts()
        {
            return new List<Type>();
        }
    }
}