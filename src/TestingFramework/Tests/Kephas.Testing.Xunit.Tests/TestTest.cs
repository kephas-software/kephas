using FluentAssertions;
using Xunit;

namespace Kephas.Testing.Tests;

public class TestTest
{
    [Theory]
    [InlineData("e", "a")]
    public void GetTestAppRuntime(string? environment, string? appName)
    {
        var appRuntime = Test.GetTestAppRuntime(environment, appName);

        appRuntime.GetEnvironment().Should().Be(environment);
        appRuntime.GetAppName().Should().Be(appName);
    }
}