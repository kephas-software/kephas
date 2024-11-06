// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeExtensionsTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using FluentAssertions;
using Kephas.Reflection;
using Xunit;

namespace Kephas.Tests.Reflection;

public class TypeExtensionsTest
{
    [Fact]
    public void ToNormalizedType_ReturnsGenericTypeDefinition_WhenTypeContainsGenericParameters()
    {
        var type = typeof(Dictionary<,>);
        var result = type.ToNormalizedType();
        result.Should().Be(typeof(Dictionary<,>));
    }

    [Fact]
    public void ToNormalizedType_ReturnsSameType_WhenTypeIsNotGeneric()
    {
        var type = typeof(string);
        var result = type.ToNormalizedType();
        result.Should().Be(type);
    }

    [Fact]
    public void GetNonNullableType_ReturnsUnderlyingType_WhenTypeIsNullable()
    {
        var type = typeof(int?);
        var result = type.GetNonNullableType();
        result.Should().Be(typeof(int));
    }

    [Fact]
    public void GetNonNullableType_ReturnsSameType_WhenTypeIsNotNullable()
    {
        var type = typeof(int);
        var result = type.GetNonNullableType();
        result.Should().Be(type);
    }

    [Fact]
    public void IsNullableType_ReturnsTrue_WhenTypeIsNullable()
    {
        var type = typeof(int?);
        var result = type.IsNullableType();
        result.Should().BeTrue();
    }

    [Fact]
    public void IsNullableType_ReturnsFalse_WhenTypeIsNotNullable()
    {
        var type = typeof(int);
        var result = type.IsNullableType();
        result.Should().BeFalse();
    }

    [Fact]
    public void IsEnumerable_ReturnsTrue_WhenTypeImplementsIEnumerable()
    {
        var type = typeof(List<int>);
        var result = type.IsEnumerable();
        result.Should().BeTrue();
    }

    [Fact]
    public void IsEnumerable_ReturnsFalse_WhenTypeDoesNotImplementIEnumerable()
    {
        var type = typeof(int);
        var result = type.IsEnumerable();
        result.Should().BeFalse();
    }

    [Fact]
    public void IsReadOnlyCollection_ReturnsTrue_WhenTypeImplementsIReadOnlyCollection()
    {
        var type = typeof(List<int>);
        var result = type.IsReadOnlyCollection();
        result.Should().BeTrue();
    }

    [Fact]
    public void IsReadOnlyCollection_ReturnsFalse_WhenTypeDoesNotImplementIReadOnlyCollection()
    {
        var type = typeof(int);
        var result = type.IsReadOnlyCollection();
        result.Should().BeFalse();
    }

    [Fact]
    public void IsCollection_ReturnsTrue_WhenTypeImplementsICollection()
    {
        var type = typeof(List<int>);
        var result = type.IsCollection();
        result.Should().BeTrue();
    }

    [Fact]
    public void IsCollection_ReturnsFalse_WhenTypeDoesNotImplementICollection()
    {
        var type = typeof(int);
        var result = type.IsCollection();
        result.Should().BeFalse();
    }

    [Fact]
    public void IsDictionary_ReturnsTrue_WhenTypeImplementsIDictionary()
    {
        var type = typeof(Dictionary<int, string>);
        var result = type.IsDictionary();
        result.Should().BeTrue();
    }

    [Fact]
    public void IsDictionary_ReturnsFalse_WhenTypeDoesNotImplementIDictionary()
    {
        var type = typeof(int);
        var result = type.IsDictionary();
        result.Should().BeFalse();
    }

    [Fact]
    public void IsQueryable_ReturnsTrue_WhenTypeImplementsIQueryable()
    {
        var type = typeof(IQueryable<int>);
        var result = type.IsQueryable();
        result.Should().BeTrue();
    }

    [Fact]
    public void IsQueryable_ReturnsFalse_WhenTypeDoesNotImplementIQueryable()
    {
        var type = typeof(int);
        var result = type.IsQueryable();
        result.Should().BeFalse();
    }

