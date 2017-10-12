// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompositionTestBase.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Base class for tests using composition.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Testing.Composition.Mef
{
    using System;
    using System.Collections.Generic;
    using System.Composition.Hosting;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;

    using Kephas.Application;
    using Kephas.Application.Configuration;
    using Kephas.Composition;
    using Kephas.Composition.Hosting;
    using Kephas.Composition.Mef.ExportProviders;
    using Kephas.Composition.Mef.Hosting;
    using Kephas.Logging;

    /// <summary>
    /// Base class for tests using composition.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public class CompositionTestBase
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

        public virtual MefCompositionContainerBuilder WithContainerBuilder(IAmbientServices ambientServices = null, ILogManager logManager = null, IAppConfiguration appConfiguration = null, IAppRuntime appRuntime = null)
        {
            logManager = logManager ?? new NullLogManager();
            appConfiguration = appConfiguration ?? new DynamicAppConfiguration();
            appRuntime = appRuntime ?? new NullAppRuntime();

            ambientServices = ambientServices ?? new AmbientServices();
            ambientServices
                .RegisterService(logManager)
                .RegisterService(appConfiguration)
                .RegisterService(appRuntime);
            return new MefCompositionContainerBuilder(new CompositionRegistrationContext(ambientServices));
        }

        public ICompositionContext CreateContainer(params Assembly[] assemblies)
        {
            return this.CreateContainer((IEnumerable<Assembly>)assemblies);
        }

        public virtual ICompositionContext CreateContainer(IEnumerable<Assembly> assemblies = null, IEnumerable<Type> parts = null, Action<MefCompositionContainerBuilder> config = null)
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
            var configuration = this.WithEmptyConfiguration().WithParts(types);
            return this.WithContainerBuilder()
                .WithAssembly(typeof(ICompositionContext).GetTypeInfo().Assembly)
                .WithConfiguration(configuration)
                .CreateContainer();
        }

        public ICompositionContext CreateContainerWithBuilder(IAmbientServices ambientServices, params Type[] types)
        {
            var configuration = this.WithEmptyConfiguration().WithParts(types);
            return this.WithContainerBuilder(ambientServices)
                .WithAssembly(typeof(ICompositionContext).GetTypeInfo().Assembly)
                .WithConfiguration(configuration)
                .CreateContainer();
        }

        public virtual IEnumerable<Assembly> GetDefaultConventionAssemblies()
        {
            return new List<Assembly>
                       {
                           typeof(ICompositionContext).GetTypeInfo().Assembly,     /* Kephas.Core*/
                           typeof(MefCompositionContainer).GetTypeInfo().Assembly, /* Kephas.Composition.Mef */
                       };
        }

        public virtual IEnumerable<Type> GetDefaultParts()
        {
            return new List<Type>();
        }
    }
}