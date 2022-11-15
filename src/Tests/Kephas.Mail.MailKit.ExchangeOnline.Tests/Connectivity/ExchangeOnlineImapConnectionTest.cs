namespace Kephas.Mail.Tests.Connectivity;

using Kephas.Configuration;
using Kephas.Connectivity;
using Kephas.Cryptography;
using Kephas.Logging;
using Kephas.Mail.Connectivity;
using Kephas.Net.Mime;
using Kephas.Security.Authentication;
using Kephas.Serialization;
using Kephas.Serialization.Json;
using Kephas.Services;
using NSubstitute;
using NUnit.Framework;

[TestFixture]
public class ExchangeOnlineImapConnectionTest
{
    [Test, Ignore("Manual test, should provide connection settings/user name and password.")]
    public async Task OpenAsync()
    {
        var context = this.CreateConnectionContext();
        var serializationService = this.CreateSerializationService();
        var configuration = this.CreateConfiguration();
        await using var connection = new ExchangeOnlineImapConnection(
            context,
            new NullEncryptionService(),
            serializationService,
            configuration,
            Substitute.For<ILogger<ExchangeOnlineImapConnection>?>());

        await connection.OpenAsync();

        await Task.Delay(TimeSpan.FromSeconds(100));

        Assert.AreEqual(1, 1);
    }

    private IConfiguration<ExchangeOnlineConnectionSettings> CreateConfiguration()
    {
        var configuration = Substitute.For<IConfiguration<ExchangeOnlineConnectionSettings>>();
        var settings = new ExchangeOnlineConnectionSettings
        {
            TenantId = "<TODO>",
            ClientId = "<TODO>",
        };

        configuration.GetSettings(Arg.Any<IContext>()).Returns(settings);
        return configuration;
    }

    private ISerializationService CreateSerializationService()
    {
        return new DefaultSerializationService(
            Substitute.For<IInjectableFactory>(),
            new List<IExportFactory<ISerializer, SerializerMetadata>>
            {
                new ExportFactory<ISerializer, SerializerMetadata>(
                    () => new JsonSerializer(DefaultJsonSerializerSettingsProvider.Instance),
                    new SerializerMetadata(typeof(JsonMediaType))),
            });
    }

    private IConnectionContext CreateConnectionContext()
    {
        return new ConnectionContext(Substitute.For<IServiceProvider>())
        {
            Credentials = new UserClearTextPasswordCredentials("<TODO>", "<TODO>"),
            Kind = "imap-ms365",
            Host = new Uri("imap-ms365://outlook.office365.com"),
        };
    }
}