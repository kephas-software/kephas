// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonFormatterTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using FluentAssertions;
using Kephas.Extensions.Logging;
using Kephas.Extensions.Logging.Formatting;
using Kephas.Runtime;
using NSubstitute;
using Serilog.Events;
using Serilog.Parsing;
using Xunit;

namespace Kephas.Extensions.Tests.Logging.Formatting;

public class JsonFormatterTest
{
    public JsonFormatterTest()
    {
        var appRuntime = Substitute.For<IAppRuntime>();
        appRuntime.GetEntryAssembly().Returns(typeof(JsonFormatterTest).Assembly);
        appRuntime.GetAppName().Returns("TEST-App");
        appRuntime.GetAppInstanceId().Returns("TEST-App-123");
        appRuntime.GetAppVersion().Returns("1.0.0-test");
        
        LoggingHelper.SetAppRuntime(appRuntime);
    }

    [Fact]
    public void Format_simple_with_default_solution_and_application()
    {
        var formatter = new JsonFormatter();
        using var writer = new StringWriter();
        var entry = new LogEvent(
            DateTimeOffset.Parse("2022-06-02 12:03:56.12345+03"),
            LogEventLevel.Error,
            null,
            new MessageTemplate("hello", Array.Empty<MessageTemplateToken>()),
            new[]
            {
                new LogEventProperty("Environment", new ScalarValue("ENV")),
                new LogEventProperty("UserName", new ScalarValue("gigi")),
                new LogEventProperty("MachineName", new ScalarValue("MYMACHINE")),
                new LogEventProperty("ThreadId", new ScalarValue(12)),
                new LogEventProperty("SourceContext", new ScalarValue("My.Class")),
                new LogEventProperty("CorrelationId", new ScalarValue("corr-id")),
            });
        formatter.Format(entry, writer);
        var actual = writer.GetStringBuilder().ToString();
        actual.Should().Be(@"{""Time"":""2022-06-02 09:03:56.123Z"",""Level"":""Error"",""Env"":""ENV"",""Message"":""hello"",""AppName"":""TEST-App"",""AppVersion"":""1.0.0-test"",""AppInstanceId"":""TEST-App-123"",""ClassName"":""My.Class"",""UserName"":""gigi"",""MachineName"":""MYMACHINE"",""ThreadId"":12,""CorrelationId"":""corr-id""}" + Environment.NewLine);
    }

    [Fact]
    public void Format_simple()
    {
        var formatter = new JsonFormatter();
        using var writer = new StringWriter();
        var entry = new LogEvent(
            DateTimeOffset.Parse("2022-06-02 12:03:56.12345+03"),
            LogEventLevel.Error,
            null,
            new MessageTemplate("hello", Array.Empty<MessageTemplateToken>()),
            new[]
            {
                new LogEventProperty("Solution", new ScalarValue("APP")),
                new LogEventProperty("AppName", new ScalarValue("TEST")),
                new LogEventProperty("AppInstanceId", new ScalarValue("TEST-123")),
                new LogEventProperty("Environment", new ScalarValue("ENV")),
                new LogEventProperty("UserName", new ScalarValue("gigi")),
                new LogEventProperty("MachineName", new ScalarValue("MYMACHINE")),
                new LogEventProperty("ThreadId", new ScalarValue(12)),
                new LogEventProperty("SourceContext", new ScalarValue("My.Class")),
                new LogEventProperty("CorrelationId", new ScalarValue("corr-id")),
            });
        formatter.Format(entry, writer);
        var actual = writer.GetStringBuilder().ToString();
        actual.Should().Be(@"{""Time"":""2022-06-02 09:03:56.123Z"",""Level"":""Error"",""Env"":""ENV"",""Message"":""hello"",""AppVersion"":""1.0.0-test"",""ClassName"":""My.Class"",""Solution"":""APP"",""AppName"":""TEST"",""AppInstanceId"":""TEST-123"",""UserName"":""gigi"",""MachineName"":""MYMACHINE"",""ThreadId"":12,""CorrelationId"":""corr-id""}" + Environment.NewLine, actual);
    }

