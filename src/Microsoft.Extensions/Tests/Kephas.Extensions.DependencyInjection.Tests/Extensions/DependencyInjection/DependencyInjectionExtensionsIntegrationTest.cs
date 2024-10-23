// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DependencyInjectionExtensionsIntegrationTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Reflection;
using Kephas.DependencyInjection;
using Kephas.DependencyInjection.Metadata;
using Kephas.DependencyInjection.Reflection;
using Kephas.DependencyInjection.ServiceProviders;
using Kephas.Extensions.DependencyInjection;
using Kephas.Services;
using Kephas.Services.Builder;
using Kephas.Services.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Kephas.Tests.Extensions.DependencyInjection;

public class DependencyInjectionExtensionsIntegrationTest
{
    [Fact]
    public void CreateInjector_simple_ambient_services_exported_no_assemblies()
    {
        var services = new AppServiceCollection()
            .Add(() => Substitute.For<ILogManager>(), b => b.Singleton())
            .Add(() => Substitute.For<IAppRuntime>(), b => b.Singleton());
        var container = CreateServicesBuilder(services)
            .AddAssemblies(typeof(AppServiceCollection).Assembly)
            .BuildServiceProvider();

        var loggerManager = container.GetRequiredService<ILogManager>();
        Assert.Equal(services.GetServiceInstance<ILogManager>(), loggerManager);

        var appRuntime = container.GetRequiredService<IAppRuntime>();
        Assert.Equal(services.GetServiceInstance<IAppRuntime>(), appRuntime);
    }

    [Fact]
    public void Resolve_AppService_Singleton()
    {
        var builder = CreateServicesBuilderWithStringLogger();
        var container = builder
            .AddAssemblies(typeof(IServiceFactory).Assembly)
            .AddTypes(typeof(ITestAppService), typeof(TestAppService))
            .BuildServiceProvider();

        var exported = container.GetRequiredService<ITestAppService>();
        var secondExported = container.GetRequiredService<ITestAppService>();

        Assert.Same(exported, secondExported);
    }

    [Fact]
    public void Resolve_AppService_Single_Success()
    {
        var builder = CreateServicesBuilderWithStringLogger();
        var container = builder
            .AddAssemblies(typeof(IServiceFactory).Assembly)
            .AddTypes(typeof(ITestAppService), typeof(TestAppService))
            .BuildServiceProvider();

        var exported = container.GetRequiredService<ITestAppService>();

        Assert.IsType<TestAppService>(exported);
    }

    [Fact]
    public void Resolve_AppService_Single_Override_Success()
    {
        var builder = CreateServicesBuilderWithStringLogger();
        var container = builder
            .AddAssemblies(typeof(IServiceFactory).Assembly)
            .AddTypes(typeof(ITestAppService), typeof(TestAppService), typeof(TestOverrideAppService))
            .BuildServiceProvider();

        var exported = container.GetRequiredService<ITestAppService>();

        Assert.IsType<TestOverrideAppService>(exported);
    }

    [Fact]
    public void ResolveMany_AppService_Multiple_Singleton()
    {
        var builder = CreateServicesBuilderWithStringLogger();
        var container = builder
            .AddAssemblies(typeof(IServiceFactory).Assembly)
            .AddTypes(typeof(ITestMultiAppService), typeof(TestMultiAppService1), typeof(TestMultiAppService2))
            .BuildServiceProvider();

        var exports = container.GetRequiredService<IEnumerable<ITestMultiAppService>>().ToList();
        var exports2 = container.GetRequiredService<IEnumerable<ITestMultiAppService>>().ToList();

        Assert.Same(exports[0], exports2[0]);
        Assert.Same(exports[1], exports2[1]);
    }

    [Fact]
    public void ResolveMany_AppService_Multiple_Preserve_Order_types()
    {
        var builder = CreateServicesBuilderWithStringLogger();
        var container = builder
            .AddTypes(typeof(ITestMultiAppService), typeof(TestMultiAppService1), typeof(TestMultiAppService2))
            .BuildServiceProvider();

        var exports = container.GetRequiredService<IEnumerable<ITestMultiAppService>>().ToList();

        Assert.Equal(2, exports.Count);
        Assert.IsType<TestMultiAppService1>(exports[0]);
        Assert.IsType<TestMultiAppService2>(exports[1]);
    }

