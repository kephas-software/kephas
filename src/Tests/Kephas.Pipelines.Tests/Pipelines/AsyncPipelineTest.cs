// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PipelineTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Text;
using Kephas.Logging;
using Kephas.Pipelines;
using Kephas.Services;
using NSubstitute;
using NUnit.Framework;

namespace Kephas.Tests.Pipelines;

[TestFixture]
public class AsyncPipelineTest
{
    [Test]
    [TestCase(LogLevel.Debug, "Debug: Invoking pipeline operation ({0}/{1}/{2})...Debug: Invoked pipeline operation ({0}/{1}/{2}).")]
    [TestCase(LogLevel.Info, "")]
    public async Task ProcessAsync_no_behaviors(LogLevel logLevel, string log)
    {
        var sb = new StringBuilder();
        var logger = Substitute.For<ILogger<AsyncPipeline<object?, object, object?>>>();
        logger.IsEnabled(Arg.Any<LogLevel>())
            .Returns(ci => ci.Arg<LogLevel>() <= logLevel);
        logger.When(l => l.Log(Arg.Any<LogLevel>(), Arg.Any<Exception?>(), Arg.Any<string?>(), Arg.Any<object?[]>()))
            .Do(ci => sb.Append($"{ci.Arg<LogLevel>()}: {ci.Arg<string>()}"));
        var pipeline = new AsyncPipeline<object?, object, object?>(GetPipelineContextFactory(), logger: logger);

        var target = new object();
        var args = new object();
        var context = Substitute.For<IContext>();
        var token = new CancellationToken();
        var result = new object();

        Task<object?> Operation()
        {
            return Task.FromResult<object?>(result);
        }

        var pipelineResult = await pipeline.ProcessAsync(target, args, context, Operation, token);
        Assert.AreSame(result, pipelineResult);
        Assert.AreEqual(log, sb.ToString());
    }

    [Test]
    [TestCase(LogLevel.Debug, "Debug: Invoking pipeline operation ({0}/{1}/{2})...Debug: Invoked pipeline operation ({0}/{1}/{2}).")]
    [TestCase(LogLevel.Info, "")]
    public async Task ProcessAsync_no_behaviors_no_context(LogLevel logLevel, string log)
    {
        var sb = new StringBuilder();
        var logger = Substitute.For<ILogger<AsyncPipeline<object?, object, object?>>>();
        logger.IsEnabled(Arg.Any<LogLevel>())
            .Returns(ci => ci.Arg<LogLevel>() <= logLevel);
        logger.When(l => l.Log(Arg.Any<LogLevel>(), Arg.Any<Exception?>(), Arg.Any<string?>(), Arg.Any<object?[]>()))
            .Do(ci => sb.Append($"{ci.Arg<LogLevel>()}: {ci.Arg<string>()}"));
        var pipeline = new AsyncPipeline<object?, object, object?>(GetPipelineContextFactory(), logger: logger);

        var target = new object();
        var args = new object();
        var token = new CancellationToken();
        var result = new object();

        Task<object?> Operation()
        {
            return Task.FromResult<object?>(result);
        }

        var pipelineResult = await pipeline.ProcessAsync(target, args, null, Operation, token);
        Assert.AreSame(result, pipelineResult);
        Assert.AreEqual(log, sb.ToString());
    }

