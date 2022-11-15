// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObjectExpandoTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Tests.Dynamic;

using Kephas.Data;
using Kephas.Dynamic;
using NSubstitute;
using NUnit.Framework;

[TestFixture]
public class ObjectExpandoTest
{
    [Test]
    public void Constructor_instance()
    {
        var instance = new ExpandoTest.TestClass();
        dynamic expando = new ObjectExpando(instance);
    }

    [Test]
    public void PublicProperty_instance()
    {
        var instance = new ExpandoTest.TestClass();
        dynamic expando = new ObjectExpando(instance);

        expando.Name = "John";
        Assert.AreEqual("John", expando.Name);
        Assert.AreEqual("John", instance.Name);
    }

    [Test]
    public void ReadOnlyProperty_instance_getter()
    {
        var instance = new ExpandoTest.TestClass();
        dynamic expando = new ObjectExpando(instance);

        expando.Name = "John";
        Assert.AreEqual(" John", expando.ReadOnlyFullName);
        Assert.AreEqual(" John", instance.ReadOnlyFullName);
    }


    [Test]
    public void ReadOnlyProperty_instance_setter()
    {
        var instance = new ExpandoTest.TestClass();
        dynamic expando = new ObjectExpando(instance);

        Assert.Throws<MemberAccessException>(() => expando.ReadOnlyFullName = "John Doe");
    }

    [Test]
    public void PrivateProperty_instance_getter()
    {
        var instance = new ExpandoTest.TestClass();
        dynamic expando = new ObjectExpando(instance);
        var value = expando.PrivateAge;
        instance.SetPrivateAge(10);
        var objValue = instance.GetPrivateAge();

        Assert.AreEqual(10, objValue);
        Assert.AreEqual(null, value);
    }

    [Test]
    public void PrivateProperty_instance_setter()
    {
        var instance = new ExpandoTest.TestClass();
        dynamic expando = new ObjectExpando(instance);
        var value = expando.PrivateAge;
        expando.PrivateAge = 10;
        var objValue = instance.GetPrivateAge();

        Assert.AreNotEqual(value, objValue);
        Assert.AreEqual(10, expando.PrivateAge);
        Assert.AreEqual(0, instance.GetPrivateAge());
    }

    [Test]
    public void TryInvokeMember_Func_property_with_instance()
    {
        dynamic instance = new ObjectExpando(new ExpandoTest.TestClass());
        instance.GetName = (Func<int, string>)(age => $"John Doe: {age}");

        var name = instance.GetName(30);
        Assert.AreEqual("John Doe: 30", name);
    }

    [Test]
    public void HasDynamicMember_Property_existing_in_object()
    {
        var expando = new ObjectExpando(Substitute.For<IIdentifiable>());
        Assert.IsTrue(expando.HasDynamicMember(nameof(IIdentifiable.Id)));
    }

    [Test]
    public void HasDynamicMember_Property_existing_in_dictionary_not_object()
    {
        var expando = new ObjectExpando(Substitute.For<IIdentifiable>());
        expando["Age"] = 12;
        Assert.IsTrue(expando.HasDynamicMember("Age"));
    }

    [Test]
    public void HasDynamicMember_Property_non_existing_in_object()
    {
        var expando = new ObjectExpando(Substitute.For<IIdentifiable>());
        Assert.IsFalse(expando.HasDynamicMember("Age"));
    }

    [Test]
    [TestCase(12, ExpectedResult = false)]
    [TestCase(80, ExpectedResult = true)]
    public bool TryInvokeMember_Method_with_instance(int age)
    {
        var instance = new ExpandoTest.AgedPerson { Age = age };
        dynamic expando = new ObjectExpando(instance);

        var isOld = expando.IsOld();
        return isOld;
    }
}