// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DependencyInjectionExtensionsTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using FluentAssertions;
using Kephas.DependencyInjection;
using Kephas.DependencyInjection.Metadata;
using Kephas.DependencyInjection.Reflection;
using Kephas.DependencyInjection.ServiceProviders;
using Kephas.Extensions.DependencyInjection;
using Kephas.Extensions.Tests.TestServices;
using Kephas.Runtime;
using Kephas.Testing;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Xunit;

namespace Kephas.Extensions.Tests.DependencyInjection;

public class ServiceCollectionExtensionsTest : TestDependencyInjectionBase
{
    [Fact]
    public void GetAppRuntime_ReturnsRegisteredAppRuntime()
    {
        var services = new ServiceCollection();
        var appRuntime = Substitute.For<IAppRuntime>();
        services.AddSingleton(appRuntime);

        var result = services.GetAppRuntime();

        result.Should().BeSameAs(appRuntime);
    }

    [Fact]
    public void GetAppRuntime_ThrowsException_WhenNotRegistered()
    {
        var services = new ServiceCollection();

        Action act = () => services.GetAppRuntime();

        act.Should().Throw<ServiceResolutionException>().WithMessage("*IAppRuntime*");
    }

    [Fact]
    public void GetAppRuntime_ReturnsDefaultAppRuntime_WhenNotRegistered()
    {
        var services = new ServiceCollection();

        var result = services.GetAppRuntime(defaultIfNotFound: true);

        result.Should().BeOfType<AppRuntime>();
    }

    [Fact]
    public void GetRegisteredSingletonService_ReturnsRegisteredService()
    {
        var services = new ServiceCollection();
        var service = Substitute.For<IAppRuntime>();
        services.AddSingleton(service);

        var result = services.GetRegisteredSingletonService<IAppRuntime>(() => null!);

        result.Should().BeSameAs(service);
    }

    [Fact]
    public void GetRegisteredSingletonService_ThrowsException_WhenNotRegistered()
    {
        var services = new ServiceCollection();

        Action act = () => services.GetRegisteredSingletonService<IAppRuntime>(() => throw new ServiceResolutionException("IAppRuntime not found"));

        act.Should().Throw<ServiceResolutionException>().WithMessage("*IAppRuntime*");
    }

    [Fact]
    public void GetRegisteredSingletonService_ReturnsDefaultService_WhenNotRegistered()
    {
        var services = new ServiceCollection();
        var defaultService = Substitute.For<IAppRuntime>();

        var result = services.GetRegisteredSingletonService(() => defaultService);

        result.Should().BeSameAs(defaultService);
    }

    [Theory]
    [InlineData(typeof(IServiceDeclarationInfoCollection))]
    [InlineData(typeof(IServiceInfoCollection))]
    [InlineData(typeof(IOpenGenericMetadataProvider))]
    [InlineData(typeof(IAppRuntime))]
    public void AddApplicationServices_default_services(Type serviceType)
    {
        var services = new ServiceCollection();
        var provider = services
            .AddApplicationServices(
                Test.GetIntegrationConfig(),
                Test.GetTestAppRuntime([]))
            .BuildServiceProvider();
        var requiredService = provider.GetRequiredService(serviceType);
        requiredService.Should()
            .BeSameAs(services.GetRegisteredSingletonService(serviceType, () => throw new Exception()));
    }

    [Theory]
    [InlineData(typeof(IEnumerable<IAppRuntime>))]
    [InlineData(typeof(IReadOnlyCollection<IAppRuntime>))]
    [InlineData(typeof(ICollection<IAppRuntime>))]
    [InlineData(typeof(IReadOnlyList<IAppRuntime>))]
    [InlineData(typeof(IList<IAppRuntime>))]
    public void AddApplicationServices_collections(Type collectionType)
    {
        var provider = Test.BuildEmptyTestServiceProvider();
        provider.GetService(collectionType).Should().BeAssignableTo<IEnumerable<IAppRuntime>>();
    }

    [Fact]
    public void AddApplicationServices_defaults()
    {
        Test.BuildEmptyTestServiceProvider()
            .Should().BeOfType<ServiceProvider>();
    }

    [Theory]
    [MemberData(nameof(GetAddApplicationServicesAssertions))]
    public void AddApplicationServices_success(IEnumerable<Type> serviceInfoTypes, Type serviceType,
        Type expectedImplementationType)
    {
        var sp = Test.BuildEmptyTestServiceProvider(
            builderConfig: b => b.AddTypes(serviceInfoTypes.ToArray()));

        sp.GetRequiredService(serviceType).Should().BeOfType(expectedImplementationType);
    }