    [Fact]
    public void ResolveMany_AppService_Multiple_Preserve_Order_mixed()
    {
        var builder = CreateServicesBuilderWithStringLogger();
        var appServices = builder.Build();
        appServices.Add(typeof(ITestMultiAppService), _ => new TestMultiAppService1(), b => b.Transient());
        appServices.Add(Substitute.For<ITestMultiAppService>());
        appServices.Add(typeof(ITestMultiAppService), typeof(TestMultiAppService2));
        var container = appServices.BuildServiceProvider();

        var exports = container.GetRequiredService<IEnumerable<ITestMultiAppService>>().ToList();

        Assert.Equal(3, exports.Count);
        Assert.IsType<TestMultiAppService1>(exports[0]);
        Assert.True(exports[1].GetType().Name.Contains("Proxy"));
        Assert.IsType<TestMultiAppService2>(exports[2]);
    }

    [Fact]
    public void ResolveMany_AppService_IExportFactory_Success()
    {
        var builder = CreateServicesBuilderWithStringLogger();
        var container = builder
            .AddAssemblies(typeof(IServiceFactory).Assembly)
            .AddTypes(typeof(ITestMultiAppService), typeof(ITestMultiAppServiceConsumer), typeof(TestMultiAppService1), typeof(TestMultiAppService2), typeof(TestMultiAppServiceConsumer))
            .BuildServiceProvider();

        var export = (TestMultiAppServiceConsumer)container.GetRequiredService<ITestMultiAppServiceConsumer>();

        Assert.Equal(2, export.Factories.Count());
        Assert.Equal(2, export.MetadataFactories.Count());
    }

    [Fact]
    public void Resolve_AppService_generic_export()
    {
        var builder = CreateServicesBuilderWithStringLogger();
        var container = builder
            .AddTypes(typeof(ITestGenericExport<>), typeof(TestGenericExport))
            .BuildServiceProvider();

        var export = container.GetRequiredService<ITestGenericExport<string>>();
        Assert.IsType<TestGenericExport>(export);
    }

    [Fact]
    public void Resolve_AppService_generic_export_with_non_generic_contract()
    {
        var builder = CreateServicesBuilderWithStringLogger();
        var container = builder
            .AddAssemblies(typeof(IServiceFactory).Assembly)
            .AddTypes(typeof(ITestGenericWithNonGenericExport), typeof(ITestGenericWithNonGenericExport<>), typeof(TestGenericWithNonGenericExport))
            .BuildServiceProvider();

        var export = container.GetRequiredService<ITestGenericWithNonGenericExport>();
        Assert.IsType<TestGenericWithNonGenericExport>(export);
    }

    [Fact]
    public void Resolve_AppService_with_injection_default_constructor()
    {
        var appServices = new AppServiceCollection();
        var builder = CreateServicesBuilderWithStringLogger(
            appServices,
            settings: new AppServicesSettings { GetServiceAssemblies = Array.Empty<Assembly> });
        appServices.Add(new ServiceInfo(typeof(ExportedClassWithFakeDependency), typeof(ExportedClassWithFakeDependency)));

        var container = builder.BuildServiceProvider();
        var exported = container.GetRequiredService<ExportedClassWithFakeDependency>();

        Assert.NotNull(exported);
        Assert.IsType<ExportedClassWithFakeDependency>(exported);
        Assert.Null(exported.Dependency);
    }

    [Fact]
    public void Resolve_AppService_with_injection_constructor()
    {
        var appServices = new AppServiceCollection();
        var builder = CreateServicesBuilderWithStringLogger(
            appServices,
            settings: new AppServicesSettings { GetServiceAssemblies = Array.Empty<Assembly> });
        appServices.Add(new ServiceInfo(typeof(ExportedClass), typeof(ExportedClass)));
        appServices.Add(new ServiceInfo(typeof(ExportedClassWithFakeDependency), typeof(ExportedClassWithFakeDependency)));

        var container = builder.BuildServiceProvider();
        var exported = container.GetRequiredService<ExportedClassWithFakeDependency>();

        Assert.NotNull(exported);
        Assert.IsType<ExportedClassWithFakeDependency>(exported);
        Assert.NotNull(exported.Dependency);
    }

