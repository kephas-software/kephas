// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OperationMessageTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using FluentAssertions;
using Kephas.ExceptionHandling;
using Kephas.Operations;
using Xunit;

namespace Kephas.Tests.Operations;

public class OperationMessageTest
{
    [Theory]
    [InlineData("hello", null, "[Information] hello")]
    [InlineData("go home", 1700208872000, "[2023-11-17T08:14:32 Information] go home")]
    public void ToString_contains_message(string message, long? timestamp, string expectedValue)
    {
        var msg = new OperationMessage(message) { Timestamp = timestamp };
        msg.ToString().Should().Be(expectedValue);
    }

    [Fact]
    public void Constructor_SetsMessage()
    {
        var message = "Test message";
        var operationMessage = new OperationMessage(message);

        operationMessage.Message.Should().Be(message);
    }

    [Fact]
    public void Constructor_SetsDefaultSeverity()
    {
        var operationMessage = new OperationMessage();

        operationMessage.Severity.Should().Be(SeverityLevel.Information);
    }

    [Fact]
    public void ToString_ReturnsFormattedString_WithTimestamp()
    {
        var timestamp = new DateTimeOffset(2023, 10, 1, 12, 0, 0, TimeSpan.Zero).ToUnixTimeMilliseconds();
        var operationMessage = new OperationMessage("Test message")
        {
            Timestamp = timestamp,
            Severity = SeverityLevel.Warning
        };

        var result = operationMessage.ToString();

        result.Should().Be("[2023-10-01T12:00:00 Warning] Test message");
    }

    [Fact]
    public void ToString_ReturnsFormattedString_WithoutTimestamp()
    {
        var operationMessage = new OperationMessage("Test message")
        {
            Severity = SeverityLevel.Error
        };

        var result = operationMessage.ToString();

        result.Should().Be("[Error] Test message");
    }

    [Fact]
    public void ToString_ReturnsFormattedString_WithNullMessage()
    {
        var operationMessage = new OperationMessage()
        {
            Severity = SeverityLevel.Critical
        };

        var result = operationMessage.ToString();

        result.Should().Be("[Critical] ");
    }
}