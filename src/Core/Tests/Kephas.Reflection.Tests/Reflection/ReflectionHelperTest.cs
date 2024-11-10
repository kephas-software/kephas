// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReflectionHelperTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Linq.Expressions;
using System.Reflection;
using FluentAssertions;
using Kephas.Reflection;
using Xunit;

namespace Kephas.Tests.Reflection;

public class ReflectionHelperTests
{
    [Fact]
    public void Call_ReturnsCorrectResult()
    {
        var methodInfo = typeof(string).GetMethod("ToUpper", Type.EmptyTypes);
        var result = methodInfo.Call<string>("test");
        result.Should().Be("TEST");
    }

    [Fact]
    public void Call_ThrowsOriginalException()
    {
        var methodInfo = typeof(string).GetMethod("Substring", new[] { typeof(int), typeof(int) });
        Action act = () => methodInfo.Call<string>("test", 10, 1);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void GetPropertyName_ReturnsCorrectPropertyName()
    {
        Expression<Func<string, object>> expression = s => s.Length;
        var result = ReflectionHelper.GetPropertyName(expression);
        result.Should().Be("Length");
    }

    [Fact]
    public void GetPropertyName_ThrowsArgumentExceptionForInvalidExpression()
    {
        Expression<Func<string, object>> expression = s => s.ToUpper();
        Action act = () => ReflectionHelper.GetPropertyName(expression);
        act.Should().Throw<ArgumentException>().WithMessage("Expected property expression.");
    }

    [Fact]
    public void GetStaticPropertyName_ReturnsCorrectPropertyName()
    {
        Expression<Func<object>> expression = () => string.Empty.Length;
        var result = ReflectionHelper.GetStaticPropertyName(expression);
        result.Should().Be("Length");
    }

    [Fact]
    public void GetStaticPropertyName_ThrowsArgumentExceptionForInvalidExpression()
    {
        Expression<Func<object>> expression = () => string.Empty.ToUpper();
        Action act = () => ReflectionHelper.GetStaticPropertyName(expression);
        act.Should().Throw<ArgumentException>().WithMessage("Expected property expression");
    }

    [Fact]
    public void GetMethodOf_ReturnsCorrectMethodInfo()
    {
        Expression<Func<object, string>> expression = obj => obj.ToString();
        var result = ReflectionHelper.GetMethodOf(expression);
        result.Name.Should().Be("ToString");
    }

    [Fact]
    public void GetNonGenericFullName_ReturnsCorrectFullNameForGenericType()
    {
        var typeInfo = typeof(Dictionary<int, string>).GetTypeInfo();
        var result = typeInfo.GetNonGenericFullName();
        result.Should().Be("System.Collections.Generic.Dictionary");
    }

    [Fact]
    public void GetNonGenericFullName_ReturnsCorrectFullNameForNonGenericType()
    {
        var typeInfo = typeof(string).GetTypeInfo();
        var result = typeInfo.GetNonGenericFullName();
        result.Should().Be("System.String");
    }

    [Fact]
    public void CreateStaticDelegate_CreatesCorrectDelegate()
    {
        var methodInfo = typeof(string).GetMethod("IsNullOrEmpty", new[] { typeof(string) });
        var del = methodInfo.CreateStaticDelegate<Func<string, bool>>();
        del("test").Should().BeFalse();
    }

    [Fact]
    public void GetPublicInstanceProperties_ReturnsCorrectProperties()
    {
        var properties = typeof(SampleClass).GetPublicInstanceProperties();
        properties.Should().ContainSingle(p => p.Name == "SampleProperty");
    }

    [Theory]
    [MemberData(nameof(GetPropertyValue_TestData))]
    public void GetPropertyValue_simple(object target, string name, object? expectedValue)
    {
        target.GetPropertyValue(name).Should().Be(expectedValue);
    }

    public static TheoryData<object, string, object?> GetPropertyValue_TestData() =>
        new()
        {
            { typeof(string), "Name", "String" },
            { "123", "Length", 3 },
            { new SampleClass { SampleProperty = "Hello" }, "SampleProperty", "Hello" },
            { new Person("John Doe", 23), "Name", "John Doe" },
        };

    [Theory]
    [MemberData(nameof(SetPropertyValue_TestData))]
    public void SetPropertyValue_simple(object instance, string name, object? newValue)
    {
        instance.SetPropertyValue(name, newValue);
        instance.GetPropertyValue(name).Should().Be(newValue);
    }

    public static TheoryData<object, string, object?> SetPropertyValue_TestData() =>
        new()
        {
            { new Person("John Doe", 23), "Name", "Jane Doe" },
            { new SampleClass { SampleProperty = "Hello" }, "SampleProperty", "There" },
        };

    public record Person(string? Name, int Age);

    public class SampleClass
    {
        public string SampleProperty { get; set; }
        public static string StaticProperty { get; set; }
    }
}