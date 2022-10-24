namespace Kephas.Serialization.Json.Tests;

using System.Diagnostics.CodeAnalysis;
using Kephas.Net.Mime;
using Kephas.Reflection;
using Kephas.Services.Builder;
using NUnit.Framework;

[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
public abstract class JsonSerializerIntegrationTestBase : SerializationTestBase
{
    protected abstract IServiceProvider BuildServiceProvider(IAppServiceCollectionBuilder servicesBuilder);

    [Test]
    public async Task SerializeAsync_injection()
    {
        var container = this.BuildServiceProvider(this.CreateServicesBuilder());
        var serializationService = container.Resolve<ISerializationService>();

        var obj = new JsonSerializerTest.TestEntity
        {
            Name = "John Doe",
            PersonalSite = new Uri("http://site.com/my-site"),
        };

        var serializedObj = await serializationService.JsonSerializeAsync(obj);

        Assert.AreEqual(
            @"{""$type"":""Kephas.Serialization.Json.Tests.JsonSerializerTest+TestEntity"",""name"":""John Doe"",""personalSite"":""http://site.com/my-site""}",
            serializedObj);
    }

    [Test]
    public async Task JsonSerializer_injection()
    {
        var builder = this.CreateServicesBuilder(this.CreateAmbientServices().WithStaticAppRuntime())
            .WithAssemblies(
                typeof(ISerializationService).Assembly,
                typeof(JsonSerializer).Assembly,
                typeof(DefaultTypeResolver).Assembly);
        var container = this.BuildServiceProvider(builder);
        var serializers = container.ResolveMany<Lazy<ISerializer, SerializerMetadata>>();
        var jsonSerializer = serializers.SingleOrDefault(s => s.Metadata.MediaType == typeof(JsonMediaType))?.Value;

        Assert.IsInstanceOf<JsonSerializer>(jsonSerializer);
    }
}