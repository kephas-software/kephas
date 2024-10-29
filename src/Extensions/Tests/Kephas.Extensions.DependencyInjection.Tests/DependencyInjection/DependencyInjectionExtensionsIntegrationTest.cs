// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DependencyInjectionExtensionsIntegrationTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Reflection;
using System.Runtime.Loader;
using System.Text.RegularExpressions;
using Kephas.DependencyInjection;
using Kephas.DependencyInjection.Builder;
using Kephas.DependencyInjection.Metadata;
using Kephas.DependencyInjection.Reflection;
using Kephas.DependencyInjection.ServiceProviders;
using Kephas.Extensions.DependencyInjection;
using Kephas.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Kephas.Extensions.Tests.DependencyInjection;

public partial class DependencyInjectionExtensionsIntegrationTest
{
#if NET6_0
    private static readonly Regex AppAssemblyNameRegex = new("^Kephas\\.", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    private static Regex GetAppAssemblyNameRegex() => AppAssemblyNameRegex;
#else
    [GeneratedRegex("^Kephas\\.", RegexOptions.Compiled | RegexOptions.IgnoreCase)]
    private static partial Regex GetAppAssemblyNameRegex();
#endif

    [Fact]
    public void CreateInjector_simple_ambient_services_exported_no_assemblies()
    {
        var services = new ServiceInfoCollection()
            .Add(() => Substitute.For<IAppRuntime>(), b => b.Singleton());
        var container = CreateServicesBuilder(services)
            .AddAssemblies(typeof(ServiceInfoCollection).Assembly)
            .BuildServiceProvider();

        var appRuntime = container.GetRequiredService<IAppRuntime>();
        Assert.Equal(services.GetServiceInstance<IAppRuntime>(), appRuntime);
    }

    [Fact]
    public void Resolve_AppService_Singleton()
    {
        var builder = CreateServicesBuilder();
        var container = builder
            .AddAssemblies(typeof(IServiceInfoCollection).Assembly)
            .AddTypes(typeof(ITestAppService), typeof(TestAppService))
            .BuildServiceProvider();

        var exported = container.GetRequiredService<ITestAppService>();
        var secondExported = container.GetRequiredService<ITestAppService>();

        Assert.Same(exported, secondExported);
    }

    [Fact]
    public void Resolve_AppService_Single_Success()
    {
        var builder = CreateServicesBuilder();
        var container = builder
            .AddAssemblies(typeof(IServiceInfoCollection).Assembly)
            .AddTypes(typeof(ITestAppService), typeof(TestAppService))
            .BuildServiceProvider();

        var exported = container.GetRequiredService<ITestAppService>();

        Assert.IsType<TestAppService>(exported);
    }

    [Fact]
    public void Resolve_AppService_Single_Override_Success()
    {
        var builder = CreateServicesBuilder();
        var container = builder
            .AddAssemblies(typeof(IServiceInfoCollection).Assembly)
            .AddTypes(typeof(ITestAppService), typeof(TestAppService), typeof(TestOverrideAppService))
            .BuildServiceProvider();

        var exported = container.GetRequiredService<ITestAppService>();

        Assert.IsType<TestOverrideAppService>(exported);
    }

    [Fact]
    public void ResolveMany_AppService_Multiple_Singleton()
    {
        var builder = CreateServicesBuilder();
        var container = builder
            .AddAssemblies(typeof(IServiceInfoCollection).Assembly)
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
        var builder = CreateServicesBuilder();
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
        var builder = CreateServicesBuilder();
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
        var builder = CreateServicesBuilder();
        var container = builder
            .AddAssemblies(typeof(IServiceInfoCollection).Assembly)
            .AddTypes(typeof(ITestMultiAppService), typeof(ITestMultiAppServiceConsumer), typeof(TestMultiAppService1), typeof(TestMultiAppService2), typeof(TestMultiAppServiceConsumer))
            .BuildServiceProvider();

        var export = (TestMultiAppServiceConsumer)container.GetRequiredService<ITestMultiAppServiceConsumer>();

        Assert.Equal(2, export.Factories.Count());
        Assert.Equal(2, export.MetadataFactories.Count());
    }

    [Fact]
    public void Resolve_AppService_generic_export()
    {
        var builder = CreateServicesBuilder();
        var container = builder
            .AddTypes(typeof(ITestGenericExport<>), typeof(TestGenericExport))
            .BuildServiceProvider();

        var export = container.GetRequiredService<ITestGenericExport<string>>();
        Assert.IsType<TestGenericExport>(export);
    }

    [Fact]
    public void Resolve_AppService_generic_export_with_non_generic_contract()
    {
        var builder = CreateServicesBuilder();
        var container = builder
            .AddAssemblies(typeof(IServiceInfoCollection).Assembly)
            .AddTypes(typeof(ITestGenericWithNonGenericExport), typeof(ITestGenericWithNonGenericExport<>), typeof(TestGenericWithNonGenericExport))
            .BuildServiceProvider();

        var export = container.GetRequiredService<ITestGenericWithNonGenericExport>();
        Assert.IsType<TestGenericWithNonGenericExport>(export);
    }

    [Fact]
    public void Resolve_AppService_with_injection_default_constructor()
    {
        var appServices = new ServiceInfoCollection();
        var builder = CreateServicesBuilder(
            appServices,
            getAppAssemblies: Array.Empty<Assembly>);
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
        var appServices = new ServiceInfoCollection();
        var builder = CreateServicesBuilder(
            appServices,
            getAppAssemblies: Array.Empty<Assembly>);
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
        var builder = CreateServicesBuilder();
        var container = builder
            .AddAssemblies(typeof(IServiceInfoCollection).Assembly)
            .AddTypes(typeof(ITestScopedExport), typeof(TestScopedExport))
            .BuildServiceProvider();

        Assert.IsType<TestScopedExport>(container.GetRequiredService<ITestScopedExport>());
    }

    [Fact]
    public void Resolve_ScopedAppService_export()
    {
        var builder = CreateServicesBuilder();
        var container = builder
            .AddAssemblies(typeof(IServiceInfoCollection).Assembly)
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
        var builder = CreateServicesBuilder()
            .AddAssemblies(typeof(IServiceInfoCollection).Assembly)
            .AddTypes(typeof(IConstructorAppService), typeof(NoInjectConstructorAppService));
        Assert.Throws<InvalidOperationException>(() => builder.Build());
    }

    [Fact]
    public void Resolve_AppService_ambiguous_constructor()
    {
        var builder = CreateServicesBuilder();
        var container = builder
            .AddAssemblies(typeof(IServiceInfoCollection).Assembly)
            .AddTypes(typeof(IConstructorAppService), typeof(AmbiguousInjectConstructorAppService))
            .BuildServiceProvider();
        Assert.Throws<InvalidOperationException>(() => container.GetRequiredService<IConstructorAppService>());
    }

    [Fact]
    public void Resolve_AppService_largest_constructor()
    {
        var builder = CreateServicesBuilder();
        var container = builder
            .AddAssemblies(typeof(IServiceInfoCollection).Assembly)
            .AddTypes(typeof(IConstructorAppService), typeof(LargestInjectConstructorAppService))
            .BuildServiceProvider();

        var component = container.GetRequiredService<IConstructorAppService>();
        Assert.NotNull(component);
    }

    [Fact]
    public void Resolve_AppService_default_constructor()
    {
        var builder = CreateServicesBuilder();
        var container = builder
            .AddAssemblies(typeof(IServiceInfoCollection).Assembly)
            .AddTypes(typeof(IConstructorAppService), typeof(DefaultConstructorAppService))
            .BuildServiceProvider();

        var export = container.GetRequiredService<IConstructorAppService>();

        Assert.IsType<DefaultConstructorAppService>(export);
    }

    [Fact]
    public void Resolve_AppService_single_constructor()
    {
        var builder = CreateServicesBuilder();
        var container = builder
            .AddAssemblies(typeof(IServiceInfoCollection).Assembly)
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
            getAppAssemblies: Array.Empty<Assembly>,
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

    private ServiceInfoCollectionBuilder CreateServicesBuilder(
        IServiceInfoCollection? appServices = null,
        Action<ServiceInfoCollectionBuilder>? config = null,
        Func<IEnumerable<Assembly>>? getAppAssemblies = null)
    {
        var mockAppRuntime = Substitute.For<IAppRuntime>();

        appServices ??= new ServiceInfoCollection();
        appServices
            .Add(mockAppRuntime, b => b.Singleton());

        var builder = ServiceInfoCollectionBuilder.Create(appServices, getAppAssemblies ?? GetDefaultAppAssemblies);
        config?.Invoke(builder);
        return builder;
    }

    private static IEnumerable<Assembly> GetDefaultAppAssemblies() =>
        AssemblyLoadContext.Default.Assemblies
            .Where(a => GetAppAssemblyNameRegex().IsMatch(a.GetName()?.Name ?? string.Empty));

    [SingletonService]
    public interface ITestAppService { }

    [SingletonService]
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
        public AmbiguousInjectConstructorAppService(IServiceInfoCollection appServices)
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