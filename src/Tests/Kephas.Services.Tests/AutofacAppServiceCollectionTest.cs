namespace Kephas.Tests;

using System.Diagnostics.CodeAnalysis;
using Autofac.Core;
using NUnit.Framework;

/// <summary>
/// Test class for <see cref="AppServiceCollection"/>.
/// </summary>
[TestFixture]
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
public class AutofacAppServiceCollectionTest : AppServiceCollectionTestBase
{
    protected override IServiceProvider BuildServiceProvider(IAppServiceCollection appServices)
    {
        return this.CreateServicesBuilder(appServices).BuildWithAutofac();
    }

    [Test]
    public void Register_circular_dependency_singleton()
    {
        var appServices = this.CreateAppServices();
        appServices.Add<CircularDependency1, CircularDependency1>();
        appServices.Add<CircularDependency2, CircularDependency2>();

        var container = this.BuildServiceProvider(appServices);

        Assert.Throws<DependencyResolutionException>(() => container.GetService<CircularDependency1>());
    }

    [Test]
    public void Register_circular_dependency_transient()
    {
        var appServices = this.CreateAppServices();
        appServices.Add<CircularDependency1, CircularDependency1>(b => b.Transient());
        appServices.Add<CircularDependency2, CircularDependency2>(b => b.Transient());

        var container = this.BuildServiceProvider(appServices);

        Assert.Throws<DependencyResolutionException>(() => container.GetService<CircularDependency1>());
    }
}