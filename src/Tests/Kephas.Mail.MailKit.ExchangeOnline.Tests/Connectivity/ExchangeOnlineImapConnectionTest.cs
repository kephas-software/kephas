// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExchangeOnlineImapConnectionTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Mail.Tests.Connectivity;

using Kephas.Configuration;
using Kephas.Connectivity;
using Kephas.Cryptography;
using Kephas.Injection;
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
    [Test]
    [Ignore("Manual test, should provide connection settings/user name and password.")]
    public async Task OpenAsync()
    {
        var context = this.CreateConnectionContext();
        var configuration = this.CreateConfiguration();
        await using var connection = new ExchangeOnlineImapConnection(
            context,
            new NullEncryptionService(),
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

    private IConnectionContext CreateConnectionContext()
    {
        return new ConnectionContext(Substitute.For<IInjector>())
        {
            Credentials = new UserClearTextPasswordCredentials("<TODO>", "<TODO>"),
            Kind = "imap-ms365",
            Host = new Uri("imap-ms365://outlook.office365.com"),
        };
    }
}