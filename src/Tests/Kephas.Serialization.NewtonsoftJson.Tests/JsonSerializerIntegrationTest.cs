namespace Kephas.Serialization.Json.Tests;

using System.Diagnostics.CodeAnalysis;
using Kephas.Services.Builder;
using NUnit.Framework;

[TestFixture]
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
    Justification = "Reviewed. Suppression is OK here.")]
public class JsonSerializerIntegrationTest : JsonSerializerIntegrationTestBase
{
    protected override IServiceProvider BuildServiceProvider(IAppServiceCollectionBuilder servicesBuilder)
    {
        return servicesBuilder.BuildWithDependencyInjection();
    }
}