// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CorrelationIdTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using FluentAssertions;
using Kephas.Diagnostics;
using Xunit;

namespace Kephas.Tests.Diagnostics;

public class CorrelationIdTest
{
    [Fact]
    public void Assignment_when_set_then_copies_value_properly()
    {
        var correlationId = new CorrelationId();
        var otherCorrelationId = correlationId;

        otherCorrelationId.Should().Be(correlationId);
    }
}