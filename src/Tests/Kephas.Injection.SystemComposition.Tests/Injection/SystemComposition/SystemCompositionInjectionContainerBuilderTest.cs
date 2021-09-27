// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SystemCompositionInjectionContainerBuilderTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Tests.Injection.SystemComposition
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
    using Kephas.Injection;
    using Kephas.Injection.AttributedModel;
    using Kephas.Injection.Conventions;
    using Kephas.Injection.Hosting;
    using Kephas.Injection.SystemComposition;
    using Kephas.Injection.SystemComposition.Conventions;
    using Kephas.Injection.SystemComposition.Hosting;
    using Kephas.Injection.SystemComposition.ScopeFactory;
    using Kephas.Logging;
    using Kephas.Services;
    using Kephas.Services.Reflection;
    using Kephas.Testing.Injection;
    using NSubstitute;
    using NUnit.Framework;

    /// <summary>
    /// Tests for <see cref="SystemCompositionInjectorBuilder"/>.
    /// </summary>
    [TestFixture]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public class SystemCompositionInjectionContainerBuilderTest : SystemCompositionInjectionTestBase
    {
        [Test]
        public async Task CreateInjector_simple_ambient_services_exported()
        {
            var builder = this.CreateCompositionContainerBuilder();
            var mockAppRuntime = builder.AppRuntime;

            mockAppRuntime.GetAppAssemblies(Arg.Any<Func<AssemblyName, bool>>())
                .Returns(new[] { typeof(ILogger).GetTypeInfo().Assembly, typeof(SystemCompositionInjector).GetTypeInfo().Assembly });

            var container = builder.Build();

            var loggerManager = container.Resolve<ILogManager>();
            Assert.AreEqual(builder.LogManager, loggerManager);

            var platformManager = container.Resolve<IAppRuntime>();
            Assert.AreEqual(mockAppRuntime, platformManager);
        }

        [Test]
        public void CreateInjector_simple_ambient_services_exported_no_assemblies()
        {
            var builder = this.CreateCompositionContainerBuilder();
            var container = builder
                .WithAssemblies(Array.Empty<Assembly>())
                .Build();

            var loggerManager = container.Resolve<ILogManager>();
            Assert.AreEqual(builder.LogManager, loggerManager);

            var platformManager = container.Resolve<IAppRuntime>();
            Assert.AreEqual(builder.AppRuntime, platformManager);
        }

        [Test]
        public void CreateInjector_composed_loggers_exported()
        {
            var builder = this.CreateInjectorBuilderWithStringLogger();
            var container = builder
                .WithAssembly(typeof(IInjector).GetTypeInfo().Assembly)
                .WithAssembly(typeof(IContextFactory).GetTypeInfo().Assembly)
                .WithAssembly(typeof(SystemCompositionConventionsBuilder).GetTypeInfo().Assembly)
                .Build();

            var logger = container.Resolve<ILogger<SystemCompositionInjectionContainerTest.ExportedClass>>();
            Assert.IsInstanceOf<TypedLogger<SystemCompositionInjectionContainerTest.ExportedClass>>(logger);
        }

        [Test]
        public void Resolve_AppService_Singleton()
        {
            var builder = this.CreateInjectorBuilderWithStringLogger();
            var container = builder
                .WithAssembly(typeof(IInjector).GetTypeInfo().Assembly)
                .WithAssembly(typeof(IContextFactory).GetTypeInfo().Assembly)
                .WithParts(new[] { typeof(ITestAppService), typeof(TestAppService) })
                .Build();

            var exported = container.Resolve<ITestAppService>();
            var secondExported = container.Resolve<ITestAppService>();

            Assert.AreSame(exported, secondExported);
        }

        [Test]
        public void Resolve_AppService_Single_Success()
        {
            var builder = this.CreateInjectorBuilderWithStringLogger();
            var container = builder
                .WithAssembly(typeof(IInjector).GetTypeInfo().Assembly)
                .WithAssembly(typeof(IContextFactory).GetTypeInfo().Assembly)
                .WithParts(new[] { typeof(ITestAppService), typeof(TestAppService) })
                .Build();

            var exported = container.Resolve<ITestAppService>();

            Assert.IsInstanceOf<TestAppService>(exported);
        }

        [Test]
        public void Resolve_AppService_Single_Override_Success()
        {
            var builder = this.CreateInjectorBuilderWithStringLogger();
            var container = builder
                .WithAssembly(typeof(IInjector).GetTypeInfo().Assembly)
                .WithAssembly(typeof(IContextFactory).GetTypeInfo().Assembly)
                .WithParts(new[] { typeof(ITestAppService), typeof(TestAppService), typeof(TestOverrideAppService) })
                .Build();

            var exported = container.Resolve<ITestAppService>();

            Assert.IsInstanceOf<TestOverrideAppService>(exported);
        }

        [Test]
        public void ResolveMany_AppService_Multiple_Singleton()
        {
            var builder = this.CreateInjectorBuilderWithStringLogger();
            var container = builder
                .WithAssembly(typeof(IInjector).GetTypeInfo().Assembly)
                .WithAssembly(typeof(IContextFactory).GetTypeInfo().Assembly)
                .WithParts(new[] { typeof(ITestMultiAppService), typeof(TestMultiAppService1), typeof(TestMultiAppService2) })
                .Build();

            var exports = container.ResolveMany<ITestMultiAppService>().ToList();
            var exports2 = container.ResolveMany<ITestMultiAppService>().ToList();

            Assert.AreSame(exports[0], exports2[0]);
            Assert.AreSame(exports[1], exports2[1]);
        }

        [Test]
        public void ResolveMany_AppService_Multiple_Success()
        {
            var builder = this.CreateInjectorBuilderWithStringLogger();
            var container = builder
                .WithAssembly(typeof(IInjector).GetTypeInfo().Assembly)
                .WithAssembly(typeof(IContextFactory).GetTypeInfo().Assembly)
                .WithParts(new[] { typeof(ITestMultiAppService), typeof(TestMultiAppService1), typeof(TestMultiAppService2) })
                .Build();

            var exports = container.ResolveMany<ITestMultiAppService>().ToList();

            Assert.AreEqual(2, exports.Count);
        }

        [Test]
        public void ResolveMany_AppService_IExportFactory_Success()
        {
            var builder = this.CreateInjectorBuilderWithStringLogger();
            var container = builder
                .WithAssembly(typeof(IInjector).GetTypeInfo().Assembly)
                .WithAssembly(typeof(IContextFactory).GetTypeInfo().Assembly)
                .WithParts(new[] { typeof(ITestMultiAppService), typeof(ITestMultiAppServiceConsumer), typeof(TestMultiAppService1), typeof(TestMultiAppService2), typeof(TestMultiAppServiceConsumer) })
                .Build();

            var export = (TestMultiAppServiceConsumer)container.Resolve<ITestMultiAppServiceConsumer>();

            Assert.AreEqual(2, export.Factories.Count());
            Assert.AreEqual(2, export.MetadataFactories.Count());
        }

        [Test]
        public void Resolve_AppService_generic_export()
        {
            var builder = this.CreateInjectorBuilderWithStringLogger();
            var container = builder
                .WithAssembly(typeof(IInjector).GetTypeInfo().Assembly)
                .WithAssembly(typeof(IContextFactory).GetTypeInfo().Assembly)
                .WithParts(new[] { typeof(ITestGenericExport<>), typeof(TestGenericExport) })
                .Build();

            var export = container.Resolve<ITestGenericExport<string>>();
            Assert.IsInstanceOf<TestGenericExport>(export);
        }

        [Test]
        public void Resolve_AppService_generic_export_with_non_generic_contract()
        {
            var builder = this.CreateInjectorBuilderWithStringLogger();
            var container = builder
                .WithAssembly(typeof(IInjector).GetTypeInfo().Assembly)
                .WithAssembly(typeof(IContextFactory).GetTypeInfo().Assembly)
                .WithParts(new[] { typeof(ITestGenericWithNonGenericExport), typeof(ITestGenericWithNonGenericExport<>), typeof(TestGenericWithNonGenericExport) })
                .Build();

            var export = container.Resolve<ITestGenericWithNonGenericExport>();
            Assert.IsInstanceOf<TestGenericWithNonGenericExport>(export);
        }

        [Test]
        public void Resolve_AppService_with_injection_constructor()
        {
            var builder = this.CreateInjectorBuilderWithStringLogger();
            var container = builder
                .WithAssembly(typeof(IInjector).GetTypeInfo().Assembly)
                .WithAssembly(typeof(IContextFactory).GetTypeInfo().Assembly)
                .WithRegistration(
                    new AppServiceInfo(typeof(ExportedClass), typeof(ExportedClass)),
                    new AppServiceInfo(typeof(ExportedClassWithFakeDependency), typeof(ExportedClassWithFakeDependency)))
                .Build();
            var exported = container.Resolve<ExportedClassWithFakeDependency>();

            Assert.IsNotNull(exported);
            Assert.IsInstanceOf<ExportedClassWithFakeDependency>(exported);
            Assert.IsNull(exported.Dependency);
        }

        [Test]
        public void Resolve_ScopedAppService_no_scope()
        {
            var builder = this.CreateInjectorBuilderWithStringLogger();
            var container = builder
                .WithAssembly(typeof(IInjector).GetTypeInfo().Assembly)
                .WithAssembly(typeof(IContextFactory).GetTypeInfo().Assembly)
                .WithParts(new[] { typeof(ITestScopedExport), typeof(TestScopedExport) })
                .Build();

            Assert.Throws<CompositionFailedException>(() => container.Resolve<ITestScopedExport>());
        }

        [Test]
        public void Resolve_ScopedAppService_export()
        {
            var builder = this.CreateInjectorBuilderWithStringLogger();
            var container = builder
                .WithAssembly(typeof(IInjector).GetTypeInfo().Assembly)
                .WithAssembly(typeof(IContextFactory).GetTypeInfo().Assembly)
                .WithParts(new[] { typeof(ITestScopedExport), typeof(TestScopedExport) })
                .Build();

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
            var builder = this.CreateInjectorBuilderWithStringLogger();
            var container = builder
                .WithAssembly(typeof(IInjector).GetTypeInfo().Assembly)
                .WithAssembly(typeof(IContextFactory).GetTypeInfo().Assembly)
                .WithParts(new[] { typeof(ITestMyScopedExport), typeof(TestMyScopedExport) })
                .WithScopeFactory<MyScopeFactory>()
                .Build();

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
            var builder = this.CreateInjectorBuilderWithStringLogger();
            var container = builder
                .WithAssembly(typeof(IInjector).GetTypeInfo().Assembly)
                .WithAssembly(typeof(IContextFactory).GetTypeInfo().Assembly)
                .WithParts(new[] { typeof(ITestMyScopedExport), typeof(TestMyScopedExport), typeof(MyScopeFactory) })
                .Build();

            var scopedContext = container.CreateScopedInjector();
            Assert.AreNotSame(container, scopedContext);
            Assert.IsNotNull(scopedContext);
        }

        [Test]
        public void Resolve_AppService_no_constructor()
        {
            var builder = this.CreateInjectorBuilderWithStringLogger();
            Assert.Throws<InjectionException>(() => builder
                .WithAssembly(typeof(IInjector).GetTypeInfo().Assembly)
                .WithAssembly(typeof(IContextFactory).GetTypeInfo().Assembly)
                .WithParts(new[] { typeof(IConstructorAppService), typeof(NoCompositionConstructorAppService) })
                .Build());
        }

        [Test]
        public void Resolve_AppService_ambiguous_constructor()
        {
            var builder = this.CreateInjectorBuilderWithStringLogger();
            Assert.Throws<InjectionException>(() => builder
                .WithAssembly(typeof(IInjector).GetTypeInfo().Assembly)
                .WithAssembly(typeof(IContextFactory).GetTypeInfo().Assembly)
                .WithParts(new[] { typeof(IConstructorAppService), typeof(AmbiguousCompositionConstructorAppService) })
                .Build());
        }

        [Test]
        public void Resolve_AppService_largest_constructor()
        {
            var builder = this.CreateInjectorBuilderWithStringLogger();
            var container = builder
                .WithAssembly(typeof(IInjector).GetTypeInfo().Assembly)
                .WithAssembly(typeof(IContextFactory).GetTypeInfo().Assembly)
                .WithParts(new[] { typeof(IConstructorAppService), typeof(LargestCompositionConstructorAppService) })
                .Build();

            var component = container.Resolve<IConstructorAppService>();
            Assert.IsNotNull(component);
        }

        [Test]
        public void Resolve_AppService_multiple_constructor()
        {
            var builder = this.CreateInjectorBuilderWithStringLogger();
            Assert.Throws<InjectionException>(() => builder
                .WithAssembly(typeof(IInjector).GetTypeInfo().Assembly)
                .WithAssembly(typeof(IContextFactory).GetTypeInfo().Assembly)
                .WithParts(new[] { typeof(IConstructorAppService), typeof(MultipleCompositionConstructorAppService) })
                .Build());
        }

        [Test]
        public void Resolve_AppService_default_constructor()
        {
            var builder = this.CreateInjectorBuilderWithStringLogger();
            var container = builder
                .WithAssembly(typeof(IInjector).GetTypeInfo().Assembly)
                .WithAssembly(typeof(IContextFactory).GetTypeInfo().Assembly)
                .WithParts(new[] { typeof(IConstructorAppService), typeof(DefaultConstructorAppService) })
                .Build();

            var export = container.Resolve<IConstructorAppService>();

            Assert.IsInstanceOf<DefaultConstructorAppService>(export);
        }

        [Test]
        public void Resolve_AppService_single_constructor()
        {
            var builder = this.CreateInjectorBuilderWithStringLogger();
            var container = builder
                .WithAssembly(typeof(IInjector).GetTypeInfo().Assembly)
                .WithAssembly(typeof(IContextFactory).GetTypeInfo().Assembly)
                .WithParts(new[] { typeof(IConstructorAppService), typeof(SingleConstructorAppService) })
                .Build();

            var export = container.Resolve<IConstructorAppService>();

            Assert.IsInstanceOf<SingleConstructorAppService>(export);
        }

        [Test]
        public async Task CreateInjector_instance_registration()
        {
            var registrar = Substitute.For<IAppServiceInfosProvider>();
            registrar.GetAppServiceInfos(Arg.Any<dynamic>())
                .Returns((typeof(string), new AppServiceInfo(typeof(string), "123")));

            var factory = this.CreateCompositionContainerBuilder(ctx => ctx.AppServiceInfosProviders = new[] { registrar });
            var mockPlatformManager = factory.AppRuntime;

            mockPlatformManager.GetAppAssemblies(Arg.Any<Func<AssemblyName, bool>>())
                .Returns(new[] { typeof(ILogger).GetTypeInfo().Assembly, typeof(SystemCompositionInjector).GetTypeInfo().Assembly });

            var container = factory.Build();

            var instance = container.Resolve<string>();
            Assert.AreEqual("123", instance);
        }

        [Test]
        public async Task CreateInjector_instance_factory_registration()
        {
            var registrar = Substitute.For<IAppServiceInfosProvider>();
            registrar.GetAppServiceInfos(Arg.Any<dynamic>())
                .Returns((typeof(string), new AppServiceInfo(typeof(string), injector => "123")));

            var factory = this.CreateCompositionContainerBuilder(ctx => ctx.AppServiceInfosProviders = new[] { registrar });
            var mockPlatformManager = factory.AppRuntime;

            mockPlatformManager.GetAppAssemblies(Arg.Any<Func<AssemblyName, bool>>())
                .Returns(new[] { typeof(ILogger).GetTypeInfo().Assembly, typeof(SystemCompositionInjector).GetTypeInfo().Assembly });

            var container = factory.Build();

            var instance = container.Resolve<string>();
            Assert.AreEqual("123", instance);
        }

        private SystemCompositionInjectorBuilder CreateCompositionContainerBuilder(Action<IInjectionBuildContext> config = null)
        {
            var mockLoggerManager = Substitute.For<ILogManager>();
            var mockPlatformManager = Substitute.For<IAppRuntime>();

            var context = new InjectionBuildContext(new AmbientServices()
                                        .Register(mockLoggerManager)
                                        .Register(mockPlatformManager));
            config?.Invoke(context);
            var factory = new SystemCompositionInjectorBuilder(context);
            return factory;
        }

        private SystemCompositionInjectorBuilder CreateInjectorBuilderWithStringLogger()
        {
            var builder = this.CreateCompositionContainerBuilder();

            var mockLoggerManager = builder.LogManager;
            mockLoggerManager.GetLogger(Arg.Any<string>()).Returns(Substitute.For<ILogger>());

            return builder;
        }

        [Export]
        public class ComposedTestLogConsumer
        {
            public ComposedTestLogConsumer(ILogger<SystemCompositionInjectionContainerBuilderTest.ComposedTestLogConsumer> logger)
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

        [SingletonAppServiceContract]
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

        [SingletonAppServiceContract(ContractType = typeof(IConverter))]
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
            /// The injector.
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
            /// The injector.
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
            /// The injector.
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
            /// The injector.
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
