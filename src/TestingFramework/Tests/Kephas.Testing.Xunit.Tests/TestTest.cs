using FluentAssertions;
using Xunit;

namespace Kephas.Testing.Tests;

public class TestTest
{
    [Theory]
    [InlineData("e", "a")]
    public void GetTestAppRuntime_generic_config(string? environment, string? appName)
    {
        var appRuntime = Test.GetTestAppRuntime<TestTest>(config =>
            {
                config.Environment = environment;
                config.AppName = appName;
            });

        appRuntime.GetEnvironment().Should().Be(environment);
        appRuntime.GetAppName().Should().Be(appName);
        appRuntime.GetEntryAssembly().Should().BeSameAs(typeof(TestTest).Assembly);
    }

    [Theory]
    [InlineData(typeof(TestTest), typeof(string) )]
    public void GetTestAppRuntime(Type type1, Type type2)
    {
        var appRuntime = Test.GetTestAppRuntime([type1.Assembly, type2.Assembly]);

        appRuntime.GetAppAssemblies().Should().Equal([type1.Assembly, type2.Assembly]);
    }
}