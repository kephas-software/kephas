namespace Kephas.Connectivity.Tests;

using NSubstitute;
using NUnit.Framework;

public abstract class DefaultConnectionProviderTestBase : ConnectivityTestBase
{
    protected abstract IServiceProvider BuildServiceProvider(params Type[] parts);

    [Test]
    public void DefaultConnectionProvider_Injection_success()
    {
        var container = this.BuildServiceProvider();
        var provider = container.Resolve<IConnectionProvider>();
        Assert.IsInstanceOf<DefaultConnectionProvider>(provider);

        var typedProvider = (DefaultConnectionProvider)provider;
        Assert.IsNotNull(typedProvider.Logger);
    }

    [Test]
    public void CreateConnection_Injection_success()
    {
        var container = this.BuildServiceProvider(parts: new[] { typeof(TestConnectionFactory) });
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
        var container = this.BuildServiceProvider();
        var provider = container.Resolve<IConnectionProvider>();

        var expected = Substitute.For<IConnection>();
        Assert.Throws<ConnectivityException>(() => provider.CreateConnection(
            "test://host:121",
            options: ctx => { ctx["connection"] = expected; }));
    }
}