    [Fact]
    public void Resolve_ScopedAppService_no_scope()
    {
        var builder = CreateServicesBuilderWithStringLogger();
        var container = builder
            .AddAssemblies(typeof(IServiceFactory).Assembly)
            .AddTypes(typeof(ITestScopedExport), typeof(TestScopedExport))
            .BuildServiceProvider();

        Assert.IsType<TestScopedExport>(container.GetRequiredService<ITestScopedExport>());
    }

    [Fact]
    public void Resolve_ScopedAppService_export()
    {
        var builder = CreateServicesBuilderWithStringLogger();
        var container = builder
            .AddAssemblies(typeof(IServiceFactory).Assembly)
            .AddTypes(typeof(ITestScopedExport), typeof(TestScopedExport))
            .BuildServiceProvider();

        var lifetimeScope = container;
        ITestScopedExport exportScope1;
        using (var scopedContext = lifetimeScope.CreateScope())
        {
            exportScope1 = scopedContext.ServiceProvider.GetRequiredService<ITestScopedExport>();
            Assert.IsType<TestScopedExport>(exportScope1);

            var export = scopedContext.ServiceProvider.GetRequiredService<ITestScopedExport>();
            Assert.Same(exportScope1, export);
        }

        using (var scopedContext2 = lifetimeScope.CreateScope())
        {
            var export2 = scopedContext2.ServiceProvider.GetRequiredService<ITestScopedExport>();
            Assert.NotSame(exportScope1, export2);
        }
    }

    [Fact]
    public void Resolve_AppService_no_constructor()
    {
        var builder = CreateServicesBuilderWithStringLogger()
            .AddAssemblies(typeof(IServiceFactory).Assembly)
            .AddTypes(typeof(IConstructorAppService), typeof(NoInjectConstructorAppService));
        Assert.Throws<InvalidOperationException>(() => builder.Build());
    }

    [Fact]
    public void Resolve_AppService_ambiguous_constructor()
    {
        var builder = CreateServicesBuilderWithStringLogger();
        var container = builder
            .AddAssemblies(typeof(IServiceFactory).Assembly)
            .AddTypes(typeof(IConstructorAppService), typeof(AmbiguousInjectConstructorAppService))
            .BuildServiceProvider();
        Assert.Throws<InvalidOperationException>(() => container.GetRequiredService<IConstructorAppService>());
    }

    [Fact]
    public void Resolve_AppService_largest_constructor()
    {
        var builder = CreateServicesBuilderWithStringLogger();
        var container = builder
            .AddAssemblies(typeof(IServiceFactory).Assembly)
            .AddTypes(typeof(IConstructorAppService), typeof(LargestInjectConstructorAppService))
            .BuildServiceProvider();

        var component = container.GetRequiredService<IConstructorAppService>();
        Assert.NotNull(component);
    }

    [Fact]
    public void Resolve_AppService_default_constructor()
    {
        var builder = CreateServicesBuilderWithStringLogger();
        var container = builder
            .AddAssemblies(typeof(IServiceFactory).Assembly)
            .AddTypes(typeof(IConstructorAppService), typeof(DefaultConstructorAppService))
            .BuildServiceProvider();

        var export = container.GetRequiredService<IConstructorAppService>();

        Assert.IsType<DefaultConstructorAppService>(export);
    }

    [Fact]
    public void Resolve_AppService_single_constructor()
    {
        var builder = CreateServicesBuilderWithStringLogger();
        var container = builder
            .AddAssemblies(typeof(IServiceFactory).Assembly)
            .AddTypes(typeof(IConstructorAppService), typeof(SingleConstructorAppService))
            .BuildServiceProvider();

        var export = container.GetRequiredService<IConstructorAppService>();

        Assert.IsType<SingleConstructorAppService>(export);
    }

    [Fact]
    public void CreateInjector_instance_registration()
    {
        var registrar = new TestServiceInfoProvider(
            new List<(Type contractDeclarationType, IServiceInfo appServiceInfo)>
            {
                (typeof(string), new ServiceInfo(typeof(string), "123")),
            });

        var builder = CreateServicesBuilder(
            settings: new AppServicesSettings { GetServiceAssemblies = Array.Empty<Assembly> },
            config: ctx => ctx
                .AddProviders(registrar)
                .AddAssemblies(typeof(ILogger).Assembly, typeof(FactoryService<,>).Assembly));

        var container = builder.BuildServiceProvider();

        var instance = container.GetRequiredService<string>();
        Assert.Equal("123", instance);
    }

