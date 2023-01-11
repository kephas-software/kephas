namespace Kephas.Tests;

using System.Diagnostics.CodeAnalysis;
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
}