    public static TheoryData<IEnumerable<Type>, Type, Type> GetAddApplicationServicesAssertions() =>
        new()
        {
            {
                [typeof(IOpen<>), typeof(DefaultOpen<>), typeof(MoreOpen<>)],
                typeof(IOpen<int>),
                typeof(MoreOpen<int>)
            },
            {
                [typeof(IOpen<>), typeof(DefaultOpen<>), typeof(MoreOpenWithDependency<>), typeof(Dependency)],
                typeof(IOpen<int>),
                typeof(MoreOpenWithDependency<int>)
            },
            {
                // Registration order matters
                [
                    typeof(IOpen<>), typeof(MoreOpenWithDependency<>), typeof(MoreOpen<>), typeof(DefaultOpen<>),
                    typeof(Dependency)
                ],
                typeof(IOpen<string>),
                typeof(MoreOpen<string>)
            },
        };

    [Fact]
    public void AddSingletonWithMetadata_open_generic_with_dependency()
    {
        var sp = new ServiceCollection()
            .AddInitialServices(Test.GetTestAppRuntime([]))
            .AddSingleton<Dependency>()
            .AddSingletonWithMetadata(typeof(IOpen<>), typeof(MoreOpenWithDependency<>))
            .BuildServiceProvider();

        var myService = sp.GetRequiredService(typeof(IOpen<object>));
        myService.Should().BeOfType<MoreOpenWithDependency<object>>();
    }

    [Fact]
    public void AddScopedWithMetadata_open_generic_with_dependency()
    {
        var sp = new ServiceCollection()
            .AddInitialServices(Test.GetTestAppRuntime([]))
            .AddScoped<Dependency>()
            .AddScopedWithMetadata(typeof(IOpen<>), typeof(MoreOpenWithDependency<>))
            .BuildServiceProvider();

        using var scope = sp.CreateScope();
        var myService = scope.ServiceProvider.GetRequiredService(typeof(IOpen<object>));
        myService.Should().BeOfType<MoreOpenWithDependency<object>>();
    }

    [Fact]
    public void AddTransientWithMetadata_open_generic_with_dependency()
    {
        var sp = new ServiceCollection()
            .AddInitialServices(Test.GetTestAppRuntime([]))
            .AddTransient<Dependency>()
            .AddTransientWithMetadata(typeof(IOpen<>), typeof(MoreOpenWithDependency<>))
            .BuildServiceProvider();

        var myService = sp.GetRequiredService(typeof(IOpen<object>));
        myService.Should().BeOfType<MoreOpenWithDependency<object>>();
    }

    [Fact]
    public void AddSingletonWithMetadata_non_declared_services()
    {
        var sp = new ServiceCollection()
            .AddInitialServices(Test.GetTestAppRuntime([]))
            .AddSingletonWithMetadata<INonDeclaredService, NonDeclaredService, NonDeclaredMetadata>(
                new Dictionary<string, object?> { { "Name", "Hello" } })
            .BuildServiceProvider();

        var nonDeclaredService = sp.GetRequiredService<INonDeclaredService>();
        nonDeclaredService.Should().BeOfType<NonDeclaredService>();

        var lazyNonDeclaredService = sp.GetRequiredService<Lazy<INonDeclaredService>>();
        lazyNonDeclaredService.Should().BeOfType<LazyService<INonDeclaredService, NonDeclaredService>>();
        lazyNonDeclaredService.Value.Should().BeOfType<NonDeclaredService>();

        var lazyNonDeclaredServiceWithMetadata =
            sp.GetRequiredService<Lazy<INonDeclaredService, NonDeclaredMetadata>>();
        lazyNonDeclaredServiceWithMetadata.Should()
            .BeOfType<LazyService<INonDeclaredService, NonDeclaredService, NonDeclaredMetadata>>();
        lazyNonDeclaredServiceWithMetadata.Value.Should().BeOfType<NonDeclaredService>();
        lazyNonDeclaredServiceWithMetadata.Metadata.Name.Should().Be("Hello");

        var factoryNonDeclaredService = sp.GetRequiredService<IFactory<INonDeclaredService>>();
        factoryNonDeclaredService.Should().BeOfType<FactoryService<INonDeclaredService, NonDeclaredService>>();
        factoryNonDeclaredService.GetService().Should().BeOfType<NonDeclaredService>();

        var factoryNonDeclaredServiceWithMetadata =
            sp.GetRequiredService<IFactory<INonDeclaredService, NonDeclaredMetadata>>();
        factoryNonDeclaredServiceWithMetadata.Should()
            .BeOfType<FactoryService<INonDeclaredService, NonDeclaredService, NonDeclaredMetadata>>();
        factoryNonDeclaredServiceWithMetadata.GetService().Should().BeOfType<NonDeclaredService>();
        factoryNonDeclaredServiceWithMetadata.Metadata.Name.Should().Be("Hello");

        lazyNonDeclaredService.Value.Should().BeSameAs(nonDeclaredService);
        lazyNonDeclaredServiceWithMetadata.Value.Should().BeSameAs(nonDeclaredService);
        factoryNonDeclaredService.GetService().Should().BeSameAs(nonDeclaredService);
        factoryNonDeclaredServiceWithMetadata.GetService().Should().BeSameAs(nonDeclaredService);

        using var spNewScope = sp.CreateScope();
        var spNew = spNewScope.ServiceProvider;
        spNew.GetRequiredService<INonDeclaredService>().Should().BeSameAs(nonDeclaredService);
        spNew.GetRequiredService<Lazy<INonDeclaredService>>().Value.Should().BeSameAs(nonDeclaredService);
        spNew.GetRequiredService<IFactory<INonDeclaredService>>().GetService().Should().BeSameAs(nonDeclaredService);
    }

