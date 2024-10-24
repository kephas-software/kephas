// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DependencyInjectionExtensionsTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.DependencyInjection;
using Kephas.DependencyInjection.Builder;
using Kephas.DependencyInjection.Metadata;

namespace Kephas.Tests.Extensions.DependencyInjection;

using Kephas.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

public class DependencyInjectionExtensionsTest
{
    [Fact]
    public void BuildWithDependencyInjection_defaults()
    {
        var appServices = Test.CreateTestServices();
        var builder = new ServiceCollectionBuilder(appServices);
        var injector = builder.BuildServiceProvider();

        Assert.IsType<ServiceProvider>(injector);
    }

    [Fact]
    public void BuildWithDependencyInjection_with_open_generic_override()
    {
        var appServices = Test.CreateTestServices();
        var builder = new ServiceCollectionBuilder(appServices)
            .AddTypes(new[] { typeof(IOpen<>), typeof(DefaultOpen<>), typeof(MoreOpen<>) });
        var injector = builder.BuildServiceProvider();

        var moreOpen = injector.GetRequiredService<IOpen<int>>();
        Assert.IsType<MoreOpen<int>>(moreOpen);
    }

    [Fact]
    public void BuildWithDependencyInjection_with_open_generic_override_and_dependency()
    {
        var appServices = Test.CreateTestServices();
        var builder = new ServiceCollectionBuilder(appServices)
            .AddTypes(new[] { typeof(IOpen<>), typeof(DefaultOpen<>), typeof(MoreOpenWithDependency<>), typeof(Dependency) });
        var injector = builder.BuildServiceProvider();

        var moreOpen = injector.GetRequiredService<IOpen<int>>();
        Assert.IsType<MoreOpenWithDependency<int>>(moreOpen);
    }

    [Fact]
    public void Autofac_generic_export_with_ctor_dependency()
    {
        var conventions = new ServiceCollection();

        conventions.AddSingleton<Dependency>();
        conventions.AddSingleton(typeof(IOpen<>), typeof(MoreOpenWithDependency<>));

        using (var container = conventions.BuildServiceProvider())
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