    [Fact]
    public void Format_api()
    {
        var formatter = new JsonFormatter();
        using var writer = new StringWriter();
        var entry = new LogEvent(
            DateTimeOffset.Parse("2022-06-02 12:03:56.12345+03"),
            LogEventLevel.Error,
            null,
            new MessageTemplate("hello", Array.Empty<MessageTemplateToken>()),
            new[]
            {
                new LogEventProperty("AppName", new ScalarValue("TEST")),
                new LogEventProperty("AppInstanceId", new ScalarValue("TEST-123")),
                new LogEventProperty("ClientIP", new ScalarValue("1.2.3.4")),
                new LogEventProperty("UserName", new ScalarValue("gigi")),
                new LogEventProperty("MachineName", new ScalarValue("MYMACHINE")),
                new LogEventProperty("ThreadId", new ScalarValue(12)),
                new LogEventProperty("SourceContext", new ScalarValue("My.Class")),
                new LogEventProperty("Uri", new ScalarValue("https://server/api")),
            });
        formatter.Format(entry, writer);
        var actual = writer.GetStringBuilder().ToString();
        actual.Should().Be(@"{""Time"":""2022-06-02 09:03:56.123Z"",""Level"":""Error"",""Env"":"""",""Message"":""hello"",""AppVersion"":""1.0.0-test"",""ClassName"":""My.Class"",""AppName"":""TEST"",""AppInstanceId"":""TEST-123"",""ClientIP"":""1.2.3.4"",""UserName"":""gigi"",""MachineName"":""MYMACHINE"",""ThreadId"":12,""Uri"":""https://server/api""}" + Environment.NewLine);
    }

    [Fact]
    public void Format_exception()
    {
        var formatter = new JsonFormatter();
        using var writer = new StringWriter();
        var entry = new LogEvent(
            DateTimeOffset.Parse("2022-06-02 12:03:56.12345+03"),
            LogEventLevel.Error,
            GetException(),
            new MessageTemplate("hello", Array.Empty<MessageTemplateToken>()),
            new[]
            {
                new LogEventProperty("AppName", new ScalarValue("TEST")),
                new LogEventProperty("Environment", new ScalarValue("TEST-ENV")),
            });
        formatter.Format(entry, writer);
        var actual = writer.GetStringBuilder().ToString();
        actual.Should().StartWith("""
                          {"Time":"2022-06-02 09:03:56.123Z","Level":"Error","Env":"TEST-ENV","Message":"hello","AppVersion":"1.0.0-test","AppInstanceId":"TEST-App-123","StackTrace":"System.InvalidOperationException: Cannot do
                          """);
        actual.Should().Contain(@"JsonFormatterTest.cs:line ");
    }

    private Exception GetException()
    {
        try
        {
            throw new InvalidOperationException("Cannot do this");
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    [Fact]
    public void Format_variables_replaced()
    {
        var formatter = new JsonFormatter();
        using var writer = new StringWriter();
        var entry = new LogEvent(
            DateTimeOffset.Parse("2022-06-02 12:03:56.12345+03"),
            LogEventLevel.Error,
            null,
            new MessageTemplate("hello {AppName} with {items}", Array.Empty<MessageTemplateToken>()),
            new[]
            {
                new LogEventProperty("AppName", new ScalarValue("TEST")),
                new LogEventProperty("AppInstanceId", new ScalarValue("TEST-abc")),
                new LogEventProperty("Environment", new ScalarValue("TEST-ENV")),
                new LogEventProperty("items", new SequenceValue(  new[] { new ScalarValue("1"), new ScalarValue("2") })),
            });
        formatter.Format(entry, writer);
        var actual = writer.GetStringBuilder().ToString();
        actual.Should().Be(@"{""Time"":""2022-06-02 09:03:56.123Z"",""Level"":""Error"",""Env"":""TEST-ENV"",""Message"":""hello TEST with [ 1, 2 ]"",""AppVersion"":""1.0.0-test"",""AppName"":""TEST"",""AppInstanceId"":""TEST-abc"",""items"":[""1"",""2""]}" + Environment.NewLine);
    }
}