    [Fact]
    public void AddScopedWithMetadata_non_declared_services()
    {
        var spRoot = new ServiceCollection()
            .AddInitialServices(Test.GetTestAppRuntime([]))
            .AddScopedWithMetadata<INonDeclaredService, NonDeclaredService, NonDeclaredMetadata>(
                new Dictionary<string, object?> { { "Name", "Hello" } })
            .BuildServiceProvider();

        using var spScope = spRoot.CreateScope();
        var sp = spScope.ServiceProvider;

        var nonDeclaredService = sp.GetRequiredService<INonDeclaredService>();
        nonDeclaredService.Should().BeOfType<NonDeclaredService>();

        var lazyNonDeclaredService = sp.GetRequiredService<Lazy<INonDeclaredService>>();
        lazyNonDeclaredService.Should().BeOfType<LazyService<INonDeclaredService, NonDeclaredService>>();
        lazyNonDeclaredService.Value.Should().BeOfType<NonDeclaredService>();

        var lazyNonDeclaredServiceWithMetadata =
            sp.GetRequiredService<Lazy<INonDeclaredService, NonDeclaredMetadata>>();
        lazyNonDeclaredServiceWithMetadata.Should()
            .BeOfType<LazyService<INonDeclaredService, NonDeclaredService, NonDeclaredMetadata>>();
        lazyNonDeclaredServiceWithMetadata.Value.Should().BeOfType<NonDeclaredService>();
        lazyNonDeclaredServiceWithMetadata.Metadata.Name.Should().Be("Hello");

        var factoryNonDeclaredService = sp.GetRequiredService<IFactory<INonDeclaredService>>();
        factoryNonDeclaredService.Should().BeOfType<FactoryService<INonDeclaredService, NonDeclaredService>>();
        factoryNonDeclaredService.GetService().Should().BeOfType<NonDeclaredService>();

        var factoryNonDeclaredServiceWithMetadata =
            sp.GetRequiredService<IFactory<INonDeclaredService, NonDeclaredMetadata>>();
        factoryNonDeclaredServiceWithMetadata.Should()
            .BeOfType<FactoryService<INonDeclaredService, NonDeclaredService, NonDeclaredMetadata>>();
        factoryNonDeclaredServiceWithMetadata.GetService().Should().BeOfType<NonDeclaredService>();
        factoryNonDeclaredServiceWithMetadata.Metadata.Name.Should().Be("Hello");

        lazyNonDeclaredService.Value.Should().BeSameAs(nonDeclaredService);
        lazyNonDeclaredServiceWithMetadata.Value.Should().BeSameAs(nonDeclaredService);
        factoryNonDeclaredService.GetService().Should().BeSameAs(nonDeclaredService);
        factoryNonDeclaredServiceWithMetadata.GetService().Should().BeSameAs(nonDeclaredService);

        using var spNewScope = spRoot.CreateScope();
        var spNew = spNewScope.ServiceProvider;
        spNew.GetRequiredService<INonDeclaredService>().Should().NotBeSameAs(nonDeclaredService);
        spNew.GetRequiredService<Lazy<INonDeclaredService>>().Value.Should().NotBeSameAs(nonDeclaredService);
        spNew.GetRequiredService<IFactory<INonDeclaredService>>().GetService().Should().NotBeSameAs(nonDeclaredService);
    }

