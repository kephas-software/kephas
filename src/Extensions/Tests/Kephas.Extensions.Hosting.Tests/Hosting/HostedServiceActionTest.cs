// --------------------------------------------------------------------------------------------------------------------
// <copyright file="C.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using FluentAssertions;
using Kephas.Extensions.Hosting;
using Kephas.Testing;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Kephas.Extensions.Tests.Hosting;

public class HostedServiceActionTest
{
    [Fact]
    public void CanProceed_ReturnsTrue_WhenNoDependencies()
    {
        var action = new HostedServiceAction(
            typeof(object),
            new List<Type>(),
            () => Task.CompletedTask,
            "TestAction",
            Substitute.For<ILogger>());

        action.CanProceed().Should().BeTrue();
    }

    [Fact]
    public void CanProceed_ReturnsFalse_WhenDependenciesExist()
    {
        var action = new HostedServiceAction(
            typeof(object),
            new List<Type> { typeof(string) },
            () => Task.CompletedTask,
            "TestAction",
            Substitute.For<ILogger>());

        action.CanProceed().Should().BeFalse();
    }

    [Fact]
    public async Task Proceed_ExecutesActionAndNext()
    {
        var actionExecuted = false;
        var nextExecuted = false;

        var action = new HostedServiceAction(
            typeof(object),
            new List<Type>(),
            () => { actionExecuted = true; return Task.CompletedTask; },
            "TestAction",
            Substitute.For<ILogger>());

        await action.Proceed(_ => { nextExecuted = true; return Task.CompletedTask; });

        actionExecuted.Should().BeTrue();
        nextExecuted.Should().BeTrue();
    }

    [Fact]
    public async Task Proceed_LogsAndThrows_WhenActionThrows()
    {
        var logger = Test.GetTestLogger<HostedServiceAction>();
        var action = new HostedServiceAction(
            typeof(object),
            new List<Type>(),
            () => throw new InvalidOperationException("Test exception"),
            "TestAction",
            logger);

        Func<Task> act = () => action.Proceed(_ => Task.CompletedTask);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("Test exception");
        logger.ToString().Should().Contain("InvalidOperationException Critical: Errors occured during TestAction of System.Object. Further processing is stopped.");
    }

    [Fact]
    public void NotifyCompleted_RemovesCompletedDependencies()
    {
        var dependencies = new List<Type> { typeof(string), typeof(int) };
        var action = new HostedServiceAction(
            typeof(object),
            dependencies,
            () => Task.CompletedTask,
            "TestAction",
            Substitute.For<ILogger>());

        action.NotifyCompleted(new List<Type> { typeof(string) });

        dependencies.Should().ContainSingle().And.Contain(typeof(int));
    }
}