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

public class AsyncPipelineTest
{
    [Fact]
    public async Task ProcessAsync_no_behaviors()
    {
        var pipeline = new AsyncPipeline<object?, object, object?>();

        var target = new object();
        var args = new object();
        var token = new CancellationToken();
        var result = new object();

        Task<object?> Operation()
        {
            return Task.FromResult<object?>(result);
        }

        var pipelineResult = await pipeline.ProcessAsync(target, args, Operation, token);
        pipelineResult.Should().BeSameAs(result);
    }

    [Theory]
    [InlineData(null, null, "test", "test")]
    [InlineData("b1Test", null, "test", "b1Test")]
    [InlineData(null, "b2Test", "test", "b2Test")]
    [InlineData("b1Test", "b2Test", "test", "b1Test")]
    public async Task ProcessAsync_with_behaviors(string? b1Result, string? b2Result, string opResult, string result)
    {
        var sb = new StringBuilder();
        var b1 = Substitute.For<IAsyncPipelineBehavior<object?, object, object?>>();
        var b2 = Substitute.For<IAsyncPipelineBehavior<object?, object, object?>>();
        var selector = Test.GetTestLazyEnumerable<IAsyncPipelineBehavior, PipelineBehaviorMetadata>(b1, b2);
        var pipeline = new AsyncPipeline<object?, object, object?>(selector);

        var target = new object();
        var args = new object();
        var token = new CancellationToken();

        b1.InvokeAsync(Arg.Any<Func<Task<object?>>>(), target, args, token)
            .Returns(ci =>
            {
                sb.Append("b1");
                return b1Result is not null
                    ? Task.FromResult<object?>(b1Result)
                    : ci.Arg<Func<Task<object?>>>()();
            });
            
        b2.InvokeAsync(Arg.Any<Func<Task<object?>>>(), target, args, token)
            .Returns(ci =>
            {
                sb.Append("b2");
                return b2Result is not null
                    ? Task.FromResult<object?>(b2Result)
                    : ci.Arg<Func<Task<object?>>>()();
            });
            
        Task<object?> Operation()
        {
            return Task.FromResult<object?>(opResult);
        }

        var pipelineResult = await pipeline.ProcessAsync(target, args, Operation, token);
        pipelineResult.Should().Be(result);
    }
}
