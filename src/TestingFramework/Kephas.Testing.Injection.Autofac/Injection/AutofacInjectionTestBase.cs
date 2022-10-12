﻿// --------------------------------------------------------------------------------------------------------------------
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
    using Kephas.Injection.Autofac;
    using Kephas.Injection.Autofac.Builder;
    using Kephas.Injection.Builder;
    using Kephas.Interaction;
    using Kephas.Logging;
    using Kephas.Serialization;

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
        public virtual AutofacInjectorBuilder WithInjectorBuilder(
            IAmbientServices? ambientServices = null,
            ILogManager? logManager = null,
            IAppRuntime? appRuntime = null)
        {
            var log = new StringBuilder();
            logManager ??= new DebugLogManager(log);
            appRuntime ??= this.CreateDefaultAppRuntime(logManager);

            ambientServices ??= this.CreateAmbientServices();
            ambientServices
                .Add(logManager)
                .WithAppRuntime(appRuntime)
                .Add(log);
            return new AutofacInjectorBuilder(new AppServiceCollectionBuilder(ambientServices));
        }

        /// <summary>
        /// Creates a container for the provided convention assemblies and parts.
        /// </summary>
        /// <param name="assemblies">Optional. A variable-length parameters list containing assemblies.</param>
        /// <returns>
        /// The new container.
        /// </returns>
        public IServiceProvider CreateInjector(params Assembly[] assemblies)
        {
            return this.CreateInjector(assemblies: (IEnumerable<Assembly>)assemblies);
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
        public virtual IServiceProvider CreateInjector(
            IAmbientServices? ambientServices = null,
            IEnumerable<Assembly>? assemblies = null,
            IEnumerable<Type>? parts = null,
            Action<IInjectorBuilder>? config = null,
            ILogManager? logManager = null,
            IAppRuntime? appRuntime = null)
        {
            ambientServices ??= this.CreateAmbientServices();
            var containerBuilder = this.WithInjectorBuilder(ambientServices, logManager, appRuntime)
                    .WithAssemblies(this.GetAssemblies())
                    .WithAssemblies(assemblies ?? Array.Empty<Assembly>())
                    .WithParts(parts ?? Type.EmptyTypes);

            config?.Invoke(containerBuilder);

            var container = containerBuilder.Build();
            ambientServices.Add(container);
            return container;
        }

        public IServiceProvider CreateInjectorWithBuilder(Action<AutofacInjectorBuilder>? config = null)
        {
            var builder = WithInjectorBuilder()
                .WithAssemblies(typeof(IServiceProvider).Assembly);
            config?.Invoke(builder);
            return builder.Build();
        }

        public IServiceProvider CreateInjectorWithBuilder(IAmbientServices ambientServices, params Type[] types)
        {
            return WithInjectorBuilder(ambientServices)
                .WithAssemblies(typeof(IServiceProvider).Assembly)
                .WithParts(types)
                .Build();
        }

        public virtual IEnumerable<Assembly> GetAssemblies()
        {
            return new List<Assembly>
                       {
                           typeof(IServiceProvider).Assembly,              /* Kephas.Injection */
                           typeof(IEventHub).Assembly,              /* Kephas.Interaction */
                           typeof(ISerializationService).Assembly,  /* Kephas.Serialization */
                           typeof(AmbientServices).Assembly,        /* Kephas.Core*/
                           typeof(AutofacServiceProvider).Assembly,      /* Kephas.Injection.Autofac */
                       };
        }

        public virtual IEnumerable<Type> GetDefaultParts()
        {
            return new List<Type>();
        }
    }
}