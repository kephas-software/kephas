// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultConnectionProviderTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Connectivity.Tests;

using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;

[TestFixture]
public class DefaultConnectionProviderTest : ConnectivityTestBase
{
    [Test]
    public void DefaultConnectionProvider_Injection_success()
    {
        var container = this.CreateInjector();
        var provider = container.Resolve<IConnectionProvider>();
        Assert.IsInstanceOf<DefaultConnectionProvider>(provider);

        var typedProvider = (DefaultConnectionProvider)provider;
        Assert.IsNotNull(typedProvider.Logger);
    }

    [Test]
    public void CreateConnection_Injection_success()
    {
        var container = this.CreateInjector(parts: new[] { typeof(TestConnectionFactory) });
        var provider = container.Resolve<IConnectionProvider>();

        var expected = Substitute.For<IConnection>();
        var actual = provider.CreateConnection(
            "test://host:121",
            options: ctx => { ctx["connection"] = expected; });

        Assert.AreEqual(expected, actual);
    }

    [Test]
    public void CreateConnection_Injection_failed()
    {
        var container = this.CreateInjector();
        var provider = container.Resolve<IConnectionProvider>();

        var expected = Substitute.For<IConnection>();
        Assert.Throws<ConnectivityException>(() => provider.CreateConnection(
            "test://host:121",
            options: ctx => { ctx["connection"] = expected; }));
    }
}