// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProducerConsumerCollectionExtensionsTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Concurrent;
using FluentAssertions;
using Kephas.Collections;
using Xunit;

namespace Kephas.Tests.Collections;

public class ProducerConsumerCollectionExtensionsTest
{
    [Fact]
    public void AddRange_AddsAllItemsToCollection()
    {
        var collection = new ConcurrentBag<int>();
        var items = new List<int> { 1, 2, 3 };

        collection.AddRange(items);

        collection.Should().Contain(items);
    }

    [Fact]
    public void AddRange_ReturnsSameCollection()
    {
        var collection = new ConcurrentBag<int>();
        var items = new List<int> { 1, 2, 3 };

        var result = collection.AddRange(items);

        result.Should().BeSameAs(collection);
    }

    [Fact]
    public void AddRange_ThrowsArgumentNullException_WhenCollectionIsNull()
    {
        ConcurrentBag<int> collection = null;
        var items = new List<int> { 1, 2, 3 };

        Action act = () => collection.AddRange(items);

        act.Should().Throw<ArgumentNullException>().WithMessage("*collection*");
    }

    [Fact]
    public void AddRange_DoesNothing_WhenItemsIsNull()
    {
        var collection = new ConcurrentBag<int>();
        IEnumerable<int> items = null;

        var result = collection.AddRange(items);

        collection.Should().BeEmpty();
        result.Should().BeSameAs(collection);
    }

    private class FailingProducerConsumerCollection<T> : IProducerConsumerCollection<T>
    {
        public bool TryAdd(T item) => false;
        public bool TryTake(out T item) { item = default; return false; }
        public T[] ToArray() => Array.Empty<T>();
        public void CopyTo(T[] array, int index) { }
        public void CopyTo(Array array, int index) { }
        public int Count => 0;
        public bool IsSynchronized => false;
        public object SyncRoot => new object();
        public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)Array.Empty<T>()).GetEnumerator();
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
    }
}