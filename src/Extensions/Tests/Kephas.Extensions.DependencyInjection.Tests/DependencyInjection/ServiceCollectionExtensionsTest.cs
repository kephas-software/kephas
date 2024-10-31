// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DependencyInjectionExtensionsTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using FluentAssertions;
using Kephas.DependencyInjection.Reflection;
using Kephas.DependencyInjection.ServiceProviders;
using Kephas.Extensions.DependencyInjection;
using Kephas.Extensions.Tests.TestServices;
using Kephas.Runtime;
using Kephas.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Kephas.Extensions.Tests.DependencyInjection;

public class ServiceCollectionExtensionsTest : TestDependencyInjectionBase
{
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
        requiredService.Should().BeSameAs(services.GetSingleton(serviceType, () => throw new Exception()));
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
    public void AddApplicationServices_success(IEnumerable<Type> serviceInfoTypes, Type serviceType, Type expectedImplementationType)
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
                [typeof(IOpen<>), typeof(MoreOpenWithDependency<>), typeof(MoreOpen<>), typeof(DefaultOpen<>), typeof(Dependency)],
                typeof(IOpen<string>),
                typeof(MoreOpen<string>)
            },
        };

    [Fact]
    public void AddSingleton_open_generic_with_dependency()
    {
        using var sp = new ServiceCollection()
            .AddSingleton<Dependency>()
            .AddSingleton(typeof(IOpen<>), typeof(MoreOpenWithDependency<>))
            .BuildServiceProvider();

        var myService = sp.GetRequiredService(typeof(IOpen<object>));
        Assert.IsType<MoreOpenWithDependency<object>>(myService);
    }

    [Fact]
    public void AddScoped_open_generic_with_dependency()
    {
        using var sp = new ServiceCollection()
            .AddScoped<Dependency>()
            .AddScoped(typeof(IOpen<>), typeof(MoreOpenWithDependency<>))
            .BuildServiceProvider();

        using var scope = sp.CreateScope();
        var myService = scope.ServiceProvider.GetRequiredService(typeof(IOpen<object>));
        Assert.IsType<MoreOpenWithDependency<object>>(myService);
    }

    [Fact]
    public void AddTransient_open_generic_with_dependency()
    {
        using var sp = new ServiceCollection()
            .AddTransient<Dependency>()
            .AddTransient(typeof(IOpen<>), typeof(MoreOpenWithDependency<>))
            .BuildServiceProvider();

        var myService = sp.GetRequiredService(typeof(IOpen<object>));
        Assert.IsType<MoreOpenWithDependency<object>>(myService);
    }
}