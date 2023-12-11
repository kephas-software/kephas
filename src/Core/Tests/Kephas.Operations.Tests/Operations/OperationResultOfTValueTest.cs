// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OperationResultOfTValueTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using FluentAssertions;
using Kephas.Operations;
using Xunit;

namespace Kephas.Tests.Operations;

public class OperationResultOfTValueTest
{
    [Fact]
    public void OperationResult_complete()
    {
        var opResult = new OperationResult<int>(12).Complete();

        Assert.Equal(OperationState.Completed, opResult.OperationState);
        Assert.Equal(1f, opResult.PercentCompleted);
        Assert.Equal(12, opResult.Value);
    }

    [Fact]
    public void OperationResult_complete_exception()
    {
        var opResult = new ArgumentException("arg").ToOperationResult<int>();
        Assert.Equal(OperationState.Failed, opResult.OperationState);
        Assert.Equal(0f, opResult.PercentCompleted);
        opResult.Value.Should().Be(0);
    }

    [Fact]
    public void Value_setter()
    {
        var opResult = new OperationResult<int>(10);
        var result = opResult.Value;

        Assert.Equal(10, result);
    }

    [Fact]
    public void Deconstruct()
    {
        IOperationResult<string> result = new OperationResult<string>("hello").Complete();
        var (value, state) = result;

        Assert.Equal("hello", value);
        Assert.Equal(OperationState.Completed, state);
    }
}
