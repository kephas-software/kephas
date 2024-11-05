// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoggingHelperTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using FluentAssertions;
using Kephas.Extensions.Logging;
using Kephas.Runtime;
using NSubstitute;
using Xunit;

namespace Kephas.Extensions.Tests.Logging;

public class LoggingHelperTest
{
    [Fact]
    public void WriteFallbackLogError()
    {
        var currentFallbackPath = LoggingHelper.FallbackLogPath;
        try
        {
            LoggingHelper.FallbackLogPath = Path.GetTempPath();
            var appRuntime = Substitute.For<IAppRuntime>();
            var appInstanceId = Guid.NewGuid().ToString("N");
            appRuntime.GetAppName().Returns("app");
            appRuntime.GetAppInstanceId().Returns(appInstanceId);
            appRuntime.GetEnvironment().Returns("env");

            var localNow = new DateTime(2024, 12, 25);
            var filePath = Path.Combine(LoggingHelper.FallbackLogPath, $"internal-{appInstanceId}.20241225.json");

            var threadId = Environment.CurrentManagedThreadId;

            try
            {
                LoggingHelper.WriteFallbackLogError("hello", appRuntime, localNow);
                var lines = File.ReadAllLines(filePath);
                lines.Should().Satisfy(e => e == $$$"""{"Time":"2024-12-24T23:00:00","Level":"Error","Message":"hello","AppName":"app","AppVersion":"","AppInstanceId":"{{{appInstanceId}}}","Env":"env","MachineName":"","ThreadId":"{{{threadId}}}"}""");
            }
            finally
            {
                try
                {
                    File.Delete(filePath);
                }
                catch
                {
                    // no 
                }
            }
        }
        finally
        {
            LoggingHelper.FallbackLogPath = currentFallbackPath;
        }
    }
}