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
    using System.Reflection;
    using System.Text;
    using Kephas.Application;
    using Kephas.Composition;
    using Kephas.Composition.Hosting;
    using Kephas.Composition.Lite.Hosting;
    using Kephas.Diagnostics.Logging;
    using Kephas.Logging;
    using Kephas.Reflection;
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
        /// A LiteCompositionContainerBuilder.
        /// </returns>
        public virtual LiteCompositionContainerBuilder WithContainerBuilder(IAmbientServices ambientServices = null, ILogManager logManager = null, IAppRuntime appRuntime = null)
        {
            var log = new StringBuilder();
            logManager = logManager ?? ambientServices?.LogManager ?? new DebugLogManager((logger, level, message, ex) => log.AppendLine($"[{logger}] [{level}] {message}{ex}"));
            appRuntime = appRuntime ?? ambientServices?.AppRuntime ?? new StaticAppRuntime(
                             logManager: logManager,
                             defaultAssemblyFilter: this.IsNotTestAssembly);

            ambientServices = ambientServices ?? new AmbientServices();
            ambientServices
                .Register(logManager)
                .Register(appRuntime)
                .Register(log);
            return new LiteCompositionContainerBuilder(new CompositionRegistrationContext(ambientServices));
        }

        /// <summary>
        /// Creates a container for the provided convention assemblies.
        /// </summary>
        /// <param name="assemblies">A variable-length parameters list containing assemblies.</param>
        /// <returns>
        /// The new container.
        /// </returns>
        public ICompositionContext CreateContainer(params Assembly[] assemblies)
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
        /// <returns>
        /// The new container.
        /// </returns>
        public virtual ICompositionContext CreateContainer(
            IAmbientServices ambientServices = null,
            IEnumerable<Assembly> assemblies = null,
            IEnumerable<Type> parts = null,
            Action<LiteCompositionContainerBuilder> config = null)
        {
            ambientServices = ambientServices ?? new AmbientServices();
            var containerBuilder = this.WithContainerBuilder(ambientServices)
                    .WithAssemblies(this.GetDefaultConventionAssemblies())
                    .WithAssemblies(assemblies ?? new Assembly[0])
                    .WithParts(parts ?? new Type[0]);

            config?.Invoke(containerBuilder);

            var container = containerBuilder.CreateContainer();
            ambientServices.Register(container);
            return container;
        }

        public ICompositionContext CreateContainerWithBuilder(Action<LiteCompositionContainerBuilder> config = null)
        {
            var builder = this.WithContainerBuilder()
                .WithAssembly(typeof(ICompositionContext).GetTypeInfo().Assembly);
            config?.Invoke(builder);
            return builder.CreateContainer();
        }

        public ICompositionContext CreateContainerWithBuilder(IAmbientServices ambientServices, params Type[] types)
        {
            return this.WithContainerBuilder(ambientServices)
                .WithAssembly(typeof(ICompositionContext).GetTypeInfo().Assembly)
                .WithParts(types)
                .CreateContainer();
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
                           typeof(ICompositionContext).GetTypeInfo().Assembly,     /* Kephas.Core*/
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