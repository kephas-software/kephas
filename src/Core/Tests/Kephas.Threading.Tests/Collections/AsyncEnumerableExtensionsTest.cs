// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AsyncEnumerableExtensionsTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Tests.Collections;

using System.Text;
using Kephas.Collections;
using Xunit;

public class AsyncEnumerableExtensionsTest
{
    [Fact]
    public async Task ForEach_async()
    {
        var sb = new StringBuilder();
        await new[] { 1, 2, 3 }.ForEach(async v => sb.Append(v));

        Assert.Equal("123", sb.ToString());
    }

    [Fact]
    public async Task ForEach_async_cancellation()
    {
        var sb = new StringBuilder();
        await new[] { 1, 2, 3 }.ForEach(async (v, token) => sb.Append(v));

        Assert.Equal("123", sb.ToString());
    }

    [Fact]
    public async Task ForEach_async_cancellation_exception_thrown_by_foreach()
    {
        using var source = new CancellationTokenSource();
        source.CancelAfter(30);
        await Assert.ThrowsAsync<OperationCanceledException>(
            () => new[] { 1, 2, 3 }.ForEach(async (v, token) => await Task.Delay(20), source.Token));
    }

    [Fact]
    public async Task ForEach_async_cancellation_exception_thrown_by_func()
    {
        using var source = new CancellationTokenSource();
        source.CancelAfter(20);
        await Assert.ThrowsAsync<TaskCanceledException>(
            () => new[] { 1, 2, 3 }.ForEach(async (v, token) => await Task.Delay(60, source.Token), source.Token));
    }
}
