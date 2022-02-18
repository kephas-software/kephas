// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AutofacDefaultConnectionProviderTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Connectivity.Tests.Injection.Autofac;

using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;

[TestFixture]
public class AutofacDefaultConnectionProviderTest : AutofacConnectivityTestBase
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
    public async Task ProcessAsync_Injection_success()
    {
        var container = this.CreateInjector(parts: new[] { typeof(TestConnectionFactory) });
        var provider = container.Resolve<IConnectionProvider>();

        var expected = Substitute.For<IConnection>();
        var actual = provider.CreateConnection(ctx =>
        {
            ctx["connection"] = expected;
            ctx.Host = new Uri("test://host:121");
        });

        Assert.AreEqual(expected, actual);
    }
}