    [Test]
    [TestCase(LogLevel.Debug, null, null, "test", "test", "Debug: Invoking pipeline behavior ({0}/{1}/{2}/{3})...b1Debug: Invoking pipeline behavior ({0}/{1}/{2}/{3})...b2Debug: Invoking pipeline operation ({0}/{1}/{2})...Debug: Invoked pipeline operation ({0}/{1}/{2}).Debug: Invoked pipeline behavior ({0}/{1}/{2}/{3})...Debug: Invoked pipeline behavior ({0}/{1}/{2}/{3})...")]
    [TestCase(LogLevel.Debug, "b1Test", null, "test", "b1Test", "Debug: Invoking pipeline behavior ({0}/{1}/{2}/{3})...b1Debug: Invoked pipeline behavior ({0}/{1}/{2}/{3})...")]
    [TestCase(LogLevel.Debug, null, "b2Test", "test", "b2Test", "Debug: Invoking pipeline behavior ({0}/{1}/{2}/{3})...b1Debug: Invoking pipeline behavior ({0}/{1}/{2}/{3})...b2Debug: Invoked pipeline behavior ({0}/{1}/{2}/{3})...Debug: Invoked pipeline behavior ({0}/{1}/{2}/{3})...")]
    [TestCase(LogLevel.Debug, "b1Test", "b2Test", "test", "b1Test", "Debug: Invoking pipeline behavior ({0}/{1}/{2}/{3})...b1Debug: Invoked pipeline behavior ({0}/{1}/{2}/{3})...")]
    [TestCase(LogLevel.Info, null, null, "test", "test", "b1b2")]
    [TestCase(LogLevel.Info, "b1Test", null, "test", "b1Test", "b1")]
    [TestCase(LogLevel.Info, null, "b2Test", "test", "b2Test", "b1b2")]
    [TestCase(LogLevel.Info, "b1Test", "b2Test", "test", "b1Test", "b1")]
    public async Task ProcessAsync_with_behaviors(LogLevel logLevel, string? b1Result, string? b2Result, string opResult, string result, string log)
    {
        var sb = new StringBuilder();
        var logger = Substitute.For<ILogger<AsyncPipeline<object?, object, object?>>>();
        logger.IsEnabled(Arg.Any<LogLevel>())
            .Returns(ci => ci.Arg<LogLevel>() <= logLevel);
        logger.When(l => l.Log(Arg.Any<LogLevel>(), Arg.Any<Exception?>(), Arg.Any<string?>(), Arg.Any<object?[]>()))
            .Do(ci => sb.Append($"{ci.Arg<LogLevel>()}: {ci.Arg<string>()}"));
        var b1 = Substitute.For<IAsyncPipelineBehavior<object?, object, object?>>();
        var b2 = Substitute.For<IAsyncPipelineBehavior<object?, object, object?>>();
        var selector = Substitute.For<ILazyEnumerable<IAsyncPipelineBehavior, PipelineBehaviorMetadata>>();
        selector.SelectServices(Arg.Any<Func<Lazy<IAsyncPipelineBehavior, PipelineBehaviorMetadata>, bool>>()).Returns(new[] { b1, b2 });
        var pipeline = new AsyncPipeline<object?, object, object?>(GetPipelineContextFactory(), selector, logger);

        var target = new object();
        var args = new object();
        var context = Substitute.For<IContext>();
        var token = new CancellationToken();

        b1.InvokeAsync(Arg.Any<Func<Task<object?>>>(), target, args, context, token)
            .Returns(ci =>
            {
                sb.Append("b1");
                return b1Result is not null
                    ? Task.FromResult<object?>(b1Result)
                    : ci.Arg<Func<object?, object, CancellationToken, Task<object?>>>()(target, args, token);
            });
            
        b2.InvokeAsync(Arg.Any<Func<Task<object?>>>(), target, args, context, token)
            .Returns(ci =>
            {
                sb.Append("b2");
                return b2Result is not null
                    ? Task.FromResult<object?>(b2Result)
                    : ci.Arg<Func<object?, object, CancellationToken, Task<object?>>>()(target, args, token);
            });
            
        Task<object?> Operation()
        {
            return Task.FromResult<object?>(opResult);
        }

        var pipelineResult = await pipeline.ProcessAsync(target, args, context, Operation, token);
        Assert.AreEqual(result, pipelineResult);
        Assert.AreEqual(log, sb.ToString());
    }

    private IExportFactory<PipelineContext> GetPipelineContextFactory()
    {
        var factory = Substitute.For<IExportFactory<PipelineContext>>();
        factory.CreateExportedValue().Returns(new PipelineContext(Substitute.For<IServiceProvider>()));
        return factory;
    }
}