    [Fact]
    public async Task CreateInjector_instance_factory_registration()
    {
        var registrar = new TestServiceInfoProvider(
            new List<(Type contractDeclarationType, IServiceInfo appServiceInfo)>
            {
                (typeof(string), new ServiceInfo(typeof(string), _ => "123")),
            });

        var builder = CreateServicesBuilder(config: ctx => ctx
            .AddProviders(registrar));

        var container = builder.BuildServiceProvider();

        var instance = container.GetRequiredService<string>();
        Assert.Equal("123", instance);
    }

    private class TestServiceInfoProvider : IServiceInfoProvider
    {
        private readonly IEnumerable<ServiceDeclarationInfo> serviceInfos;

        public TestServiceInfoProvider(IEnumerable<(Type contractDeclarationType, IServiceInfo appServiceInfo)> serviceInfos)
        {
            this.serviceInfos = serviceInfos.Select(si => new ServiceDeclarationInfo(si.contractDeclarationType, si.appServiceInfo)).ToList();
        }

        public IEnumerable<ServiceDeclarationInfo> GetServiceInfos() => serviceInfos;
    }

    private IAppServiceCollectionBuilder CreateServicesBuilder(
        IAppServiceCollection? appServices = null,
        Action<IAppServiceCollectionBuilder>? config = null,
        AppServicesSettings? settings = null)
    {
        var mockLoggerManager = Substitute.For<ILogManager>();
        var mockAppRuntime = Substitute.For<IAppRuntime>();

        appServices ??= new AppServiceCollection();
        appServices
            .Add(mockLoggerManager, b => b.Singleton())
            .Add(mockAppRuntime, b => b.Singleton());
        var builder = new AppServiceCollectionBuilder(appServices, settings);
        config?.Invoke(builder);
        return builder;
    }

    private IAppServiceCollectionBuilder CreateServicesBuilderWithStringLogger(
        IAppServiceCollection? appServices = null,
        AppServicesSettings? settings = null)
    {
        appServices ??= new AppServiceCollection();
        var builder = CreateServicesBuilder(appServices, settings: settings);

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

    [SingletonService]
    public interface ITestAppService { }

    [SingletonService(AllowMultiple = true)]
    public interface ITestMultiAppService { }

    [SingletonService]
    public interface ITestMultiAppServiceConsumer { }

    public class TestAppService : ITestAppService { }

    [Priority(Priority.High)]
    public class TestOverrideAppService : ITestAppService { }

    public class TestMultiAppService1 : ITestMultiAppService { }

    public class TestMultiAppService2 : ITestMultiAppService { }

    public class TestMultiAppServiceConsumer : ITestMultiAppServiceConsumer
    {
        public TestMultiAppServiceConsumer(
            IEnumerable<IFactory<ITestMultiAppService>> factories,
            IEnumerable<IFactory<ITestMultiAppService, ServiceMetadata>> metadataFactories)
        {
            Factories = factories;
            MetadataFactories = metadataFactories;
        }

        public IEnumerable<IFactory<ITestMultiAppService>> Factories { get; set; }

        public IEnumerable<IFactory<ITestMultiAppService, ServiceMetadata>> MetadataFactories { get; set; }
    }

    public interface IConverter { }

    [SingletonService(ServiceType = typeof(IConverter))]
    public interface IConverter<TSource, TTarget> : IConverter { }

    [Priority(100)]
    public class StringToIntConverter : IConverter<string, int> { }

    public class SecondStringToIntConverter : IConverter<string, int> { }

    [ScopedService]
    public interface ITestMyScopedExport { }

    public class TestMyScopedExport : ITestMyScopedExport { }

    [ScopedService]
    public interface ITestScopedExport { }

    public class TestScopedExport : ITestScopedExport { }

    [SingletonService]
    public interface ITestGenericExport<T> { }

    public class TestGenericExport : ITestGenericExport<string> { }

    public interface ITestGenericWithNonGenericExport { }

    [TransientService(ServiceType = typeof(ITestGenericWithNonGenericExport))]
    public interface ITestGenericWithNonGenericExport<T> : ITestGenericWithNonGenericExport { }

    public class TestGenericWithNonGenericExport : ITestGenericWithNonGenericExport<string> { }

    [SingletonService]
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