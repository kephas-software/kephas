// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AsyncOperationResultTest.cs" company="Kephas Software SRL">
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

public class AsyncOperationResultTest
{
    [Fact]
    public async Task AsyncOperationResult_task_completion_sets_state()
    {
        var opResult = new AsyncOperationResult(Task.FromResult((object)12));
        await opResult.AsTask();

        Assert.Equal(OperationStatus.Completed, opResult.OperationStatus);
        Assert.Equal(1f, opResult.PercentCompleted);
        Assert.Equal(12, opResult.Value);
    }

    [Fact]
    public async Task AsyncOperationResult_task_completion_sets_state_delay()
    {
        var opResult = new AsyncOperationResult(Task.Delay(100).ContinueWith(t => 12));
        await opResult.AsTask();

        Assert.Equal(OperationStatus.Completed, opResult.OperationStatus);
        Assert.Equal(1f, opResult.PercentCompleted);
        Assert.Equal(12, opResult.Value);
    }

    [Fact]
    public async Task AsyncOperationResult_task_completion_sets_state_exception()
    {
        var opResult = new AsyncOperationResult(Task.FromException(new ArgumentException("arg")));
        await opResult.AsTask().ContinueWith(t =>
        {
            Assert.Equal(OperationStatus.Failed, opResult.OperationStatus);
            Assert.Equal(0f, opResult.PercentCompleted);
            var ex = Assert.Throws<AggregateException>(() => { var _ = opResult.Value; });
            ex!.InnerException.Should().BeOfType<ArgumentException>();
        });
    }

    [Fact]
    public async Task AsyncOperationResult_task_completion_sets_state_canceled()
    {
        var opResult = new AsyncOperationResult(Task.FromCanceled(new CancellationToken(true)));
        await opResult.AsTask().ContinueWith(t =>
        {
            Assert.Equal(OperationStatus.Canceled, opResult.OperationStatus);
            Assert.Equal(0f, opResult.PercentCompleted);
            var ex = Assert.Throws<AggregateException>(() => { var _ = opResult.Value; });
            ex!.InnerException.Should().BeOfType<TaskCanceledException>();
        });
    }

    [Fact]
    public async Task GetAwaiter_no_result()
    {
        var opResult = new AsyncOperationResult(Task.CompletedTask);
        var result = await opResult.AsTask();

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAwaiter_subsequent_result()
    {
        var opResult = new AsyncOperationResult(Task.CompletedTask);
        var result = await opResult.AsTask();
        result.Should().BeNull();

        opResult.Value = 40;
        result = await opResult.AsTask();

        Assert.Equal(40, result);
    }

    [Fact]
    public async Task GetAwaiter_task()
    {
        var opResult = new AsyncOperationResult(Task.FromResult((object)12));
        var result = await opResult.AsTask();

        Assert.Equal(12, result);
    }

    [Fact]
    public async Task AsTask()
    {
        var opResult = new AsyncOperationResult(Task.FromResult((object)12));
        await opResult.AsTask();

        Assert.Equal(12, opResult.Value);
    }

    [Fact]
    public void Messages_Clear()
    {
        var opResult = new AsyncOperationResult(Task.CompletedTask).MergeException(new InvalidOperationException());

        opResult.Messages.Should().NotBeEmpty();
        opResult.Messages.Clear();
        opResult.Messages.Should().BeEmpty();
    }

    [Fact]
    public void Deconstruct()
    {
        IOperationResult result = new AsyncOperationResult(Task.FromResult("hello"));
        var (value, state) = result;

        value.Should().BeNull();
        state.Should().Be(OperationStatus.Completed);
    }
}