    [Fact]
    public void AddTransientWithMetadata_non_declared_services()
    {
        var sp = new ServiceCollection()
            .AddInitialServices(Test.GetTestAppRuntime([]))
            .AddTransientWithMetadata<INonDeclaredService, NonDeclaredService, NonDeclaredMetadata>(
                new Dictionary<string, object?> { { "Name", "Hello" } })
            .BuildServiceProvider();

        var nonDeclaredService = sp.GetRequiredService<INonDeclaredService>();
        nonDeclaredService.Should().BeOfType<NonDeclaredService>();

        var lazyNonDeclaredService = sp.GetRequiredService<Lazy<INonDeclaredService>>();
        lazyNonDeclaredService.Should().BeOfType<LazyService<INonDeclaredService, NonDeclaredService>>();
        lazyNonDeclaredService.Value.Should().BeOfType<NonDeclaredService>();

        var lazyNonDeclaredServiceWithMetadata =
            sp.GetRequiredService<Lazy<INonDeclaredService, NonDeclaredMetadata>>();
        lazyNonDeclaredServiceWithMetadata.Should()
            .BeOfType<LazyService<INonDeclaredService, NonDeclaredService, NonDeclaredMetadata>>();
        lazyNonDeclaredServiceWithMetadata.Value.Should().BeOfType<NonDeclaredService>();
        lazyNonDeclaredServiceWithMetadata.Metadata.Name.Should().Be("Hello");

        var factoryNonDeclaredService = sp.GetRequiredService<IFactory<INonDeclaredService>>();
        factoryNonDeclaredService.Should().BeOfType<FactoryService<INonDeclaredService, NonDeclaredService>>();
        factoryNonDeclaredService.GetService().Should().BeOfType<NonDeclaredService>();

        var factoryNonDeclaredServiceWithMetadata =
            sp.GetRequiredService<IFactory<INonDeclaredService, NonDeclaredMetadata>>();
        factoryNonDeclaredServiceWithMetadata.Should()
            .BeOfType<FactoryService<INonDeclaredService, NonDeclaredService, NonDeclaredMetadata>>();
        factoryNonDeclaredServiceWithMetadata.GetService().Should().BeOfType<NonDeclaredService>();
        factoryNonDeclaredServiceWithMetadata.Metadata.Name.Should().Be("Hello");

        lazyNonDeclaredService.Value.Should().NotBeSameAs(nonDeclaredService);
        lazyNonDeclaredServiceWithMetadata.Value.Should().NotBeSameAs(nonDeclaredService);
        factoryNonDeclaredService.GetService().Should().NotBeSameAs(nonDeclaredService);
        factoryNonDeclaredServiceWithMetadata.GetService().Should().NotBeSameAs(nonDeclaredService);
    }