    [Fact]
    public void TryGetDictionaryKeyItemType_ReturnsKeyAndItemType_WhenTypeIsDictionary()
    {
        var type = typeof(Dictionary<int, string>);
        var result = type.TryGetDictionaryKeyItemType();
        result.Should().Be((typeof(int), typeof(string)));
    }

    [Fact]
    public void TryGetDictionaryKeyItemType_ReturnsNull_WhenTypeIsNotDictionary()
    {
        var type = typeof(int);
        var result = type.TryGetDictionaryKeyItemType();
        result.Should().BeNull();
    }

    [Fact]
    public void TryGetEnumerableItemType_ReturnsItemType_WhenTypeIsEnumerable()
    {
        var type = typeof(List<int>);
        var result = type.TryGetEnumerableItemType();
        result.Should().Be(typeof(int));
    }

    [Fact]
    public void TryGetEnumerableItemType_ReturnsNull_WhenTypeIsNotEnumerable()
    {
        var type = typeof(int);
        var result = type.TryGetEnumerableItemType();
        result.Should().BeNull();
    }

    [Fact]
    public void TryGetQueryableItemType_ReturnsItemType_WhenTypeIsQueryable()
    {
        var type = typeof(IQueryable<int>);
        var result = type.TryGetQueryableItemType();
        result.Should().Be(typeof(int));
    }

    [Fact]
    public void TryGetQueryableItemType_ReturnsNull_WhenTypeIsNotQueryable()
    {
        var type = typeof(int);
        var result = type.TryGetQueryableItemType();
        result.Should().BeNull();
    }

    [Fact]
    public void TryGetCollectionItemType_ReturnsItemType_WhenTypeIsCollection()
    {
        var type = typeof(List<int>);
        var result = type.TryGetCollectionItemType();
        result.Should().Be(typeof(int));
    }

    [Fact]
    public void TryGetCollectionItemType_ReturnsNull_WhenTypeIsNotCollection()
    {
        var type = typeof(int);
        var result = type.TryGetCollectionItemType();
        result.Should().BeNull();
    }

    [Fact]
    public void IsConstructedGenericOf_ReturnsTrue_WhenTypeIsConstructedGenericOfOpenGenericType()
    {
        var type = typeof(List<int>);
        var result = type.IsConstructedGenericOf(typeof(List<>));
        result.Should().BeTrue();
    }

    [Fact]
    public void IsConstructedGenericOf_ReturnsFalse_WhenTypeIsNotConstructedGenericOfOpenGenericType()
    {
        var type = typeof(int);
        var result = type.IsConstructedGenericOf(typeof(List<>));
        result.Should().BeFalse();
    }

    [Fact]
    public void TryGetBaseConstructedGenericOf_ReturnsBaseConstructedGeneric_WhenTypeIsConstructedGenericOfOpenGenericType()
    {
        var type = typeof(List<int>);
        var result = type.TryGetBaseConstructedGenericOf(typeof(IEnumerable<>));
        result.Should().Be(typeof(IEnumerable<int>));
    }

    [Fact]
    public void TryGetBaseConstructedGenericOf_ReturnsNull_WhenTypeIsNotConstructedGenericOfOpenGenericType()
    {
        var type = typeof(int);
        var result = type.TryGetBaseConstructedGenericOf(typeof(IEnumerable<>));
        result.Should().BeNull();
    }

    [Fact]
    public void GetQualifiedFullName_ReturnsQualifiedFullName_WhenTypeIsProvided()
    {
        var type = typeof(int);
        var result = type.GetQualifiedFullName();
        result.Should().Be("System.Int32, System.Private.CoreLib");
    }

    [Fact]
    public void GetQualifiedFullName_ReturnsQualifiedFullNameWithoutVersionInfo_WhenStripVersionInfoIsTrue()
    {
        var type = typeof(int);
        var result = type.GetQualifiedFullName(true);
        result.Should().Be("System.Int32, System.Private.CoreLib");
    }
}