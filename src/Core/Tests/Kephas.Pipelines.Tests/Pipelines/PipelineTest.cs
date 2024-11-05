// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PipelineTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Text;
using FluentAssertions;
using Kephas.Pipelines;
using Kephas.Testing;
using NSubstitute;
using Xunit;

namespace Kephas.Tests.Pipelines;

public class PipelineTest
{
    [Fact]
    public void Process_no_behaviors()
    {
        var pipeline = new Pipeline<object?, object, object?>();

        var target = new object();
        var args = new object();
        var result = new object();

        object? Operation() => result;

        var pipelineResult = pipeline.Process(target, args, Operation);
        pipelineResult.Should().BeSameAs(result);
    }

    [Theory]
    [InlineData(null, null, "test", "test")]
    [InlineData("b1Test", null, "test", "b1Test")]
    [InlineData(null, "b2Test", "test", "b2Test")]
    [InlineData("b1Test", "b2Test", "test", "b1Test")]
    public void Process_with_behaviors(string? b1Result, string? b2Result, string opResult, string result)
    {
        var sb = new StringBuilder();
        var b1 = Substitute.For<IPipelineBehavior<object?, object, object?>>();
        var b2 = Substitute.For<IPipelineBehavior<object?, object, object?>>();
        var selector = Test.GetTestLazyEnumerable<IPipelineBehavior, PipelineBehaviorMetadata>(b1, b2);
        var pipeline = new Pipeline<object?, object, object?>(selector);

        var target = new object();
        var args = new object();

        b1.Invoke(Arg.Any<Func<object?>>(), target, args)
            .Returns(ci =>
            {
                sb.Append("b1");
                return b1Result ?? ci.Arg<Func<object?>>()();
            });
            
        b2.Invoke(Arg.Any<Func<object?>>(), target, args)
            .Returns(ci =>
            {
                sb.Append("b2");
                return b2Result ?? ci.Arg<Func<object?>>()();
            });
            
        object? Operation() => opResult;

        var pipelineResult = pipeline.Process(target, args, Operation);
        pipelineResult.Should().Be(result);
    }
}