    [Fact]
    public void AddSingletonWithMetadata_non_declared_services_default_metadata()
    {
        var sp = new ServiceCollection()
            .AddInitialServices(Test.GetTestAppRuntime([]))
            .AddSingletonWithMetadata<INonDeclaredService, NonDeclaredService>(
                new Dictionary<string, object?> { { "Name", "Hello" } })
            .BuildServiceProvider();

        var nonDeclaredService = sp.GetRequiredService<INonDeclaredService>();
        nonDeclaredService.Should().BeOfType<NonDeclaredService>();

        var lazyNonDeclaredService = sp.GetRequiredService<Lazy<INonDeclaredService>>();
        lazyNonDeclaredService.Should().BeOfType<LazyService<INonDeclaredService, NonDeclaredService>>();
        lazyNonDeclaredService.Value.Should().BeOfType<NonDeclaredService>();

        var lazyNonDeclaredServiceWithMetadata = sp.GetRequiredService<Lazy<INonDeclaredService, ServiceMetadata>>();
        lazyNonDeclaredServiceWithMetadata.Should()
            .BeOfType<LazyService<INonDeclaredService, NonDeclaredService, ServiceMetadata>>();
        lazyNonDeclaredServiceWithMetadata.Value.Should().BeOfType<NonDeclaredService>();
        lazyNonDeclaredServiceWithMetadata.Metadata.Priority.Should().Be(Priority.Normal);

        var factoryNonDeclaredService = sp.GetRequiredService<IFactory<INonDeclaredService>>();
        factoryNonDeclaredService.Should().BeOfType<FactoryService<INonDeclaredService, NonDeclaredService>>();
        factoryNonDeclaredService.GetService().Should().BeOfType<NonDeclaredService>();

        var factoryNonDeclaredServiceWithMetadata =
            sp.GetRequiredService<IFactory<INonDeclaredService, ServiceMetadata>>();
        factoryNonDeclaredServiceWithMetadata.Should()
            .BeOfType<FactoryService<INonDeclaredService, NonDeclaredService, ServiceMetadata>>();
        factoryNonDeclaredServiceWithMetadata.GetService().Should().BeOfType<NonDeclaredService>();
        factoryNonDeclaredServiceWithMetadata.Metadata.Priority.Should().Be(Priority.Normal);

        lazyNonDeclaredService.Value.Should().BeSameAs(nonDeclaredService);
        lazyNonDeclaredServiceWithMetadata.Value.Should().BeSameAs(nonDeclaredService);
        factoryNonDeclaredService.GetService().Should().BeSameAs(nonDeclaredService);
        factoryNonDeclaredServiceWithMetadata.GetService().Should().BeSameAs(nonDeclaredService);

        using var spNewScope = sp.CreateScope();
        var spNew = spNewScope.ServiceProvider;
        spNew.GetRequiredService<INonDeclaredService>().Should().BeSameAs(nonDeclaredService);
        spNew.GetRequiredService<Lazy<INonDeclaredService>>().Value.Should().BeSameAs(nonDeclaredService);
        spNew.GetRequiredService<IFactory<INonDeclaredService>>().GetService().Should().BeSameAs(nonDeclaredService);
    }

    [Fact]
    public void AddScopedWithMetadata_non_declared_services_default_metadata()
    {
        var spRoot = new ServiceCollection()
            .AddInitialServices(Test.GetTestAppRuntime([]))
            .AddScopedWithMetadata<INonDeclaredService, NonDeclaredService>(
                new Dictionary<string, object?> { { "Name", "Hello" } })
            .BuildServiceProvider();

        using var spScope = spRoot.CreateScope();
        var sp = spScope.ServiceProvider;

        var nonDeclaredService = sp.GetRequiredService<INonDeclaredService>();
        nonDeclaredService.Should().BeOfType<NonDeclaredService>();

        var lazyNonDeclaredService = sp.GetRequiredService<Lazy<INonDeclaredService>>();
        lazyNonDeclaredService.Should().BeOfType<LazyService<INonDeclaredService, NonDeclaredService>>();
        lazyNonDeclaredService.Value.Should().BeOfType<NonDeclaredService>();

        var lazyNonDeclaredServiceWithMetadata = sp.GetRequiredService<Lazy<INonDeclaredService, ServiceMetadata>>();
        lazyNonDeclaredServiceWithMetadata.Should()
            .BeOfType<LazyService<INonDeclaredService, NonDeclaredService, ServiceMetadata>>();
        lazyNonDeclaredServiceWithMetadata.Value.Should().BeOfType<NonDeclaredService>();
        lazyNonDeclaredServiceWithMetadata.Metadata.Priority.Should().Be(Priority.Normal);

        var factoryNonDeclaredService = sp.GetRequiredService<IFactory<INonDeclaredService>>();
        factoryNonDeclaredService.Should().BeOfType<FactoryService<INonDeclaredService, NonDeclaredService>>();
        factoryNonDeclaredService.GetService().Should().BeOfType<NonDeclaredService>();

        var factoryNonDeclaredServiceWithMetadata =
            sp.GetRequiredService<IFactory<INonDeclaredService, ServiceMetadata>>();
        factoryNonDeclaredServiceWithMetadata.Should()
            .BeOfType<FactoryService<INonDeclaredService, NonDeclaredService, ServiceMetadata>>();
        factoryNonDeclaredServiceWithMetadata.GetService().Should().BeOfType<NonDeclaredService>();
        factoryNonDeclaredServiceWithMetadata.Metadata.Priority.Should().Be(Priority.Normal);

        lazyNonDeclaredService.Value.Should().BeSameAs(nonDeclaredService);
        lazyNonDeclaredServiceWithMetadata.Value.Should().BeSameAs(nonDeclaredService);
        factoryNonDeclaredService.GetService().Should().BeSameAs(nonDeclaredService);
        factoryNonDeclaredServiceWithMetadata.GetService().Should().BeSameAs(nonDeclaredService);

        using var spNewScope = spRoot.CreateScope();
        var spNew = spNewScope.ServiceProvider;
        spNew.GetRequiredService<INonDeclaredService>().Should().NotBeSameAs(nonDeclaredService);
        spNew.GetRequiredService<Lazy<INonDeclaredService>>().Value.Should().NotBeSameAs(nonDeclaredService);
        spNew.GetRequiredService<IFactory<INonDeclaredService>>().GetService().Should().NotBeSameAs(nonDeclaredService);
    }

