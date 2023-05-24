namespace Kephas.Serialization.Json.Tests;

using System.Diagnostics.CodeAnalysis;
using Kephas.Services.Builder;
using NUnit.Framework;

[TestFixture]
public class JsonSerializerIntegrationTest : JsonSerializerIntegrationTestBase
{
    protected override IServiceProvider BuildServiceProvider(IAppServiceCollectionBuilder servicesBuilder)
    {
        return servicesBuilder.BuildWithDependencyInjection();
    }
}