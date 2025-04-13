// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CollectionExtensionsTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Collections;
using Xunit;
using FluentAssertions;

namespace Kephas.Tests.Collections;

public class CollectionExtensionsTest
{
    [Fact]
    public void AddRange_AddsItemsToCollection()
    {
        var collection = new List<int>();
        var items = new[] { 1, 2, 3 };

        collection.AddRange(items);

        collection.Should().HaveCount(3);
        collection.Should().Contain(new[] { 1, 2, 3 });
    }

    [Fact]
    public async Task SingleOrDefaultAsync_ReturnsSingleElement()
    {
        var source = new List<int> { 1 }.AsQueryable();

        var result = await source.SingleOrDefaultAsync();

        result.Should().Be(1);
    }

    [Fact]
    public async Task SingleOrDefaultAsync_EmptySource_ReturnsDefault()
    {
        var source = new List<object>().AsQueryable();

        var result = await source.SingleOrDefaultAsync();

        result.Should().BeNull();
    }

    [Fact]
    public async Task SingleOrDefaultAsync_MultipleElements_ThrowsException()
    {
        var source = new List<object> { 1, 2 }.AsQueryable();

        await FluentActions.Awaiting(() => source.SingleOrDefaultAsync().AsTask())
            .Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task FirstOrDefaultAsync_ReturnsFirstElement()
    {
        var source = new List<int> { 1, 2, 3 }.AsQueryable();

        var result = await source.FirstOrDefaultAsync();

        result.Should().Be(1);
    }

    [Fact]
    public async Task FirstOrDefaultAsync_EmptySource_ReturnsDefault()
    {
        var source = new List<object>().AsQueryable();

        var result = await source.FirstOrDefaultAsync();

        result.Should().BeNull();
    }

    [Fact]
    public async Task SingleOrDefaultAsync_WithPredicate_ReturnsSingleElement()
    {
        var source = new List<int> { 1, 2, 3 }.AsQueryable();

        var result = await source.SingleOrDefaultAsync(x => x == 2);

        result.Should().Be(2);
    }

    [Fact]
    public async Task FirstOrDefaultAsync_WithPredicate_ReturnsFirstElement()
    {
        var source = new List<int> { 1, 2, 3 }.AsQueryable();

        var result = await source.FirstOrDefaultAsync(x => x > 1);

        result.Should().Be(2);
    }
}