// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumerableExtensionsTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests.Collections;

using System.Text;

using Kephas.Collections;
using NUnit.Framework;

[TestFixture]
public class EnumerableExtensionsTest
{
    [Test]
    public void ForEach_sync()
    {
        var sb = new StringBuilder();
        new[] { 1, 2, 3 }.ForEach(v => sb.Append(v));

        Assert.AreEqual("123", sb.ToString());
    }

    [Test]
    public async Task ForEach_async()
    {
        var sb = new StringBuilder();
        await new[] { 1, 2, 3 }.ForEach(async v => sb.Append(v));

        Assert.AreEqual("123", sb.ToString());
    }

    [Test]
    public async Task ForEach_async_cancellation()
    {
        var sb = new StringBuilder();
        await new[] { 1, 2, 3 }.ForEach(async (v, token) => sb.Append(v));

        Assert.AreEqual("123", sb.ToString());
    }

    [Test]
    public void ForEach_async_cancellation_exception_thrown_by_foreach()
    {
        var source = new CancellationTokenSource();
        source.CancelAfter(30);
        Assert.ThrowsAsync<OperationCanceledException>(
            () => new[] { 1, 2, 3 }.ForEach(async (v, token) => await Task.Delay(20), source.Token));
    }

    [Test]
    public void ForEach_async_cancellation_exception_thrown_by_func()
    {
        var source = new CancellationTokenSource();
        source.CancelAfter(20);
        Assert.ThrowsAsync<TaskCanceledException>(
            () => new[] { 1, 2, 3 }.ForEach(async (v, token) => await Task.Delay(60, source.Token), source.Token));
    }
}