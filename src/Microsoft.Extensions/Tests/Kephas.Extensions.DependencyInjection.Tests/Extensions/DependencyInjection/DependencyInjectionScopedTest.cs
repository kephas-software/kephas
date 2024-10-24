// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DependencyInjectionScopedTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using FluentAssertions;
using Kephas.DependencyInjection;
using Kephas.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Kephas.Tests.Extensions.DependencyInjection;

public class DependencyInjectionScopedTest
{
    [Fact]
    public void CreateScopedInjector_NestedScopes()
    {
        var appServices = Test.CreateTestServices();
        appServices.Add(new ServiceInfo(
            typeof(DependencyInjectionTest.ScopeExportedClass),
            typeof(DependencyInjectionTest.ScopeExportedClass),
            AppServiceLifetime.Scoped));
        var container = appServices.BuildServiceProvider();
        using var scopedContext = container.CreateScope();
        var scopedInstance1 = scopedContext.ServiceProvider.GetRequiredService<DependencyInjectionTest.ScopeExportedClass>();

        using var nestedContext = scopedContext.ServiceProvider.CreateScope();
        Assert.NotSame(scopedContext, nestedContext);

        var scopedInstance2 = nestedContext.ServiceProvider.GetRequiredService<DependencyInjectionTest.ScopeExportedClass>();
        Assert.NotSame(scopedInstance1, scopedInstance2);
    }

    [Fact]
    public void CreateScopedInjector_Injector_registration_scope()
    {
        var container = Test.CreateTestServiceCollectionBuilder().BuildServiceProvider();
        using var scopedContext = container.CreateScope();
        var selfScopedContext = scopedContext.ServiceProvider.GetRequiredService<IServiceProvider>();
        Assert.Same(selfScopedContext, scopedContext);
    }

    [Fact]
    public void CreateScopedInjector_Injector_registration_scope_consumers()
    {
        var appServices = Test.CreateTestServices();
        appServices.Add(new ServiceInfo(
            typeof(DependencyInjectionTest.ScopeExportedClass),
            typeof(DependencyInjectionTest.ScopeExportedClass),
            AppServiceLifetime.Scoped));
        var container = appServices.BuildServiceProvider();
        using var scopedContext = container.CreateScope();
        var scopedInstance = scopedContext.ServiceProvider.GetRequiredService<DependencyInjectionTest.ScopeExportedClass>();
        Assert.Same(scopedContext, scopedInstance.ServiceProvider);
    }
}