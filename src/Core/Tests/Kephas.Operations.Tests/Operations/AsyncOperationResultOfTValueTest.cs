// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AsyncOperationResultOfTValueTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using FluentAssertions;
using Kephas.Operations;
using Xunit;

namespace Kephas.Tests.Operations;

public class AsyncOperationResultOfTValueTest
{
    [Fact]
    public void AsyncOperationResult_completed_task()
    {
        var opResult = new AsyncOperationResult<int>(Task.FromResult(12));

        Assert.Equal(OperationState.Completed, opResult.OperationState);
        Assert.Equal(1f, opResult.PercentCompleted);
        Assert.Equal(12, opResult.Value);
    }

    [Fact]
    public async Task AsyncOperationResult_task_completion_sets_state()
    {
        var opResult = new AsyncOperationResult<int>(Task.FromResult(12));
        await opResult.AsTask();

        Assert.Equal(OperationState.Completed, opResult.OperationState);
        Assert.Equal(1f, opResult.PercentCompleted);
        Assert.Equal(12, opResult.Value);
    }

    [Fact]
    public async Task AsyncOperationResult_task_completion_sets_state_delay()
    {
        var opResult = new AsyncOperationResult<int>(Task.Delay(100).ContinueWith(t => 12));
        await opResult.AsTask();

        Assert.Equal(OperationState.Completed, opResult.OperationState);
        Assert.Equal(1f, opResult.PercentCompleted);
        Assert.Equal(12, opResult.Value);
    }

    [Fact]
    public async Task AsyncOperationResult_task_completion_sets_state_exception()
    {
        var opResult = new AsyncOperationResult<int>(Task.FromException<int>(new ArgumentException("arg")));
        await opResult.AsTask().ContinueWith(t =>
        {
            Assert.Equal(OperationState.Failed, opResult.OperationState);
            Assert.Equal(0f, opResult.PercentCompleted);
            var ex = Assert.Throws<AggregateException>(() => { var _ = opResult.Value; });
            ex!.InnerException.Should().BeOfType<ArgumentException>();
        });
    }

    [Fact]
    public async Task AsyncOperationResult_task_completion_sets_state_canceled()
    {
        var opResult = new AsyncOperationResult<int>(Task.FromCanceled<int>(new CancellationToken(true)));
        await opResult.AsTask().ContinueWith(t =>
        {
            Assert.Equal(OperationState.Canceled, opResult.OperationState);
            Assert.Equal(0f, opResult.PercentCompleted);
            var ex = Assert.Throws<AggregateException>(() => { var _ = opResult.Value; });
            ex!.InnerException.Should().BeOfType<TaskCanceledException>();
        });
    }

    [Fact]
    public async Task GetAwaiter_default_result()
    {
        var opResult = new AsyncOperationResult<int>(Task.FromResult(10));
        var result = await opResult.AsTask();

        Assert.Equal(10, result);
    }

    [Fact]
    public async Task GetAwaiter_subsequent_result()
    {
        var opResult = new AsyncOperationResult<int>(Task.FromResult(0));
        var result = await opResult.AsTask();
        Assert.Equal(0, result);

        opResult.Value = 40;
        result = await opResult.AsTask();

        Assert.Equal(40, result);
    }

    [Fact]
    public async Task GetAwaiter_task()
    {
        var opResult = new AsyncOperationResult<int>(Task.FromResult(10));
        var result = await opResult.AsTask();

        Assert.Equal(10, result);
    }

    [Fact]
    public async Task AsTask()
    {
        var opResult = new AsyncOperationResult<int>(Task.FromResult(10));
        var result = await opResult.AsTask();

        Assert.Equal(10, result);
        Assert.Equal(10, opResult.Value);
    }

    [Fact]
    public void Deconstruct()
    {
        IOperationResult<string> result = new AsyncOperationResult<string>(Task.FromResult("hello")).Complete();
        var (value, state) = result;

        Assert.Equal("hello", value);
        Assert.Equal(OperationState.Completed, state);
    }
}
