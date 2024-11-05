// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogPipelineBehaviorTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using FluentAssertions;
using Kephas.Extensions.Mediator.PipelineBehaviors;
using Kephas.Testing;
using MediatR;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Kephas.Extensions.Tests.Mediator.PipelineBehaviors;

public class LogPipelineBehaviorTest
{
    [Fact]
    public async Task Handle_LogsRequestAndResponse_WhenLogLevelIsDebug()
    {
        var logger = Test.GetTestLogger<LogPipelineBehavior<IRequest<string>, string>>(LogLevel.Debug);
        var behavior = new LogPipelineBehavior<IRequest<string>, string>(logger);
        var request = Substitute.For<IRequest<string>>();
        var response = "response";

        var result = await behavior.Handle(request, () => Task.FromResult(response), CancellationToken.None);

        result.Should().Be(response);
        logger.ToString().Should().ContainAll(
            "Debug: Handling request: Substitute.IRequest<String>",
            "Debug: Handled request successfully: Substitute.IRequest<String>");
    }

    [Fact]
    public async Task Handle_DoesNotLog_WhenLogLevelIsNotDebug()
    {
        var logger = Test.GetTestLogger<LogPipelineBehavior<IRequest<string>, string>>(LogLevel.Information);
        var behavior = new LogPipelineBehavior<IRequest<string>, string>(logger);
        var request = Substitute.For<IRequest<string>>();
        var response = "response";

        var result = await behavior.Handle(request, () => Task.FromResult(response), CancellationToken.None);

        result.Should().Be(response);
        logger.ToString().Should().Be(
            """

            """);
    }

    [Fact]
    public async Task Handle_LogsException_WhenLogLevelIsDebug()
    {
        var logger = Test.GetTestLogger<LogPipelineBehavior<IRequest<string>, string>>(LogLevel.Debug);
        var behavior = new LogPipelineBehavior<IRequest<string>, string>(logger);
        var request = Substitute.For<IRequest<string>>();
        var exception = new Exception("Test exception");

        Func<Task> act = async () => await behavior.Handle(request, () => Task.FromException<string>(exception), CancellationToken.None);

        await act.Should().ThrowAsync<Exception>().WithMessage("Test exception");
        logger.ToString().Should().ContainAll(
            "Debug: Handling request: Substitute.IRequest<String>",
            "Exception Debug: Handling request failed: Substitute.IRequest<String>");
    }
}