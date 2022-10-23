// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AutofacInjectionBuilderTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Tests.Injection.Autofac
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading.Tasks;

    using global::Autofac.Core;
    using global::Autofac.Core.Activators.Reflection;
    using Kephas.Application;
    using Kephas.Services;
    using Kephas.Services.Autofac;
    using Kephas.Services.Builder;
    using Kephas.Logging;
    using Kephas.Services;
    using Kephas.Services.Reflection;
    using Kephas.Testing;
    using Kephas.Testing.Injection;
    using NSubstitute;
    using NUnit.Framework;

    /// <summary>
    /// Tests for <see cref="AutofacInjectorBuilder"/>.
    /// </summary>
    [TestFixture]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public class AutofacInjectionBuilderTest : AutofacInjectionTestBase
    {
        [Test]
        public async Task CreateInjector_simple_ambient_services_exported()
        {
            var builder = this.CreateInjectorBuilder();
            var mockAppRuntime = ambientServices.GetAppRuntime();

            mockAppRuntime.GetAppAssemblies()
                .Returns(ci => new[]
                {
                    typeof(IServiceProvider).Assembly,
                    typeof(ILogger).Assembly,
                    typeof(AmbientServices).Assembly,
                    typeof(AutofacServiceProvider).Assembly,
                });

            var container = builder.Build();

            var loggerManager = container.Resolve<ILogManager>();
            Assert.AreEqual(ambientServices.LogManager, loggerManager);

            var platformManager = container.Resolve<IAppRuntime>();
            Assert.AreEqual(mockAppRuntime, platformManager);
        }

        [Test]
        public void CreateInjector_simple_ambient_services_exported_no_assemblies()
        {
            var builder = this.CreateInjectorBuilder();
            var container = builder
                .WithAssemblies(typeof(AmbientServices).Assembly)
                .Build();

            var loggerManager = container.Resolve<ILogManager>();
            Assert.AreEqual(ambientServices.LogManager, loggerManager);

            var platformManager = container.Resolve<IAppRuntime>();
            Assert.AreEqual(ambientServices.GetAppRuntime(), platformManager);
        }

        [Test]
        public void CreateInjector_composed_loggers_exported()
        {
            var builder = this.CreateInjectorBuilderWithStringLogger();
            var container = builder
                .WithAssemblies(typeof(IInjectableFactory).Assembly)
                .WithAssemblies(typeof(AutofacServiceProvider).Assembly)
                .Build();

            var logger = container.Resolve<ILogger<AutofacInjectorTest.ExportedClass>>();
            Assert.IsInstanceOf<TypedLogger<AutofacInjectorTest.ExportedClass>>(logger);
        }

        [Test]
        public void Resolve_AppService_Singleton()
        {
            var builder = this.CreateInjectorBuilderWithStringLogger();
            var container = builder
                .WithAssemblies(typeof(IInjectableFactory).Assembly)
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
                .WithAssemblies(typeof(IInjectableFactory).Assembly)
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
                .WithAssemblies(typeof(IInjectableFactory).Assembly)
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
                .WithAssemblies(typeof(IInjectableFactory).Assembly)
                .WithParts(new[] { typeof(ITestMultiAppService), typeof(TestMultiAppService1), typeof(TestMultiAppService2) })
                .Build();

            var exports = container.ResolveMany<ITestMultiAppService>().ToList();
            var exports2 = container.ResolveMany<ITestMultiAppService>().ToList();

            Assert.AreSame(exports[0], exports2[0]);
            Assert.AreSame(exports[1], exports2[1]);
        }

        [Test]
        public void ResolveMany_AppService_Multiple_Preserve_Order_types()
        {
            var builder = this.CreateInjectorBuilderWithStringLogger();
            var container = builder
                .WithAssemblies(typeof(IInjectableFactory).Assembly)
                .WithParts(new[] { typeof(ITestMultiAppService), typeof(TestMultiAppService1), typeof(TestMultiAppService2) })
                .Build();

            var exports = container.ResolveMany<ITestMultiAppService>().ToList();

            Assert.AreEqual(2, exports.Count);
            Assert.IsInstanceOf<TestMultiAppService1>(exports[0]);
            Assert.IsInstanceOf<TestMultiAppService2>(exports[1]);
        }

        [Test]
        public void ResolveMany_AppService_Multiple_Preserve_Order_mixed()
        {
            var builder = this.CreateInjectorBuilderWithStringLogger()
                .WithAssemblies(typeof(IInjectableFactory).Assembly);
            builder.ForFactory(typeof(ITestMultiAppService), _ => new TestMultiAppService1()).Transient();
            builder.ForInstance(Substitute.For<ITestMultiAppService>()).As<ITestMultiAppService>();
            builder.ForType(typeof(TestMultiAppService2)).As<ITestMultiAppService>();
            var container = builder.Build();

            var exports = container.ResolveMany<ITestMultiAppService>().ToList();

            Assert.AreEqual(3, exports.Count);
            Assert.IsInstanceOf<TestMultiAppService1>(exports[0]);
            Assert.IsTrue(exports[1].GetType().Name.Contains("Proxy"));
            Assert.IsInstanceOf<TestMultiAppService2>(exports[2]);
        }

        [Test]
        public void ResolveMany_AppService_IExportFactory_Success()
        {
            var builder = this.CreateInjectorBuilderWithStringLogger();
            var container = builder
                .WithAssemblies(typeof(IInjectableFactory).Assembly)
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
                .WithAssemblies(typeof(IInjectableFactory).Assembly)
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
                .WithAssemblies(typeof(IInjectableFactory).Assembly)
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
                .WithAssemblies(typeof(IInjectableFactory).Assembly)
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
                .WithAssemblies(typeof(IInjectableFactory).Assembly)
                .WithParts(new[] { typeof(ITestScopedExport), typeof(TestScopedExport) })
                .Build();

            Assert.IsInstanceOf<TestScopedExport>(container.Resolve<ITestScopedExport>());
        }

        [Test]
        public void Resolve_ScopedAppService_export()
        {
            var builder = this.CreateInjectorBuilderWithStringLogger();
            var container = builder
                .WithAssemblies(typeof(IInjectableFactory).Assembly)
                .WithParts(new[] { typeof(ITestScopedExport), typeof(TestScopedExport) })
                .Build();

            ITestScopedExport exportScope1;
            using (var scopedContext = container.CreateScope())
            {
                exportScope1 = scopedContext.Resolve<ITestScopedExport>();
                Assert.IsInstanceOf<TestScopedExport>(exportScope1);

                var export = scopedContext.Resolve<ITestScopedExport>();
                Assert.AreSame(exportScope1, export);
            }

            using (var scopedContext2 = container.CreateScope())
            {
                var export2 = scopedContext2.Resolve<ITestScopedExport>();
                Assert.AreNotSame(exportScope1, export2);
            }
        }

        [Test]
        public void Resolve_AppService_no_constructor()
        {
            var builder = this.CreateInjectorBuilderWithStringLogger()
                .WithAssemblies(typeof(IInjectableFactory).Assembly)
                .WithParts(new[] { typeof(IConstructorAppService), typeof(NoInjectConstructorAppService) });
            Assert.Throws<NoConstructorsFoundException>(() => builder.Build());
        }

        [Test]
        public void Resolve_AppService_ambiguous_constructor()
        {
            var builder = this.CreateInjectorBuilderWithStringLogger();
            var container = builder
                .WithAssemblies(typeof(IInjectableFactory).Assembly)
                .WithParts(new[] { typeof(IConstructorAppService), typeof(AmbiguousInjectConstructorAppService) })
                .Build();
            Assert.Throws<DependencyResolutionException>(() => container.Resolve<IConstructorAppService>());
        }

        [Test]
        public void Resolve_AppService_largest_constructor()
        {
            var builder = this.CreateInjectorBuilderWithStringLogger();
            var container = builder
                .WithAssemblies(typeof(IInjectableFactory).Assembly)
                .WithParts(new[] { typeof(IConstructorAppService), typeof(LargestInjectConstructorAppService) })
                .Build();

            var component = container.Resolve<IConstructorAppService>();
            Assert.IsNotNull(component);
        }

        [Test]
        public void Resolve_AppService_multiple_constructor()
        {
            var builder = this.CreateInjectorBuilderWithStringLogger();
            Assert.Throws<InjectionException>(() => builder
                .WithAssemblies(typeof(IInjectableFactory).Assembly)
                .WithParts(new[] { typeof(IConstructorAppService), typeof(MultipleInjectConstructorAppService) })
                .Build());
        }

        [Test]
        public void Resolve_AppService_default_constructor()
        {
            var builder = this.CreateInjectorBuilderWithStringLogger();
            var container = builder
                .WithAssemblies(typeof(IInjectableFactory).Assembly)
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
                .WithAssemblies(typeof(IInjectableFactory).Assembly)
                .WithParts(new[] { typeof(IConstructorAppService), typeof(SingleConstructorAppService) })
                .Build();

            var export = container.Resolve<IConstructorAppService>();

            Assert.IsInstanceOf<SingleConstructorAppService>(export);
        }

        [Test]
        public async Task CreateInjector_instance_registration()
        {
            var registrar = new TestAppServiceInfosProvider(
                new List<(Type contractDeclarationType, IAppServiceInfo appServiceInfo)>
                {
                    (typeof(string), new AppServiceInfo(typeof(string), "123")),
                });

            var builder = this.CreateInjectorBuilder(ctx => ctx.AppServiceInfosProviders.Add(registrar));
            var mockAppRuntime = builder.AmbientServices.GetAppRuntime();

            mockAppRuntime.GetAppAssemblies()
                .Returns(new[] { typeof(ILogger).Assembly, typeof(AutofacServiceProvider).Assembly });

            var container = builder.Build().BuildWithAutofac();

            var instance = container.Resolve<string>();
            Assert.AreEqual("123", instance);
        }

        [Test]
        public async Task CreateInjector_instance_factory_registration()
        {
            var registrar = new TestAppServiceInfosProvider(
                new List<(Type contractDeclarationType, IAppServiceInfo appServiceInfo)>
                {
                    (typeof(string), new AppServiceInfo(typeof(string), injector => "123")),
                });

            var builder = this.CreateInjectorBuilder(ctx => ctx.AppServiceInfosProviders.Add(registrar));
            var mockPlatformManager = ambientServices.GetAppRuntime();

            mockPlatformManager.GetAppAssemblies()
                .Returns(new[] { typeof(ILogger).Assembly, typeof(AutofacServiceProvider).Assembly });

            var container = factory.Build();

            var instance = container.Resolve<string>();
            Assert.AreEqual("123", instance);
        }

        private class TestAppServiceInfosProvider : IAppServiceInfosProvider
        {
            private readonly IEnumerable<ContractDeclaration> serviceInfos;

            public TestAppServiceInfosProvider(IEnumerable<(Type contractDeclarationType, IAppServiceInfo appServiceInfo)> serviceInfos)
            {
                this.serviceInfos = serviceInfos.Select(si => new ContractDeclaration(si.contractDeclarationType, si.appServiceInfo)).ToList();
            }

            public IEnumerable<ContractDeclaration> GetAppServiceContracts() => this.serviceInfos;
        }

        private IAppServiceCollectionBuilder CreateInjectorBuilder(Action<IAppServiceCollectionBuilder>? config = null)
        {
            var mockLoggerManager = Substitute.For<ILogManager>();
            var mockAppRuntime = Substitute.For<IAppRuntime>();

            var ambientServices = this.CreateAmbientServices()
                .Add(mockLoggerManager)
                .Add(mockAppRuntime);
            var builder = new AppServiceCollectionBuilder(ambientServices);
            config?.Invoke(builder);
            return builder;
        }

        private IAppServiceCollectionBuilder CreateInjectorBuilderWithStringLogger()
        {
            var builder = this.CreateInjectorBuilder();

            var mockLoggerManager = builder.AmbientServices.GetServiceInstance<ILogManager>();
            mockLoggerManager.GetLogger(Arg.Any<string>()).Returns(Substitute.For<ILogger>());

            return builder;
        }

        // [Export]
        public class ComposedTestLogConsumer
        {
            public ComposedTestLogConsumer(ILogger<ComposedTestLogConsumer> logger)
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

        public interface IConverter { }

        [SingletonAppServiceContract(ContractType = typeof(IConverter))]
        public interface IConverter<TSource, TTarget> : IConverter { }

        [ProcessingPriority(100)]
        public class StringToIntConverter : IConverter<string, int> { }

        public class SecondStringToIntConverter : IConverter<string, int> { }

        [ScopedAppServiceContract()]
        public interface ITestMyScopedExport { }

        public class TestMyScopedExport : ITestMyScopedExport { }

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
            public SingleConstructorAppService(IServiceProvider injectionContainer)
            {
            }
        }

        public class AmbiguousInjectConstructorAppService : IConstructorAppService
        {
            public AmbiguousInjectConstructorAppService(IAmbientServices ambientServices)
            {
            }

            public AmbiguousInjectConstructorAppService(IServiceProvider injectionContainer)
            {
            }
        }

        public class LargestInjectConstructorAppService : IConstructorAppService
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="LargestInjectConstructorAppService"/> class.
            /// </summary>
            public LargestInjectConstructorAppService()
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="LargestInjectConstructorAppService"/> class.
            /// </summary>
            /// <param name="injectionContainer">
            /// The injector.
            /// </param>
            public LargestInjectConstructorAppService(IServiceProvider injectionContainer)
            {
            }
        }

        public class NoInjectConstructorAppService : IConstructorAppService
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="NoInjectConstructorAppService"/> class.
            /// </summary>
            private NoInjectConstructorAppService()
            {
            }
        }

        public class ExportedClass
        {
        }

        public class ExportedClassWithFakeDependency : ExportedClass
        {
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