    [Fact]
    public void AddTransientWithMetadata_non_declared_services_default_metadata()
    {
        var sp = new ServiceCollection()
            .AddInitialServices(Test.GetTestAppRuntime([]))
            .AddTransientWithMetadata<INonDeclaredService, NonDeclaredService>(
                new Dictionary<string, object?> { { "Name", "Hello" } })
            .BuildServiceProvider();

        var nonDeclaredService = sp.GetRequiredService<INonDeclaredService>();
        nonDeclaredService.Should().BeOfType<NonDeclaredService>();

        var lazyNonDeclaredService = sp.GetRequiredService<Lazy<INonDeclaredService>>();
        lazyNonDeclaredService.Should().BeOfType<LazyService<INonDeclaredService, NonDeclaredService>>();
        lazyNonDeclaredService.Value.Should().BeOfType<NonDeclaredService>();

        var lazyNonDeclaredServiceWithMetadata = sp.GetRequiredService<Lazy<INonDeclaredService, ServiceMetadata>>();
        lazyNonDeclaredServiceWithMetadata.Should()
            .BeOfType<LazyService<INonDeclaredService, NonDeclaredService, ServiceMetadata>>();
        lazyNonDeclaredServiceWithMetadata.Value.Should().BeOfType<NonDeclaredService>();
        lazyNonDeclaredServiceWithMetadata.Metadata.Priority.Should().Be(Priority.Normal);

        var factoryNonDeclaredService = sp.GetRequiredService<IFactory<INonDeclaredService>>();
        factoryNonDeclaredService.Should().BeOfType<FactoryService<INonDeclaredService, NonDeclaredService>>();
        factoryNonDeclaredService.GetService().Should().BeOfType<NonDeclaredService>();

        var factoryNonDeclaredServiceWithMetadata =
            sp.GetRequiredService<IFactory<INonDeclaredService, ServiceMetadata>>();
        factoryNonDeclaredServiceWithMetadata.Should()
            .BeOfType<FactoryService<INonDeclaredService, NonDeclaredService, ServiceMetadata>>();
        factoryNonDeclaredServiceWithMetadata.GetService().Should().BeOfType<NonDeclaredService>();
        factoryNonDeclaredServiceWithMetadata.Metadata.Priority.Should().Be(Priority.Normal);

        lazyNonDeclaredService.Value.Should().NotBeSameAs(nonDeclaredService);
        lazyNonDeclaredServiceWithMetadata.Value.Should().NotBeSameAs(nonDeclaredService);
        factoryNonDeclaredService.GetService().Should().NotBeSameAs(nonDeclaredService);
        factoryNonDeclaredServiceWithMetadata.GetService().Should().NotBeSameAs(nonDeclaredService);
    }

#if NET6_0
    [Fact]
    public void AddSingletonWithMetadata_non_declared_factory()
    {
        var registration = () => new ServiceCollection()
            .AddInitialServices(Test.GetTestAppRuntime([]))
            .AddSingletonWithMetadata<NonDeclaredService, NonDeclaredMetadata>(
                s => new NonDeclaredService(),
                new Dictionary<string, object?> { { "Name", "Hello" } });
        registration.Should().Throw<NotSupportedException>();
    }
#else
    [Fact]
    public void AddSingletonWithMetadata_non_declared_factory()
    {
        var sp = new ServiceCollection()
            .AddInitialServices(Test.GetTestAppRuntime([]))
            .AddSingletonWithMetadata<NonDeclaredService, NonDeclaredMetadata>(
                sp => new NonDeclaredService(),
                new Dictionary<string, object?> { { "Name", "Hello" } })
            .BuildServiceProvider();

        var nonDeclaredService = sp.GetRequiredService<NonDeclaredService>();
        nonDeclaredService.Should().BeOfType<NonDeclaredService>();
        
        var lazyNonDeclaredService = sp.GetRequiredService<Lazy<NonDeclaredService>>();
        lazyNonDeclaredService.Should().BeOfType<Lazy<NonDeclaredService>>();
        lazyNonDeclaredService.Value.Should().BeOfType<NonDeclaredService>();

        var lazyNonDeclaredServiceWithMetadata = sp.GetRequiredService<Lazy<NonDeclaredService, NonDeclaredMetadata>>();
        lazyNonDeclaredServiceWithMetadata.Should().BeOfType<Lazy<NonDeclaredService, NonDeclaredMetadata>>();
        lazyNonDeclaredServiceWithMetadata.Value.Should().BeOfType<NonDeclaredService>();
        lazyNonDeclaredServiceWithMetadata.Metadata.Name.Should().Be("Hello");

        var factoryNonDeclaredService = sp.GetRequiredService<IFactory<NonDeclaredService>>();
        factoryNonDeclaredService.Should().BeOfType<Factory<NonDeclaredService>>();
        factoryNonDeclaredService.GetService().Should().BeOfType<NonDeclaredService>();
        
        var factoryNonDeclaredServiceWithMetadata =
 sp.GetRequiredService<IFactory<NonDeclaredService, NonDeclaredMetadata>>();
        factoryNonDeclaredServiceWithMetadata.Should().BeOfType<Factory<NonDeclaredService, NonDeclaredMetadata>>();
        factoryNonDeclaredServiceWithMetadata.GetService().Should().BeOfType<NonDeclaredService>();
        factoryNonDeclaredServiceWithMetadata.Metadata.Name.Should().Be("Hello");
        
        lazyNonDeclaredService.Value.Should().BeSameAs(nonDeclaredService);
        lazyNonDeclaredServiceWithMetadata.Value.Should().BeSameAs(nonDeclaredService);
        factoryNonDeclaredService.GetService().Should().BeSameAs(nonDeclaredService);
        factoryNonDeclaredServiceWithMetadata.GetService().Should().BeSameAs(nonDeclaredService);
        
        using var spNewScope = sp.CreateScope();
        var spNew = spNewScope.ServiceProvider;
        spNew.GetRequiredService<NonDeclaredService>().Should().BeSameAs(nonDeclaredService);
        spNew.GetRequiredService<Lazy<NonDeclaredService>>().Value.Should().BeSameAs(nonDeclaredService);
        spNew.GetRequiredService<IFactory<NonDeclaredService>>().GetService().Should().BeSameAs(nonDeclaredService);
    }
#endif

#if NET6_0
    [Fact]
    public void AddScopedWithMetadata_non_declared_factory()
    {
        var registration = () => new ServiceCollection()
            .AddInitialServices(Test.GetTestAppRuntime([]))
            .AddScopedWithMetadata<NonDeclaredService, NonDeclaredMetadata>(
                s => new NonDeclaredService(),
                new Dictionary<string, object?> { { "Name", "Hello" } });
        registration.Should().Throw<NotSupportedException>();
    }
#else
    [Fact]
    public void AddScopedWithMetadata_non_declared_factory()
    {
        var spRoot = new ServiceCollection()
            .AddInitialServices(Test.GetTestAppRuntime([]))
            .AddScopedWithMetadata<NonDeclaredService, NonDeclaredMetadata>(
                s => new NonDeclaredService(),
                new Dictionary<string, object?> { { "Name", "Hello" } })
            .BuildServiceProvider();

        using var spScope = spRoot.CreateScope();
        var sp = spScope.ServiceProvider;

        var nonDeclaredService = sp.GetRequiredService<NonDeclaredService>();
        nonDeclaredService.Should().BeOfType<NonDeclaredService>();
        
        var lazyNonDeclaredService = sp.GetRequiredService<Lazy<NonDeclaredService>>();
        lazyNonDeclaredService.Should().BeOfType<Lazy<NonDeclaredService>>();
        lazyNonDeclaredService.Value.Should().BeOfType<NonDeclaredService>();

        var lazyNonDeclaredServiceWithMetadata = sp.GetRequiredService<Lazy<NonDeclaredService, NonDeclaredMetadata>>();
        lazyNonDeclaredServiceWithMetadata.Should().BeOfType<Lazy<NonDeclaredService, NonDeclaredMetadata>>();
        lazyNonDeclaredServiceWithMetadata.Value.Should().BeOfType<NonDeclaredService>();
        lazyNonDeclaredServiceWithMetadata.Metadata.Name.Should().Be("Hello");

        var factoryNonDeclaredService = sp.GetRequiredService<IFactory<NonDeclaredService>>();
        factoryNonDeclaredService.Should().BeOfType<Factory<NonDeclaredService>>();
        factoryNonDeclaredService.GetService().Should().BeOfType<NonDeclaredService>();
        
        var factoryNonDeclaredServiceWithMetadata =
 sp.GetRequiredService<IFactory<NonDeclaredService, NonDeclaredMetadata>>();
        factoryNonDeclaredServiceWithMetadata.Should().BeOfType<Factory<NonDeclaredService, NonDeclaredMetadata>>();
        factoryNonDeclaredServiceWithMetadata.GetService().Should().BeOfType<NonDeclaredService>();
        factoryNonDeclaredServiceWithMetadata.Metadata.Name.Should().Be("Hello");
        
        lazyNonDeclaredService.Value.Should().BeSameAs(nonDeclaredService);
        lazyNonDeclaredServiceWithMetadata.Value.Should().BeSameAs(nonDeclaredService);
        factoryNonDeclaredService.GetService().Should().BeSameAs(nonDeclaredService);
        factoryNonDeclaredServiceWithMetadata.GetService().Should().BeSameAs(nonDeclaredService);
        
        using var spNewScope = spRoot.CreateScope();
        var spNew = spNewScope.ServiceProvider;
        spNew.GetRequiredService<NonDeclaredService>().Should().NotBeSameAs(nonDeclaredService);
        spNew.GetRequiredService<Lazy<NonDeclaredService>>().Value.Should().NotBeSameAs(nonDeclaredService);
        spNew.GetRequiredService<IFactory<NonDeclaredService>>().GetService().Should().NotBeSameAs(nonDeclaredService);
    }
#endif

