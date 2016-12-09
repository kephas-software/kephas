// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompositionTestBase.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Base class for tests using composition.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using NSubstitute;

namespace Kephas.Testing.Composition.Mef
{
    using System;
    using System.Collections.Generic;
    using System.Composition.Hosting;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;

    using Kephas.Application;
    using Kephas.Composition;
    using Kephas.Composition.Hosting;
    using Kephas.Composition.Mef.ExportProviders;
    using Kephas.Composition.Mef.Hosting;
    using Kephas.Configuration;
    using Kephas.Logging;

    using NUnit.Framework;

    /// <summary>
    /// Base class for tests using composition.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public class CompositionTestBase
    {
        [OneTimeSetUp]
        public void TestSetup()
        {
            this.OnTestSetup();
        }

        public virtual void OnTestSetup()
        {
        }

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

        public virtual MefCompositionContainerBuilder WithContainerBuilder(ILogManager logManager = null, IConfigurationManager configurationManager = null, IAppRuntime appRuntime = null)
        {
            logManager = logManager ?? Substitute.For<ILogManager>();
            configurationManager = configurationManager ?? Substitute.For<IConfigurationManager>();
            appRuntime = appRuntime ?? Substitute.For<IAppRuntime>();

            var ambientServices = new AmbientServices();
            ambientServices
                .RegisterService(logManager)
                .RegisterService(configurationManager)
                .RegisterService(appRuntime);
            return new MefCompositionContainerBuilder(new CompositionContainerBuilderContext(ambientServices));
        }

        public ICompositionContext CreateContainer(params Assembly[] assemblies)
        {
            return this.CreateContainer((IEnumerable<Assembly>)assemblies);
        }

        public virtual ICompositionContext CreateContainer(IEnumerable<Assembly> assemblies)
        {
            return
                this.WithContainerBuilder()
                    .WithAssemblies(this.GetDefaultConventionAssemblies())
                    .WithAssemblies(assemblies ?? new Assembly[0])
                    .CreateContainer();
        }

        public ICompositionContext CreateContainerWithBuilder(params Type[] types)
        {
            var configuration = this.WithEmptyConfiguration().WithParts(types);
            return this
                .WithContainerBuilder()
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