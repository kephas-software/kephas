// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OperationResultTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the operation result test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using FluentAssertions;
using Kephas.Operations;
using Xunit;

namespace Kephas.Tests.Operations;

public class OperationResultTest
{
    [Fact]
    public void OperationResult_complete()
    {
        var opResult = new OperationResult(12).Complete();

        Assert.Equal(OperationStatus.Completed, opResult.OperationStatus);
        Assert.Equal(1f, opResult.PercentCompleted);
        Assert.Equal(12, opResult.Value);
    }

    [Fact]
    public void OperationResult_complete_exception()
    {
        var opResult = new ArgumentException("arg").ToOperationResult<object?>();
        Assert.Equal(OperationStatus.Failed, opResult.OperationStatus);
        Assert.Equal(0f, opResult.PercentCompleted);
        opResult.Value.Should().BeNull();
    }

    [Fact]
    public void Messages_Clear()
    {
        var opResult = new OperationResult().MergeException(new InvalidOperationException());

        opResult.Messages.Should().NotBeEmpty();
        opResult.Messages.Clear();
        opResult.Messages.Should().BeEmpty();
    }

    [Fact]
    public void Deconstruct()
    {
        IOperationResult result = new OperationResult("hello").Complete();
        var (value, state) = result;

        Assert.Equal("hello", value);
        Assert.Equal(OperationStatus.Completed, state);
    }
}
