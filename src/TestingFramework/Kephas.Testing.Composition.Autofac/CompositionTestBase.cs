// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompositionTestBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Base class for tests using composition.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Testing.Composition.Mef
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;

    using global::Autofac;

    using Kephas.Application;
    using Kephas.Composition;
    using Kephas.Composition.Autofac.Hosting;
    using Kephas.Composition.Autofac.Metadata;
    using Kephas.Composition.Hosting;
    using Kephas.Logging;

    /// <summary>
    /// Base class for tests using composition.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public class CompositionTestBase
    {
        public virtual ContainerBuilder WithEmptyConfiguration()
        {
            var configuration = new ContainerBuilder();
            return configuration;
        }

        public virtual ContainerBuilder WithExportProviders(ContainerBuilder configuration)
        {
            configuration.RegisterSource(new ExportFactoryRegistrationSource());
            configuration.RegisterSource(new ExportFactoryWithMetadataRegistrationSource());
            return configuration;
        }

        public virtual AutofacCompositionContainerBuilder WithContainerBuilder(IAmbientServices ambientServices = null, ILogManager logManager = null, IAppRuntime appRuntime = null)
        {
            logManager = logManager ?? new NullLogManager();
            appRuntime = appRuntime ?? new DefaultAppRuntime();

            ambientServices = ambientServices ?? new AmbientServices();
            ambientServices
                .RegisterService(logManager)
                .RegisterService(appRuntime);
            return new AutofacCompositionContainerBuilder(new CompositionRegistrationContext(ambientServices));
        }

        public ICompositionContext CreateContainer(params Assembly[] assemblies)
        {
            return this.CreateContainer((IEnumerable<Assembly>)assemblies);
        }

        public virtual ICompositionContext CreateContainer(IEnumerable<Assembly> assemblies = null, IEnumerable<Type> parts = null, Action<AutofacCompositionContainerBuilder> config = null)
        {
            var ambientServices = new AmbientServices();
            var containerBuilder = this.WithContainerBuilder(ambientServices)
                    .WithAssemblies(this.GetDefaultConventionAssemblies())
                    .WithAssemblies(assemblies ?? new Assembly[0])
                    .WithParts(parts ?? new Type[0]);

            config?.Invoke(containerBuilder);

            var container = containerBuilder.CreateContainer();
            ambientServices.RegisterService(container);
            return container;
        }

        public ICompositionContext CreateContainerWithBuilder(params Type[] types)
        {
            return this.WithContainerBuilder()
                .WithAssembly(typeof(ICompositionContext).GetTypeInfo().Assembly)
                .WithParts(types)
                .CreateContainer();
        }

        public ICompositionContext CreateContainerWithBuilder(IAmbientServices ambientServices, params Type[] types)
        {
            return this.WithContainerBuilder(ambientServices)
                .WithAssembly(typeof(ICompositionContext).GetTypeInfo().Assembly)
                .WithParts(types)
                .CreateContainer();
        }

        public virtual IEnumerable<Assembly> GetDefaultConventionAssemblies()
        {
            return new List<Assembly>
                       {
                           typeof(ICompositionContext).GetTypeInfo().Assembly,     /* Kephas.Core*/
                           typeof(AutofacCompositionContainer).GetTypeInfo().Assembly, /* Kephas.Composition.Autofac */
                       };
        }

        public virtual IEnumerable<Type> GetDefaultParts()
        {
            return new List<Type>();
        }
    }
}