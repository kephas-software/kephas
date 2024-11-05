// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObjectExpandoTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Dynamic;

using Kephas.Dynamic;
using NSubstitute;
using Xunit;

namespace Kephas.Tests.Dynamic;

public class ObjectExpandoTest
{
    [Fact]
    public void Constructor_instance()
    {
        var instance = new ExpandoTest.TestClass();
        dynamic expando = new ObjectExpando(instance);
    }

    [Fact]
    public void PublicProperty_instance()
    {
        var instance = new ExpandoTest.TestClass();
        dynamic expando = new ObjectExpando(instance);

        expando.Name = "John";
        Assert.Equal("John", expando.Name);
        Assert.Equal("John", instance.Name);
    }

    [Fact]
    public void ReadOnlyProperty_instance_getter()
    {
        var instance = new ExpandoTest.TestClass();
        dynamic expando = new ObjectExpando(instance);

        expando.Name = "John";
        Assert.Equal(" John", expando.ReadOnlyFullName);
        Assert.Equal(" John", instance.ReadOnlyFullName);
    }


    [Fact]
    public void ReadOnlyProperty_instance_setter()
    {
        var instance = new ExpandoTest.TestClass();
        dynamic expando = new ObjectExpando(instance);

        Assert.Throws<MemberAccessException>(() => expando.ReadOnlyFullName = "John Doe");
    }

    [Fact]
    public void PrivateProperty_instance_getter()
    {
        var instance = new ExpandoTest.TestClass();
        dynamic expando = new ObjectExpando(instance);
        var value = expando.PrivateAge;
        instance.SetPrivateAge(10);
        var objValue = instance.GetPrivateAge();

        Assert.Equal(10, objValue);
        Assert.Null(value);
    }

    [Fact]
    public void PrivateProperty_instance_setter()
    {
        var instance = new ExpandoTest.TestClass();
        dynamic expando = new ObjectExpando(instance);
        var value = expando.PrivateAge;
        expando.PrivateAge = 10;
        var objValue = instance.GetPrivateAge();

        Assert.NotEqual<object>(value, objValue);
        Assert.Equal(10, expando.PrivateAge);
        Assert.Equal(0, instance.GetPrivateAge());
    }

    [Fact]
    public void TryInvokeMember_Func_property_with_instance()
    {
        dynamic instance = new ObjectExpando(new ExpandoTest.TestClass());
        instance.GetName = (Func<int, string>)(age => $"John Doe: {age}");

        var name = instance.GetName(30);
        Assert.Equal("John Doe: 30", name);
    }

    [Fact]
    public void HasDynamicMember_Property_existing_in_object()
    {
        var expando = new ObjectExpando(Substitute.For<ITestIdentifiable>());
        Assert.True(expando.HasKey(nameof(ITestIdentifiable.Id)));
    }

    [Fact]
    public void HasDynamicMember_Property_existing_in_dictionary_not_object()
    {
        var expando = new ObjectExpando(Substitute.For<ITestIdentifiable>());
        expando["Age"] = 12;
        Assert.True(expando.HasKey("Age"));
    }

    [Fact]
    public void HasDynamicMember_Property_non_existing_in_object()
    {
        var expando = new ObjectExpando(Substitute.For<ITestIdentifiable>());
        Assert.False(expando.HasKey("Age"));
    }

    [Theory]
    [InlineData(12, false)]
    [InlineData(80, true)]
    public void TryInvokeMember_Method_with_instance(int age, bool expectedResult)
    {
        var instance = new ExpandoTest.AgedPerson { Age = age };
        dynamic expando = new ObjectExpando(instance);

        var isOld = expando.IsOld();
        Assert.Equal(expectedResult, isOld);
    }

    [Fact]
    public void GetKeys()
    {
        var instance = new ExpandoTest.AgedPerson { Age = 12 };
        IDynamic expando = new ObjectExpando(instance);

        Assert.Equivalent(new[] { "Age" }, expando.GetKeys());
    }

    [Fact]
    public void GetDynamicMemberNames()
    {
        var instance = new ExpandoTest.AgedPerson { Age = 12 };
        DynamicObject expando = new ObjectExpando(instance);

        Assert.Equivalent(new[] { "Age" }, expando.GetDynamicMemberNames());
    }

    public interface ITestIdentifiable
    {
        object Id { get; }
    }
}
