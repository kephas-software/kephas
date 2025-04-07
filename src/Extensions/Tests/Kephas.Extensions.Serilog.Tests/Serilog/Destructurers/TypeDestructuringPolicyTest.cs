// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeDestructuringPolicyTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using FluentAssertions;
using Kephas.Extensions.Serilog.Destructurers;
using Serilog.Events;
using Xunit;

namespace Kephas.Extensions.Tests.Serilog.Destructurers;

public class TypeDestructuringPolicyTest
{
    [Fact]
    public void TryDestructure_not_handled()
    {
        var p = new TypeDestructuringPolicy();
        p.TryDestructure("123", null!, out var result)
            .Should().BeFalse();
        result.Should().BeNull();
    }
    
    [Fact]
    public void TryDestructure_handled()
    {
        var p = new TypeDestructuringPolicy();
        p.TryDestructure(typeof(string), null!, out var result)
            .Should().BeTrue();
        result.Should().Be(new ScalarValue(typeof(string).FullName));
    }
}