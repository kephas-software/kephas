// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PipelineTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Text;
using FluentAssertions;
using Kephas.Pipelines;
using Kephas.Services;
using Kephas.Testing;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Kephas.Tests.Pipelines;

public class PipelineTest
{
    [Theory]
    [InlineData(LogLevel.Debug, "Debug: Invoking pipeline operation ({op-params})...\nDebug: Invoked pipeline operation ({op-params}).\n")]
    [InlineData(LogLevel.Information, "")]
    public void Process_no_behaviors(LogLevel logLevel, string log)
    {
        var logger = Test.GetTestLogger<Pipeline<object?, object, object?>>(logLevel);
        var pipeline = new Pipeline<object?, object, object?>(logger: logger);

        var target = new object();
        var args = new object();
        var result = new object();

        object? Operation() => result;

        var pipelineResult = pipeline.Process(target, args, Operation);
        pipelineResult.Should().BeSameAs(result);
        var expectedLog = log
            .Replace("\n", Environment.NewLine)
            .Replace("{op-params}", $"System.Object/System.Object/System.Object");
        logger.ToString().Should().Be(expectedLog);
    }

    [Theory]
    [InlineData(LogLevel.Debug, "Debug: Invoking pipeline operation ({op-params})...\nDebug: Invoked pipeline operation ({op-params}).\n")]
    [InlineData(LogLevel.Information, "")]
    public void Process_no_behaviors_no_context(LogLevel logLevel, string log)
    {
        var logger = Test.GetTestLogger<Pipeline<object?, object, object?>>(logLevel);
        var pipeline = new Pipeline<object?, object, object?>(logger: logger);

        var target = new object();
        var args = new object();
        var result = new object();

        object? Operation() => result;

        var pipelineResult = pipeline.Process(target, args, Operation);
        pipelineResult.Should().BeSameAs(result);
        var expectedLog = log
            .Replace("\n", Environment.NewLine)
            .Replace("{op-params}", $"System.Object/System.Object/System.Object");
        logger.ToString().Should().Be(expectedLog);
    }

    [Theory]
    [InlineData(LogLevel.Debug, null, null, "test", "test", "Debug: Invoking pipeline behavior ({behavior-params})...\nb1Debug: Invoking pipeline behavior ({behavior-params})...\nb2Debug: Invoking pipeline operation ({op-params})...\nDebug: Invoked pipeline operation ({op-params}).\nDebug: Invoked pipeline behavior ({behavior-params}).\nDebug: Invoked pipeline behavior ({behavior-params}).\n")]
    [InlineData(LogLevel.Debug, "b1Test", null, "test", "b1Test", "Debug: Invoking pipeline behavior ({behavior-params})...\nb1Debug: Invoked pipeline behavior ({behavior-params}).\n")]
    [InlineData(LogLevel.Debug, null, "b2Test", "test", "b2Test", "Debug: Invoking pipeline behavior ({behavior-params})...\nb1Debug: Invoking pipeline behavior ({behavior-params})...\nb2Debug: Invoked pipeline behavior ({behavior-params}).\nDebug: Invoked pipeline behavior ({behavior-params}).\n")]
    [InlineData(LogLevel.Debug, "b1Test", "b2Test", "test", "b1Test", "Debug: Invoking pipeline behavior ({behavior-params})...\nb1Debug: Invoked pipeline behavior ({behavior-params}).\n")]
    [InlineData(LogLevel.Information, null, null, "test", "test", "b1b2")]
    [InlineData(LogLevel.Information, "b1Test", null, "test", "b1Test", "b1")]
    [InlineData(LogLevel.Information, null, "b2Test", "test", "b2Test", "b1b2")]
    [InlineData(LogLevel.Information, "b1Test", "b2Test", "test", "b1Test", "b1")]
    public void Process_with_behaviors(LogLevel logLevel, string? b1Result, string? b2Result, string opResult, string result, string log)
    {
        var sb = new StringBuilder();
        var logger = Test.GetTestLogger<Pipeline<object?, object, object?>>(sb, logLevel);
        var b1 = Substitute.For<IPipelineBehavior<object?, object, object?>>();
        var b2 = Substitute.For<IPipelineBehavior<object?, object, object?>>();
        var selector = Test.GetTestLazyEnumerable<IPipelineBehavior, PipelineBehaviorMetadata>(b1, b2);
        var pipeline = new Pipeline<object?, object, object?>(selector, logger);

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
        var expectedLog = log
            .Replace("\n", Environment.NewLine)
            .Replace("{behavior-params}", $"{b1.GetType()}/System.Object/System.Object/System.Object")
            .Replace("{op-params}", $"System.Object/System.Object/System.Object");
        sb.ToString().Should().Be(expectedLog);
    }
}
