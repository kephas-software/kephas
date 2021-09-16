﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SystemCompositionContainerBuilderTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Tests.Composition.Mef
{
    using System;
    using System.Collections.Generic;
    using System.Composition;
    using System.Composition.Hosting;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Composition.Mef;
    using Kephas.Composition.Mef.Conventions;
    using Kephas.Composition.Mef.Hosting;
    using Kephas.Composition.Mef.ScopeFactory;
    using Kephas.Injection;
    using Kephas.Injection.AttributedModel;
    using Kephas.Injection.Conventions;
    using Kephas.Injection.Hosting;
    using Kephas.Logging;
    using Kephas.Services;
    using Kephas.Services.Composition;
    using Kephas.Services.Reflection;
    using Kephas.Testing.Composition;
    using NSubstitute;
    using NUnit.Framework;

    /// <summary>
    /// Tests for <see cref="SystemCompositionInjectorBuilder"/>.
    /// </summary>
    [TestFixture]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public class SystemCompositionContainerBuilderTest : SystemCompositionTestBase
    {
        [Test]
        public async Task CreateContainer_simple_ambient_services_exported()
        {
            var builder = this.CreateCompositionContainerBuilder();
            var mockAppRuntime = builder.AppRuntime;

            mockAppRuntime.GetAppAssemblies(Arg.Any<Func<AssemblyName, bool>>())
                .Returns(new[] { typeof(ILogger).GetTypeInfo().Assembly, typeof(SystemCompositionContainer).GetTypeInfo().Assembly });

            var container = builder.CreateInjector();

            var loggerManager = container.Resolve<ILogManager>();
            Assert.AreEqual(builder.LogManager, loggerManager);

            var platformManager = container.Resolve<IAppRuntime>();
            Assert.AreEqual(mockAppRuntime, platformManager);
        }

        [Test]
        public void CreateContainer_simple_ambient_services_exported_no_assemblies()
        {
            var builder = this.CreateCompositionContainerBuilder();
            var container = builder
                .WithAssemblies(new Assembly[0])
                .WithPart(typeof(AppServiceInfoConventionsRegistrar))
                .CreateInjector();

            var loggerManager = container.Resolve<ILogManager>();
            Assert.AreEqual(builder.LogManager, loggerManager);

            var platformManager = container.Resolve<IAppRuntime>();
            Assert.AreEqual(builder.AppRuntime, platformManager);
        }

        [Test]
        public void CreateContainer_composed_loggers_exported()
        {
            var builder = this.CreateCompositionContainerBuilderWithStringLogger();
            var container = builder
                .WithAssembly(typeof(IInjector).GetTypeInfo().Assembly)
                .WithAssembly(typeof(MefConventionsBuilder).GetTypeInfo().Assembly)
                .CreateInjector();

            var logger = container.Resolve<ILogger<SystemCompositionContainerTest.ExportedClass>>();
            Assert.IsInstanceOf<TypedLogger<SystemCompositionContainerTest.ExportedClass>>(logger);
        }

        [Test]
        public void Resolve_AppService_Singleton()
        {
            var builder = this.CreateCompositionContainerBuilderWithStringLogger();
            var container = builder
                .WithAssembly(typeof(IInjector).GetTypeInfo().Assembly)
                .WithParts(new[] { typeof(ITestAppService), typeof(TestAppService) })
                .CreateInjector();

            var exported = container.Resolve<ITestAppService>();
            var secondExported = container.Resolve<ITestAppService>();

            Assert.AreSame(exported, secondExported);
        }

        [Test]
        public void Resolve_AppService_Single_Success()
        {
            var builder = this.CreateCompositionContainerBuilderWithStringLogger();
            var container = builder
                .WithAssembly(typeof(IInjector).GetTypeInfo().Assembly)
                .WithParts(new[] { typeof(ITestAppService), typeof(TestAppService) })
                .CreateInjector();

            var exported = container.Resolve<ITestAppService>();

            Assert.IsInstanceOf<TestAppService>(exported);
        }

        [Test]
        public void Resolve_AppService_Single_Override_Success()
        {
            var builder = this.CreateCompositionContainerBuilderWithStringLogger();
            var container = builder
                .WithAssembly(typeof(IInjector).GetTypeInfo().Assembly)
                .WithParts(new[] { typeof(ITestAppService), typeof(TestAppService), typeof(TestOverrideAppService) })
                .CreateInjector();

            var exported = container.Resolve<ITestAppService>();

            Assert.IsInstanceOf<TestOverrideAppService>(exported);
        }

        [Test]
        public void ResolveMany_AppService_Multiple_Singleton()
        {
            var builder = this.CreateCompositionContainerBuilderWithStringLogger();
            var container = builder
                .WithAssembly(typeof(IInjector).GetTypeInfo().Assembly)
                .WithParts(new[] { typeof(ITestMultiAppService), typeof(TestMultiAppService1), typeof(TestMultiAppService2) })
                .CreateInjector();

            var exports = container.ResolveMany<ITestMultiAppService>().ToList();
            var exports2 = container.ResolveMany<ITestMultiAppService>().ToList();

            Assert.AreSame(exports[0], exports2[0]);
            Assert.AreSame(exports[1], exports2[1]);
        }

        [Test]
        public void ResolveMany_AppService_Multiple_Success()
        {
            var builder = this.CreateCompositionContainerBuilderWithStringLogger();
            var container = builder
                .WithAssembly(typeof(IInjector).GetTypeInfo().Assembly)
                .WithParts(new[] { typeof(ITestMultiAppService), typeof(TestMultiAppService1), typeof(TestMultiAppService2) })
                .CreateInjector();

            var exports = container.ResolveMany<ITestMultiAppService>().ToList();

            Assert.AreEqual(2, exports.Count);
        }

        [Test]
        public void ResolveMany_AppService_IExportFactory_Success()
        {
            var builder = this.CreateCompositionContainerBuilderWithStringLogger();
            var container = builder
                .WithAssembly(typeof(IInjector).GetTypeInfo().Assembly)
                .WithParts(new[] { typeof(ITestMultiAppService), typeof(ITestMultiAppServiceConsumer), typeof(TestMultiAppService1), typeof(TestMultiAppService2), typeof(TestMultiAppServiceConsumer) })
                .CreateInjector();

            var export = (TestMultiAppServiceConsumer)container.Resolve<ITestMultiAppServiceConsumer>();

            Assert.AreEqual(2, export.Factories.Count());
            Assert.AreEqual(2, export.MetadataFactories.Count());
        }

        [Test]
        public void Resolve_AppService_generic_export()
        {
            var builder = this.CreateCompositionContainerBuilderWithStringLogger();
            var container = builder
                .WithAssembly(typeof(IInjector).GetTypeInfo().Assembly)
                .WithParts(new[] { typeof(ITestGenericExport<>), typeof(TestGenericExport) })
                .CreateInjector();

            var export = container.Resolve<ITestGenericExport<string>>();
            Assert.IsInstanceOf<TestGenericExport>(export);
        }

        [Test]
        public void Resolve_AppService_generic_export_with_non_generic_contract()
        {
            var builder = this.CreateCompositionContainerBuilderWithStringLogger();
            var container = builder
                .WithAssembly(typeof(IInjector).GetTypeInfo().Assembly)
                .WithParts(new[] { typeof(ITestGenericWithNonGenericExport), typeof(ITestGenericWithNonGenericExport<>), typeof(TestGenericWithNonGenericExport) })
                .CreateInjector();

            var export = container.Resolve<ITestGenericWithNonGenericExport>();
            Assert.IsInstanceOf<TestGenericWithNonGenericExport>(export);
        }

        [Test]
        public void Resolve_AppService_with_injection_constructor()
        {
            var builder = this.CreateCompositionContainerBuilderWithStringLogger();
            var container = builder
                .WithAssembly(typeof(IInjector).GetTypeInfo().Assembly)
                .WithRegistration(
                    new AppServiceInfo(typeof(ExportedClass), typeof(ExportedClass)),
                    new AppServiceInfo(typeof(ExportedClassWithFakeDependency), typeof(ExportedClassWithFakeDependency)))
                .CreateInjector();
            var exported = container.Resolve<ExportedClassWithFakeDependency>();

            Assert.IsNotNull(exported);
            Assert.IsInstanceOf<ExportedClassWithFakeDependency>(exported);
            Assert.IsNull(exported.Dependency);
        }

        [Test]
        public void Resolve_ScopedAppService_no_scope()
        {
            var builder = this.CreateCompositionContainerBuilderWithStringLogger();
            var container = builder
                .WithAssembly(typeof(IInjector).GetTypeInfo().Assembly)
                .WithParts(new[] { typeof(ITestScopedExport), typeof(TestScopedExport) })
                .CreateInjector();

            Assert.Throws<CompositionFailedException>(() => container.Resolve<ITestScopedExport>());
        }

        [Test]
        public void Resolve_ScopedAppService_export()
        {
            var builder = this.CreateCompositionContainerBuilderWithStringLogger();
            var container = builder
                .WithAssembly(typeof(IInjector).GetTypeInfo().Assembly)
                .WithParts(new[] { typeof(ITestScopedExport), typeof(TestScopedExport) })
                .CreateInjector();

            ITestScopedExport exportScope1;
            using (var scopedContext = container.CreateScopedInjector())
            {
                exportScope1 = scopedContext.Resolve<ITestScopedExport>();
                Assert.IsInstanceOf<TestScopedExport>(exportScope1);

                var export = scopedContext.Resolve<ITestScopedExport>();
                Assert.AreSame(exportScope1, export);
            }

            using (var scopedContext2 = container.CreateScopedInjector())
            {
                var export2 = scopedContext2.Resolve<ITestScopedExport>();
                Assert.AreNotSame(exportScope1, export2);
            }
        }

        [Test, Ignore("Custom scope factories not supported from version 7.0.0")]
        public void Resolve_ScopedAppService_custom_scope_export()
        {
            var builder = this.CreateCompositionContainerBuilderWithStringLogger();
            var container = builder
                .WithAssembly(typeof(IInjector).GetTypeInfo().Assembly)
                .WithParts(new[] { typeof(ITestMyScopedExport), typeof(TestMyScopedExport) })
                .WithScopeFactory<MyScopeFactory>()
                .CreateInjector();

            ITestMyScopedExport exportScope1;
            using (var scopedContext = container.CreateScopedInjector())
            {
                exportScope1 = scopedContext.Resolve<ITestMyScopedExport>();
                Assert.IsInstanceOf<TestMyScopedExport>(exportScope1);

                var export = scopedContext.Resolve<ITestMyScopedExport>();
                Assert.AreSame(exportScope1, export);
            }

            using (var scopedContext2 = container.CreateScopedInjector())
            {
                var export2 = scopedContext2.Resolve<ITestMyScopedExport>();
                Assert.AreNotSame(exportScope1, export2);
            }
        }

        [Test]
        public void Resolve_ScopedAppService_scopefactory_composed_only_once()
        {
            var builder = this.CreateCompositionContainerBuilderWithStringLogger();
            var container = builder
                .WithAssembly(typeof(IInjector).GetTypeInfo().Assembly)
                .WithParts(new[] { typeof(ITestMyScopedExport), typeof(TestMyScopedExport), typeof(MyScopeFactory) })
                .CreateInjector();

            var scopedContext = container.CreateScopedInjector();
            Assert.AreNotSame(container, scopedContext);
            Assert.IsNotNull(scopedContext);
        }

        [Test]
        public void Resolve_AppService_no_constructor()
        {
            var builder = this.CreateCompositionContainerBuilderWithStringLogger();
            Assert.Throws<InjectionException>(() => builder
                .WithAssembly(typeof(IInjector).GetTypeInfo().Assembly)
                .WithParts(new[] { typeof(IConstructorAppService), typeof(NoCompositionConstructorAppService) })
                .CreateInjector());
        }

        [Test]
        public void Resolve_AppService_ambiguous_constructor()
        {
            var builder = this.CreateCompositionContainerBuilderWithStringLogger();
            Assert.Throws<InjectionException>(() => builder
                .WithAssembly(typeof(IInjector).GetTypeInfo().Assembly)
                .WithParts(new[] { typeof(IConstructorAppService), typeof(AmbiguousCompositionConstructorAppService) })
                .CreateInjector());
        }

        [Test]
        public void Resolve_AppService_largest_constructor()
        {
            var builder = this.CreateCompositionContainerBuilderWithStringLogger();
            var container = builder
                .WithAssembly(typeof(IInjector).GetTypeInfo().Assembly)
                .WithParts(new[] { typeof(IConstructorAppService), typeof(LargestCompositionConstructorAppService) })
                .CreateInjector();

            var component = container.Resolve<IConstructorAppService>();
            Assert.IsNotNull(component);
        }

        [Test]
        public void Resolve_AppService_multiple_constructor()
        {
            var builder = this.CreateCompositionContainerBuilderWithStringLogger();
            Assert.Throws<InjectionException>(() => builder
                .WithAssembly(typeof(IInjector).GetTypeInfo().Assembly)
                .WithParts(new[] { typeof(IConstructorAppService), typeof(MultipleCompositionConstructorAppService) })
                .CreateInjector());
        }

        [Test]
        public void Resolve_AppService_default_constructor()
        {
            var builder = this.CreateCompositionContainerBuilderWithStringLogger();
            var container = builder
                .WithAssembly(typeof(IInjector).GetTypeInfo().Assembly)
                .WithParts(new[] { typeof(IConstructorAppService), typeof(DefaultConstructorAppService) })
                .CreateInjector();

            var export = container.Resolve<IConstructorAppService>();

            Assert.IsInstanceOf<DefaultConstructorAppService>(export);
        }

        [Test]
        public void Resolve_AppService_single_constructor()
        {
            var builder = this.CreateCompositionContainerBuilderWithStringLogger();
            var container = builder
                .WithAssembly(typeof(IInjector).GetTypeInfo().Assembly)
                .WithParts(new[] { typeof(IConstructorAppService), typeof(SingleConstructorAppService) })
                .CreateInjector();

            var export = container.Resolve<IConstructorAppService>();

            Assert.IsInstanceOf<SingleConstructorAppService>(export);
        }

        [Test]
        public async Task CreateContainer_instance_registration()
        {
            var registrar = Substitute.For<IConventionsRegistrar>();
            registrar
                .WhenForAnyArgs(r => r.RegisterConventions(Arg.Any<IConventionsBuilder>(), Arg.Any<IList<Type>>(), Arg.Any<IInjectionRegistrationContext>()))
                .Do(ci => { ci.Arg<IConventionsBuilder>().ForInstance(typeof(string), "123"); });

            var factory = this.CreateCompositionContainerBuilder(ctx => ctx.Registrars = new[] { registrar });
            var mockPlatformManager = factory.AppRuntime;

            mockPlatformManager.GetAppAssemblies(Arg.Any<Func<AssemblyName, bool>>())
                .Returns(new[] { typeof(ILogger).GetTypeInfo().Assembly, typeof(SystemCompositionContainer).GetTypeInfo().Assembly });

            var container = factory.CreateInjector();

            var instance = container.Resolve<string>();
            Assert.AreEqual("123", instance);
        }

        [Test]
        public async Task CreateContainer_instance_extension_registration()
        {
            var registrar = Substitute.For<IConventionsRegistrar>();
            registrar
                .WhenForAnyArgs(r => r.RegisterConventions(Arg.Any<IConventionsBuilder>(), Arg.Any<IList<Type>>(), Arg.Any<IInjectionRegistrationContext>()))
                .Do(ci => { ci.Arg<IConventionsBuilder>().ForInstance<string>("123"); });

            var factory = this.CreateCompositionContainerBuilder(ctx => ctx.Registrars = new[] { registrar });
            var mockPlatformManager = factory.AppRuntime;

            mockPlatformManager.GetAppAssemblies(Arg.Any<Func<AssemblyName, bool>>())
                .Returns(new[] { typeof(ILogger).GetTypeInfo().Assembly, typeof(SystemCompositionContainer).GetTypeInfo().Assembly });

            var container = factory.CreateInjector();

            var instance = container.Resolve<string>();
            Assert.AreEqual("123", instance);
        }

        [Test]
        public async Task CreateContainer_instance_factory_registration()
        {
            var registrar = Substitute.For<IConventionsRegistrar>();
            registrar
                .WhenForAnyArgs(r => r.RegisterConventions(Arg.Any<IConventionsBuilder>(), Arg.Any<IList<Type>>(), Arg.Any<IInjectionRegistrationContext>()))
                .Do(ci => { ci.Arg<IConventionsBuilder>().ForInstanceFactory(typeof(string), ctx => "123"); });

            var factory = this.CreateCompositionContainerBuilder(ctx => ctx.Registrars = new[] { registrar });
            var mockPlatformManager = factory.AppRuntime;

            mockPlatformManager.GetAppAssemblies(Arg.Any<Func<AssemblyName, bool>>())
                .Returns(new[] { typeof(ILogger).GetTypeInfo().Assembly, typeof(SystemCompositionContainer).GetTypeInfo().Assembly });

            var container = factory.CreateInjector();

            var instance = container.Resolve<string>();
            Assert.AreEqual("123", instance);
        }

        [Test]
        public async Task CreateContainer_instance_factory_extension_registration()
        {
            var registrar = Substitute.For<IConventionsRegistrar>();
            registrar
                .WhenForAnyArgs(r => r.RegisterConventions(Arg.Any<IConventionsBuilder>(), Arg.Any<IList<Type>>(), Arg.Any<IInjectionRegistrationContext>()))
                .Do(ci => { ci.Arg<IConventionsBuilder>().ForInstanceFactory<string>(ctx => "123"); });

            var factory = this.CreateCompositionContainerBuilder(ctx => ctx.Registrars = new[] { registrar });
            var mockPlatformManager = factory.AppRuntime;

            mockPlatformManager.GetAppAssemblies(Arg.Any<Func<AssemblyName, bool>>())
                .Returns(new[] { typeof(ILogger).GetTypeInfo().Assembly, typeof(SystemCompositionContainer).GetTypeInfo().Assembly });

            var container = factory.CreateInjector();

            var instance = container.Resolve<string>();
            Assert.AreEqual("123", instance);
        }

        private SystemCompositionInjectorBuilder CreateCompositionContainerBuilder(Action<IInjectionRegistrationContext> config = null)
        {
            var mockLoggerManager = Substitute.For<ILogManager>();
            var mockPlatformManager = Substitute.For<IAppRuntime>();

            var context = new InjectionRegistrationContext(new AmbientServices()
                                        .Register(mockLoggerManager)
                                        .Register(mockPlatformManager));
            config?.Invoke(context);
            var factory = new SystemCompositionInjectorBuilder(context);
            return factory;
        }

        private SystemCompositionInjectorBuilder CreateCompositionContainerBuilderWithStringLogger()
        {
            var builder = this.CreateCompositionContainerBuilder();

            var mockLoggerManager = builder.LogManager;
            mockLoggerManager.GetLogger(Arg.Any<string>()).Returns(Substitute.For<ILogger>());

            return builder;
        }

        [Export]
        public class ComposedTestLogConsumer
        {
            public ComposedTestLogConsumer(ILogger<SystemCompositionContainerBuilderTest.ComposedTestLogConsumer> logger)
            {
                this.Logger = logger;
            }

            /// <summary>
            /// Gets or sets the logger.
            /// </summary>
            public ILogger<ComposedTestLogConsumer> Logger { get; private set; }
        }

        public class NonComposedTestLogConsumer
        {
            /// <summary>
            /// Gets or sets the logger.
            /// </summary>
            public ILogger<NonComposedTestLogConsumer> Logger { get; set; }
        }

        [SingletonAppServiceContract(MetadataAttributes = new[] { typeof(OverridePriorityAttribute) })]
        public interface ITestAppService { }

        [SingletonAppServiceContract(AllowMultiple = true)]
        public interface ITestMultiAppService { }

        [SingletonAppServiceContract]
        public interface ITestMultiAppServiceConsumer { }

        public class TestAppService : ITestAppService { }

        [OverridePriority(Priority.High)]
        public class TestOverrideAppService : ITestAppService { }

        public class TestMultiAppService1 : ITestMultiAppService { }

        public class TestMultiAppService2 : ITestMultiAppService { }

        public class TestMultiAppServiceConsumer : ITestMultiAppServiceConsumer
        {
            public TestMultiAppServiceConsumer(
                IEnumerable<IExportFactory<ITestMultiAppService>> factories,
                IEnumerable<IExportFactory<ITestMultiAppService, AppServiceMetadata>> metadataFactories)
            {
                this.Factories = factories;
                this.MetadataFactories = metadataFactories;
            }

            public IEnumerable<IExportFactory<ITestMultiAppService>> Factories { get; set; }

            public IEnumerable<IExportFactory<ITestMultiAppService, AppServiceMetadata>> MetadataFactories { get; set; }
        }

        public class TestMetadataConsumer
        {
            /// <summary>
            /// Gets or sets the test services.
            /// </summary>
            public ICollection<ExportFactoryAdapter<ITestAppService, AppServiceMetadata>> TestServices { get; set; }
        }

        public interface IConverter { }

        [SingletonAppServiceContract(MetadataAttributes = new[] { typeof(ProcessingPriorityAttribute) }, 
            ContractType = typeof(IConverter))]
        public interface IConverter<TSource, TTarget> : IConverter { }

        [ProcessingPriority(100)]
        public class StringToIntConverter : IConverter<string, int> { }

        public class SecondStringToIntConverter : IConverter<string, int> { }

        public class TestConverterConsumer
        {
            /// <summary>
            /// Gets or sets the converters.
            /// </summary>
            public ICollection<ExportFactoryAdapter<IConverter, AppServiceMetadata>> Converters { get; set; }
        }

        [ScopedAppServiceContract]
        public interface ITestMyScopedExport { }

        public class TestMyScopedExport : ITestMyScopedExport { }

        [InjectionScope]
        public class MyScopeFactory : ScopeFactoryBase
        {
            public MyScopeFactory([SharingBoundary(InjectionScopeNames.Default)] ExportFactory<CompositionContext> scopedContextFactory)
                : base(scopedContextFactory)
            {
            }
        }

        [ScopedAppServiceContract]
        public interface ITestScopedExport { }

        public class TestScopedExport : ITestScopedExport { }

        [SingletonAppServiceContract]
        public interface ITestGenericExport<T> { }

        public class TestGenericExport : ITestGenericExport<string> { }

        public interface ITestGenericWithNonGenericExport { }

        [AppServiceContract(ContractType = typeof(ITestGenericWithNonGenericExport))]
        public interface ITestGenericWithNonGenericExport<T> : ITestGenericWithNonGenericExport { }

        public class TestGenericWithNonGenericExport : ITestGenericWithNonGenericExport<string> { }

        [SingletonAppServiceContract]
        public interface IConstructorAppService { }

        public class DefaultConstructorAppService : IConstructorAppService
        {
        }

        public class SingleConstructorAppService : IConstructorAppService
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="SingleConstructorAppService"/> class.
            /// </summary>
            /// <param name="injectionContainer">
            /// The composition container.
            /// </param>
            public SingleConstructorAppService(IInjector injectionContainer)
            {
            }
        }

        public class AmbiguousCompositionConstructorAppService : IConstructorAppService
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="AmbiguousCompositionConstructorAppService"/>
            /// class.
            /// </summary>
            /// <param name="ambientServices">The ambient services.</param>
            public AmbiguousCompositionConstructorAppService(IAmbientServices ambientServices)
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="AmbiguousCompositionConstructorAppService"/> class.
            /// </summary>
            /// <param name="injectionContainer">
            /// The composition container.
            /// </param>
            public AmbiguousCompositionConstructorAppService(IInjector injectionContainer)
            {
            }
        }

        public class LargestCompositionConstructorAppService : IConstructorAppService
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="LargestCompositionConstructorAppService"/> class.
            /// </summary>
            public LargestCompositionConstructorAppService()
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="LargestCompositionConstructorAppService"/> class.
            /// </summary>
            /// <param name="injectionContainer">
            /// The composition container.
            /// </param>
            public LargestCompositionConstructorAppService(IInjector injectionContainer)
            {
            }
        }

        public class NoCompositionConstructorAppService : IConstructorAppService
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="NoCompositionConstructorAppService"/> class.
            /// </summary>
            private NoCompositionConstructorAppService()
            {
            }
        }

        public class MultipleCompositionConstructorAppService : IConstructorAppService
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="MultipleCompositionConstructorAppService"/> class.
            /// </summary>
            [InjectConstructor]
            public MultipleCompositionConstructorAppService()
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="MultipleCompositionConstructorAppService"/> class.
            /// </summary>
            /// <param name="injectionContainer">
            /// The composition container.
            /// </param>
            [InjectConstructor]
            public MultipleCompositionConstructorAppService(IInjector injectionContainer)
            {
            }
        }

        public class ExportedClass
        {
        }

        public class ExportedClassWithFakeDependency : ExportedClass
        {
            [InjectConstructor]
            public ExportedClassWithFakeDependency()
            {
            }

            public ExportedClassWithFakeDependency(ExportedClass dependency)
            {
                this.Dependency = dependency;
            }

            public ExportedClass Dependency { get; }
        }
    }
}
