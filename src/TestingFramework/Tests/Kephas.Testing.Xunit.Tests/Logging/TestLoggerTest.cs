using Kephas.Testing.Logging;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Kephas.Testing.Tests.Logging;

public class TestLoggerTest
{
    [Fact]
    public void LogInformation_message()
    {
        var logger = new TestLogger<string>();
        logger.LogInformation("hello");
        
        Assert.Equal($"Information: hello{Environment.NewLine}", logger.ToString());
    }

    [Fact]
    public void LogInformation_exception()
    {
        var logger = new TestLogger<string>();
        logger.LogInformation(new InvalidOperationException(), "bad boy");
        
        Assert.Equal($"InvalidOperationException Information: bad boy{Environment.NewLine}", logger.ToString());
    }
}