    [Fact]
    public void AddTransientWithMetadata_non_declared_factory()
    {
        var sp = new ServiceCollection()
            .AddInitialServices(Test.GetTestAppRuntime([]))
            .AddTransientWithMetadata<NonDeclaredService, NonDeclaredMetadata>(
                s => new NonDeclaredService(),
                new Dictionary<string, object?> { { "Name", "Hello" } })
            .BuildServiceProvider();

        var nonDeclaredService = sp.GetRequiredService<NonDeclaredService>();
        nonDeclaredService.Should().BeOfType<NonDeclaredService>();

        var lazyNonDeclaredService = sp.GetRequiredService<Lazy<NonDeclaredService>>();
        lazyNonDeclaredService.Should().BeOfType<Lazy<NonDeclaredService>>();
        lazyNonDeclaredService.Value.Should().BeOfType<NonDeclaredService>();

        var lazyNonDeclaredServiceWithMetadata = sp.GetRequiredService<Lazy<NonDeclaredService, NonDeclaredMetadata>>();
        lazyNonDeclaredServiceWithMetadata.Should().BeOfType<Lazy<NonDeclaredService, NonDeclaredMetadata>>();
        lazyNonDeclaredServiceWithMetadata.Value.Should().BeOfType<NonDeclaredService>();
        lazyNonDeclaredServiceWithMetadata.Metadata.Name.Should().Be("Hello");

        var factoryNonDeclaredService = sp.GetRequiredService<IFactory<NonDeclaredService>>();
        factoryNonDeclaredService.Should().BeOfType<Factory<NonDeclaredService>>();
        factoryNonDeclaredService.GetService().Should().BeOfType<NonDeclaredService>();

        var factoryNonDeclaredServiceWithMetadata =
            sp.GetRequiredService<IFactory<NonDeclaredService, NonDeclaredMetadata>>();
        factoryNonDeclaredServiceWithMetadata.Should().BeOfType<Factory<NonDeclaredService, NonDeclaredMetadata>>();
        factoryNonDeclaredServiceWithMetadata.GetService().Should().BeOfType<NonDeclaredService>();
        factoryNonDeclaredServiceWithMetadata.Metadata.Name.Should().Be("Hello");

        lazyNonDeclaredService.Value.Should().NotBeSameAs(nonDeclaredService);
        lazyNonDeclaredServiceWithMetadata.Value.Should().NotBeSameAs(nonDeclaredService);
        factoryNonDeclaredService.GetService().Should().NotBeSameAs(nonDeclaredService);
        factoryNonDeclaredServiceWithMetadata.GetService().Should().NotBeSameAs(nonDeclaredService);
    }
}