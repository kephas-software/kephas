// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DependencyInjectionExtensionsIntegrationTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Extensions.DependencyInjection;
using Kephas.Services;
using Kephas.Services.Builder;
using Kephas.Services.Reflection;
using NSubstitute;
using NUnit.Framework;

using Kephas.Services.Metadata;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Kephas.Tests.Extensions.DependencyInjection
{
    [TestFixture]
    public class DependencyInjectionExtensionsIntegrationTest
    {
        [Test]
        public void CreateInjector_simple_ambient_services_exported_no_assemblies()
        {
            var services = new AppServiceCollection()
                .Add<ILogManager>(() => Substitute.For<ILogManager>(), b => b.Singleton())
                .Add<IAppRuntime>(() => Substitute.For<IAppRuntime>(), b => b.Singleton());
            var container = CreateServicesBuilder(services)
                .AddAssemblies(typeof(AppServiceCollection).Assembly)
                .BuildServiceProvider();

            var loggerManager = container.Resolve<ILogManager>();
            Assert.AreEqual(services.GetServiceInstance<ILogManager>(), loggerManager);

            var appRuntime = container.Resolve<IAppRuntime>();
            Assert.AreEqual(services.GetServiceInstance<IAppRuntime>(), appRuntime);
        }

        [Test]
        public void Resolve_AppService_Singleton()
        {
            var builder = CreateAutofacServicesBuilderWithStringLogger();
            var container = builder
                .AddAssemblies(typeof(IServiceFactory).Assembly)
                .AddTypes(new[] { typeof(ITestAppService), typeof(TestAppService) })
                .BuildServiceProvider();

            var exported = container.Resolve<ITestAppService>();
            var secondExported = container.Resolve<ITestAppService>();

            Assert.AreSame(exported, secondExported);
        }

        [Test]
        public void Resolve_AppService_Single_Success()
        {
            var builder = CreateAutofacServicesBuilderWithStringLogger();
            var container = builder
                .AddAssemblies(typeof(IServiceFactory).Assembly)
                .AddTypes(new[] { typeof(ITestAppService), typeof(TestAppService) })
                .BuildServiceProvider();

            var exported = container.Resolve<ITestAppService>();

            Assert.IsInstanceOf<TestAppService>(exported);
        }

        [Test]
        public void Resolve_AppService_Single_Override_Success()
        {
            var builder = CreateAutofacServicesBuilderWithStringLogger();
            var container = builder
                .AddAssemblies(typeof(IServiceFactory).Assembly)
                .AddTypes(new[] { typeof(ITestAppService), typeof(TestAppService), typeof(TestOverrideAppService) })
                .BuildServiceProvider();

            var exported = container.Resolve<ITestAppService>();

            Assert.IsInstanceOf<TestOverrideAppService>(exported);
        }

        [Test]
        public void ResolveMany_AppService_Multiple_Singleton()
        {
            var builder = CreateAutofacServicesBuilderWithStringLogger();
            var container = builder
                .AddAssemblies(typeof(IServiceFactory).Assembly)
                .AddTypes(new[] { typeof(ITestMultiAppService), typeof(TestMultiAppService1), typeof(TestMultiAppService2) })
                .BuildServiceProvider();

            var exports = container.ResolveMany<ITestMultiAppService>().ToList();
            var exports2 = container.ResolveMany<ITestMultiAppService>().ToList();

            Assert.AreSame(exports[0], exports2[0]);
            Assert.AreSame(exports[1], exports2[1]);
        }

        [Test]
        public void ResolveMany_AppService_Multiple_Preserve_Order_types()
        {
            var builder = CreateAutofacServicesBuilderWithStringLogger();
            var container = builder
                .AddTypes(new[] { typeof(ITestMultiAppService), typeof(TestMultiAppService1), typeof(TestMultiAppService2) })
                .BuildServiceProvider();

            var exports = container.ResolveMany<ITestMultiAppService>().ToList();

            Assert.AreEqual(2, exports.Count);
            Assert.IsInstanceOf<TestMultiAppService1>(exports[0]);
            Assert.IsInstanceOf<TestMultiAppService2>(exports[1]);
        }

        [Test]
        public void ResolveMany_AppService_Multiple_Preserve_Order_mixed()
        {
            var builder = CreateAutofacServicesBuilderWithStringLogger();
            var appServices = builder.Build();
            appServices.Add(typeof(ITestMultiAppService), _ => new TestMultiAppService1(), b => b.Transient());
            appServices.Add(Substitute.For<ITestMultiAppService>());
            appServices.Add(typeof(ITestMultiAppService), typeof(TestMultiAppService2));
            var container = appServices.BuildServiceProvider();

            var exports = container.ResolveMany<ITestMultiAppService>().ToList();

            Assert.AreEqual(3, exports.Count);
            Assert.IsInstanceOf<TestMultiAppService1>(exports[0]);
            Assert.IsTrue(exports[1].GetType().Name.Contains("Proxy"));
            Assert.IsInstanceOf<TestMultiAppService2>(exports[2]);
        }

        [Test]
        public void ResolveMany_AppService_IExportFactory_Success()
        {
            var builder = CreateAutofacServicesBuilderWithStringLogger();
            var container = builder
                .AddAssemblies(typeof(IServiceFactory).Assembly)
                .AddTypes(new[] { typeof(ITestMultiAppService), typeof(ITestMultiAppServiceConsumer), typeof(TestMultiAppService1), typeof(TestMultiAppService2), typeof(TestMultiAppServiceConsumer) })
                .BuildServiceProvider();

            var export = (TestMultiAppServiceConsumer)container.Resolve<ITestMultiAppServiceConsumer>();

            Assert.AreEqual(2, export.Factories.Count());
            Assert.AreEqual(2, export.MetadataFactories.Count());
        }

        [Test]
        public void Resolve_AppService_generic_export()
        {
            var builder = CreateAutofacServicesBuilderWithStringLogger();
            var container = builder
                .AddTypes(new[] { typeof(ITestGenericExport<>), typeof(TestGenericExport) })
                .BuildServiceProvider();

            var export = container.Resolve<ITestGenericExport<string>>();
            Assert.IsInstanceOf<TestGenericExport>(export);
        }

        [Test]
        public void Resolve_AppService_generic_export_with_non_generic_contract()
        {
            var builder = CreateAutofacServicesBuilderWithStringLogger();
            var container = builder
                .AddAssemblies(typeof(IServiceFactory).Assembly)
                .AddTypes(new[] { typeof(ITestGenericWithNonGenericExport), typeof(ITestGenericWithNonGenericExport<>), typeof(TestGenericWithNonGenericExport) })
                .BuildServiceProvider();

            var export = container.Resolve<ITestGenericWithNonGenericExport>();
            Assert.IsInstanceOf<TestGenericWithNonGenericExport>(export);
        }

        [Test]
        public void Resolve_AppService_with_injection_constructor()
        {
            var appServices = new AppServiceCollection();
            var builder = CreateAutofacServicesBuilderWithStringLogger(appServices);
            appServices.Add(new AppServiceInfo(typeof(ExportedClass), typeof(ExportedClass)));
            appServices.Add(new AppServiceInfo(typeof(ExportedClassWithFakeDependency), typeof(ExportedClassWithFakeDependency)));

            var container = builder.BuildServiceProvider();
            var exported = container.GetRequiredService<ExportedClassWithFakeDependency>();

            Assert.IsNotNull(exported);
            Assert.IsInstanceOf<ExportedClassWithFakeDependency>(exported);
            Assert.IsNull(exported.Dependency);
        }

        [Test]
        public void Resolve_ScopedAppService_no_scope()
        {
            var builder = CreateAutofacServicesBuilderWithStringLogger();
            var container = builder
                .AddAssemblies(typeof(IServiceFactory).Assembly)
                .AddTypes(new[] { typeof(ITestScopedExport), typeof(TestScopedExport) })
                .BuildServiceProvider();

            Assert.IsInstanceOf<TestScopedExport>(container.Resolve<ITestScopedExport>());
        }

        [Test]
        public void Resolve_ScopedAppService_export()
        {
            var builder = CreateAutofacServicesBuilderWithStringLogger();
            var container = builder
                .AddAssemblies(typeof(IServiceFactory).Assembly)
                .AddTypes(typeof(ITestScopedExport), typeof(TestScopedExport))
                .BuildServiceProvider();

            var lifetimeScope = container;
            ITestScopedExport exportScope1;
            using (var scopedContext = lifetimeScope.CreateScope())
            {
                exportScope1 = scopedContext.ServiceProvider.Resolve<ITestScopedExport>();
                Assert.IsInstanceOf<TestScopedExport>(exportScope1);

                var export = scopedContext.ServiceProvider.Resolve<ITestScopedExport>();
                Assert.AreSame(exportScope1, export);
            }

            using (var scopedContext2 = lifetimeScope.CreateScope())
            {
                var export2 = scopedContext2.ServiceProvider.Resolve<ITestScopedExport>();
                Assert.AreNotSame(exportScope1, export2);
            }
        }

        [Test]
        public void Resolve_AppService_no_constructor()
        {
            var builder = CreateAutofacServicesBuilderWithStringLogger()
                .AddAssemblies(typeof(IServiceFactory).Assembly)
                .AddTypes(new[] { typeof(IConstructorAppService), typeof(NoInjectConstructorAppService) });
            Assert.Throws<InvalidOperationException>(() => builder.Build());
        }

        [Test]
        public void Resolve_AppService_ambiguous_constructor()
        {
            var builder = CreateAutofacServicesBuilderWithStringLogger();
            var container = builder
                .AddAssemblies(typeof(IServiceFactory).Assembly)
                .AddTypes(new[] { typeof(IConstructorAppService), typeof(AmbiguousInjectConstructorAppService) })
                .BuildServiceProvider();
            Assert.Throws<InvalidOperationException>(() => container.Resolve<IConstructorAppService>());
        }

        [Test]
        public void Resolve_AppService_largest_constructor()
        {
            var builder = CreateAutofacServicesBuilderWithStringLogger();
            var container = builder
                .AddAssemblies(typeof(IServiceFactory).Assembly)
                .AddTypes(new[] { typeof(IConstructorAppService), typeof(LargestInjectConstructorAppService) })
                .BuildServiceProvider();

            var component = container.Resolve<IConstructorAppService>();
            Assert.IsNotNull(component);
        }

        [Test]
        public void Resolve_AppService_default_constructor()
        {
            var builder = CreateAutofacServicesBuilderWithStringLogger();
            var container = builder
                .AddAssemblies(typeof(IServiceFactory).Assembly)
                .AddTypes(new[] { typeof(IConstructorAppService), typeof(DefaultConstructorAppService) })
                .BuildServiceProvider();

            var export = container.Resolve<IConstructorAppService>();

            Assert.IsInstanceOf<DefaultConstructorAppService>(export);
        }

        [Test]
        public void Resolve_AppService_single_constructor()
        {
            var builder = CreateAutofacServicesBuilderWithStringLogger();
            var container = builder
                .AddAssemblies(typeof(IServiceFactory).Assembly)
                .AddTypes(new[] { typeof(IConstructorAppService), typeof(SingleConstructorAppService) })
                .BuildServiceProvider();

            var export = container.Resolve<IConstructorAppService>();

            Assert.IsInstanceOf<SingleConstructorAppService>(export);
        }

        [Test]
        public void CreateInjector_instance_registration()
        {
            var registrar = new TestAppServiceInfoProvider(
                new List<(Type contractDeclarationType, IAppServiceInfo appServiceInfo)>
                {
                    (typeof(string), new AppServiceInfo(typeof(string), "123")),
                });

            var builder = CreateServicesBuilder(config: ctx => ctx
                .AddProviders(registrar)
                .AddAssemblies(typeof(ILogger).Assembly, typeof(FactoryService<,>).Assembly));

            var container = builder.BuildServiceProvider();

            var instance = container.Resolve<string>();
            Assert.AreEqual("123", instance);
        }

        [Test]
        public async Task CreateInjector_instance_factory_registration()
        {
            var registrar = new TestAppServiceInfoProvider(
                new List<(Type contractDeclarationType, IAppServiceInfo appServiceInfo)>
                {
                    (typeof(string), new AppServiceInfo(typeof(string), _ => "123")),
                });

            var builder = CreateServicesBuilder(config: ctx => ctx
                .AddProviders(registrar));

            var container = builder.BuildServiceProvider();

            var instance = container.Resolve<string>();
            Assert.AreEqual("123", instance);
        }

        private class TestAppServiceInfoProvider : IAppServiceInfoProvider
        {
            private readonly IEnumerable<ContractDeclaration> serviceInfos;

            public TestAppServiceInfoProvider(IEnumerable<(Type contractDeclarationType, IAppServiceInfo appServiceInfo)> serviceInfos)
            {
                this.serviceInfos = serviceInfos.Select(si => new ContractDeclaration(si.contractDeclarationType, si.appServiceInfo)).ToList();
            }

            public IEnumerable<ContractDeclaration> GetAppServiceContracts() => serviceInfos;
        }

        private IAppServiceCollectionBuilder CreateServicesBuilder(IAppServiceCollection? appServices = null, Action<IAppServiceCollectionBuilder>? config = null)
        {
            var mockLoggerManager = Substitute.For<ILogManager>();
            var mockAppRuntime = Substitute.For<IAppRuntime>();

            appServices ??= new AppServiceCollection();
            appServices
                .Add(mockLoggerManager, b => b.Singleton())
                .Add(mockAppRuntime, b => b.Singleton());
            var builder = new AppServiceCollectionBuilder(appServices);
            config?.Invoke(builder);
            return builder;
        }

        private IAppServiceCollectionBuilder CreateAutofacServicesBuilderWithStringLogger(IAppServiceCollection? appServices = null)
        {
            appServices ??= new AppServiceCollection();
            var builder = CreateServicesBuilder(appServices);

            var mockLoggerManager = appServices.GetServiceInstance<ILogManager>();
            mockLoggerManager.GetLogger(Arg.Any<string>()).Returns(Substitute.For<ILogger>());

            return builder;
        }

        // [Export]
        public class ComposedTestLogConsumer
        {
            public ComposedTestLogConsumer(ILogger<ComposedTestLogConsumer> logger)
            {
                Logger = logger;
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
                Factories = factories;
                MetadataFactories = metadataFactories;
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

        public interface IAppRuntime {}

        public interface ILogManager
        {
            ILogger GetLogger(string name);
        }

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
            public AmbiguousInjectConstructorAppService(IAppServiceCollection appServices)
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

        public class ExportedClassWithFakeDependency
        {
            public ExportedClassWithFakeDependency()
            {
            }

            public ExportedClassWithFakeDependency(ExportedClass dependency)
            {
                Dependency = dependency;
            }

            public ExportedClass Dependency { get; }
        }
    }
}
