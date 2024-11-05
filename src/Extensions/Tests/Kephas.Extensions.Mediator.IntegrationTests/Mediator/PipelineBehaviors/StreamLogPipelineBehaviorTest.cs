// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StreamLogPipelineBehaviorTest.cs" company="Kephas Software SRL">
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

public class StreamLogPipelineBehaviorTest
{
    [Fact]
    public void Handle_LogsRequestAndResponse_WhenLogLevelIsDebug()
    {
        var logger = Test.GetTestLogger<StreamLogPipelineBehavior<IStreamRequest<string>, string>>(LogLevel.Debug);
        var behavior = new StreamLogPipelineBehavior<IStreamRequest<string>, string>(logger);
        var request = Substitute.For<IStreamRequest<string>>();
        var response = Substitute.For<IAsyncEnumerable<string>>();

        var result = behavior.Handle(request, () => response, CancellationToken.None);

        result.Should().Be(response);
        logger.ToString().Should().ContainAll(
            "Debug: Handling stream request: Substitute.IStreamRequest<String>",
            "Debug: Handled stream request successfully: Substitute.IStreamRequest<String>");
    }

    [Fact]
    public void Handle_DoesNotLog_WhenLogLevelIsNotDebug()
    {
        var logger = Test.GetTestLogger<StreamLogPipelineBehavior<IStreamRequest<string>, string>>(LogLevel.Information);
        var behavior = new StreamLogPipelineBehavior<IStreamRequest<string>, string>(logger);
        var request = Substitute.For<IStreamRequest<string>>();
        var response = Substitute.For<IAsyncEnumerable<string>>();

        var result = behavior.Handle(request, () => response, CancellationToken.None);

        result.Should().Be(response);
        logger.ToString().Should().Be(
            """

            """);
    }

    [Fact]
    public void Handle_LogsException_WhenLogLevelIsDebug()
    {
        var logger = Test.GetTestLogger<StreamLogPipelineBehavior<IStreamRequest<string>, string>>(LogLevel.Debug);
        var behavior = new StreamLogPipelineBehavior<IStreamRequest<string>, string>(logger);
        var request = Substitute.For<IStreamRequest<string>>();
        var exception = new Exception("Test exception");

        Func<IAsyncEnumerable<string>> act = () => behavior.Handle(request, () => throw exception, CancellationToken.None);

        act.Should().Throw<Exception>().WithMessage("Test exception");
        logger.ToString().Should().ContainAll(
            "Debug: Handling stream request: Substitute.IStreamRequest<String>",
            "Exception Debug: Handling stream request failed: Substitute.IStreamRequest<String>");
    }
}