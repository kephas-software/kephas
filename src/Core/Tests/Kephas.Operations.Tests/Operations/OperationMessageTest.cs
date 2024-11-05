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
using Kephas.Operations;
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
}
