// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OperationMessageTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the data i/o message test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using FluentAssertions;
using Kephas.ExceptionHandling;
using Kephas.Operations;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Kephas.Tests.Operations;

public class OperationMessageTest
{
    [Theory]
    [InlineData("hello", null, "[Information] hello")]
    [InlineData("go home", 638358092727179476, "[2023-11-17T09:14:32 Information] go home")]
    public void ToString_contains_message(string message, long? timestamp, string expectedValue)
    {
        var msg = new OperationMessage(message) { Timestamp = timestamp };
        msg.ToString().Should().Be(expectedValue);
    }

    [Theory]
    [InlineData(typeof(ArgumentException), "hello", LogLevel.Warning, "[Warning] hello (ArgumentException)")]
    [InlineData(typeof(TestCriticalException), "hello", null, "[Critical] hello (TestCriticalException)")]
    [InlineData(typeof(TestCriticalException), "hello", LogLevel.Warning, "[Warning] hello (TestCriticalException)")]
    public void ToString_contains_exception(Type exceptionType, string exceptionMessage, LogLevel? logLevel, string expectedValue)
    {
        var exception = (Exception)Activator.CreateInstance(exceptionType, exceptionMessage)!;
        var msg = logLevel is not null
            ? new OperationMessage(exception) { LogLevel = logLevel.Value }
            : new OperationMessage(exception);

        msg.ToString().Should().Be(expectedValue);
    }

    internal class TestCriticalException : AppException
    {
        public TestCriticalException(string message)
            : base(message)
        {
            Severity = LogLevel.Critical;
        }
    }
}
