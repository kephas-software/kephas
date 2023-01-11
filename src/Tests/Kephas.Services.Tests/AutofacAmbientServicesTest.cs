namespace Kephas.Tests;

using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;

/// <summary>
/// Test class for <see cref="AppServiceCollection"/>.
/// </summary>
[TestFixture]
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
public class AutofacAmbientServicesTest : AmbientServicesTestBase
{
    protected override IServiceProvider BuildServiceProvider(IAmbientServices ambientServices)
    {
        return this.CreateServicesBuilder(ambientServices).BuildWithAutofac();
    }
}