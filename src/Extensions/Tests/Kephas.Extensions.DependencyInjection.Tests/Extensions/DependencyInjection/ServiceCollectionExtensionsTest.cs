// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DependencyInjectionExtensionsTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using FluentAssertions;
using Kephas.DependencyInjection;
using Kephas.DependencyInjection.Metadata;
using Kephas.DependencyInjection.ServiceProviders;
using Kephas.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Kephas.Tests.Extensions.DependencyInjection;

public class ServiceCollectionExtensionsTest
{
    [Theory]
    [InlineData(typeof(IServiceDeclarationInfoCollection))]
    public void AddApplicationServices_default_services(Type serviceType)
    {
        var services = new ServiceCollection();
        var provider = services
            .AddApplicationServices(Test.GetIntegrationConfig())
            .BuildServiceProvider();
        var requiredService = provider.GetRequiredService(serviceType);
        requiredService.Should().BeSameAs(services.GetSingleton(serviceType, () => throw new Exception()));
    }
    
    [Fact]
    public void AddApplicationServices_defaults()
    {
        var builder = Test.CreateTestServiceInfoCollectionBuilder();
        var provider = builder.BuildServiceProvider();

        Assert.IsType<ServiceProvider>(provider);
    }

    [Fact]
    public void AddApplicationServices_with_open_generic_override()
    {
        var builder = Test.CreateTestServiceInfoCollectionBuilder()
            .AddTypes(typeof(IOpen<>), typeof(DefaultOpen<>), typeof(MoreOpen<>));
        var provider = builder.BuildServiceProvider();

        var moreOpen = provider.GetRequiredService<IOpen<int>>();
        Assert.IsType<MoreOpen<int>>(moreOpen);
    }

    [Fact]
    public void AddApplicationServices_with_open_generic_override_and_dependency()
    {
        var builder = Test.CreateTestServiceInfoCollectionBuilder()
            .AddTypes(typeof(IOpen<>), typeof(DefaultOpen<>), typeof(MoreOpenWithDependency<>), typeof(Dependency));
        var provider = builder.BuildServiceProvider();

        var moreOpen = provider.GetRequiredService<IOpen<int>>();
        Assert.IsType<MoreOpenWithDependency<int>>(moreOpen);
    }

    [Fact]
    public void AddApplicationServices_generic_export_with_ctor_dependency()
    {
        var services = new ServiceCollection();

        services.AddSingleton<Dependency>();
        services.AddSingleton(typeof(IOpen<>), typeof(MoreOpenWithDependency<>));

        using (var container = services.BuildServiceProvider())
        {
            var myService = container.GetRequiredService(typeof(IOpen<object>));
            Assert.IsType<MoreOpenWithDependency<object>>(myService);
        }
    }

    [SingletonService(AsOpenGeneric = true)]
    public interface IOpen<T> { }

    public class DefaultOpen<T> : IOpen<T> { }

    [Priority(Priority.High)]
    public class MoreOpen<T> : IOpen<T> { }

    [Priority(Priority.High)]
    public class MoreOpenWithDependency<T> : IOpen<T>
    {
        public MoreOpenWithDependency(Dependency dep)
        {
        }
    }

    [TransientService]
    public class Dependency { }
}