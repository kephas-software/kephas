// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExpandoTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Test class for <see cref="Expando" />
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Tests.Dynamic;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Kephas.Dynamic;
using Xunit;

/// <summary>
/// Test class for <see cref="Expando"/>
/// </summary>
public class ExpandoTest
{
    [Fact]
    public void Constructor_default()
    {
        dynamic expando = new Expando();
    }

    [Fact]
    public void PublicProperty_default()
    {
        dynamic expando = new Expando();

        expando.Property = "value";
        Assert.Equal("value", expando.Property);
    }

    [Fact]
    public void PublicProperty_derived_expando()
    {
        dynamic derived = new DerivedExpando();
        derived.Age = 23;

        Assert.Equal(23, derived.Age);

        var typedDerived = (DerivedExpando)derived;
        Assert.Equal(23, typedDerived.Age);
    }

    [Fact]
    public void TryInvokeMember_Func_property()
    {
        dynamic instance = new Expando();
        instance.GetName = (Func<int, string>)(age => $"John Doe: {age}");

        var name = instance.GetName(30);
        Assert.Equal("John Doe: 30", name);
    }

    [Theory]
    [InlineData(12)]
    [InlineData(15.3)]
    [InlineData("23.45")]
    public void Dynamic_Property(object age)
    {
        dynamic expando = new Expando();
        expando.Age = age;

        Assert.Equal(age, expando.Age);
    }

    [Fact]
    public async Task Dynamic_Property_concurrent()
    {
        dynamic expando = new Expando(isThreadSafe: true);
        Action accessor = () =>
        {
            for (var i = 0; i <= 2000; i++)
            {
                expando["Prop" + Thread.CurrentThread.Name + i] = Thread.CurrentThread.Name + i;
            }
        };

        var tasks = new List<Task>();
        for (var j = 0; j < 200; j++)
        {
            tasks.Add(Task.Factory.StartNew(accessor));
        }

        await Task.WhenAll(tasks);
    }

    [Fact]
    public void Dynamic_Property_non_existing()
    {
        dynamic expando = new Expando();
        var age = expando.Age;
        Assert.Null(age);
    }

    [Fact]
    public void Indexer_Property_non_existing()
    {
        var expando = new Expando();
        var age = expando["Age"];
        Assert.Null(age);
    }

    [Fact]
    public void Indexer_set_to_null_removes_member()
    {
        var expando = new Expando();
        expando["Age"] = 12;
        Assert.True(expando.HasKey("Age"));

        expando["Age"] = null;
        Assert.False(expando.HasKey("Age"));
    }

    [Fact]
    public void Indexer_set_to_null_non_existing_member()
    {
        var expando = new Expando();
        Assert.False(expando.HasKey("Age"));

        expando["Age"] = null;
        Assert.False(expando.HasKey("Age"));
    }

    [Fact]
    public void HasDynamicMember_Property_existing_in_dictionary()
    {
        var expando = new Expando();
        expando["Age"] = 12;
        Assert.True(expando.HasKey("Age"));
    }

    [Fact]
    public void HasDynamicMember_Property_non_existing()
    {
        var expando = new Expando();
        Assert.False(expando.HasKey("Age"));
    }

    [Theory]
    [InlineData(12, false)]
    [InlineData(80, true)]
    public void TryInvokeMember_Method(int age, bool expectedResult)
    {
        var instance = new DerivedExpando { Age = age };
        var expando = instance.ToDynamicObject();

        var isOld = expando.IsOld();
        isOld.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData(12, false)]
    [InlineData(80, true)]
    public void TryInvokeMember_Method_with_instance(int age, bool expectedResult)
    {
        dynamic instance = new DerivedExpando { Age = age };

        var isOld = instance.IsOld();
        isOld.Should().Be(expectedResult);
    }

    [Fact]
    public void ToDictionary_wrapped()
    {
        var instance = new WrapperExpando(new TestClass());
        var dictionary = instance.ToDictionary();
        Assert.Equal(3, dictionary.Count);
        Assert.True((bool)dictionary[nameof(WrapperExpando.HasWrappedObject)]);
    }

    [Fact]
    public void ToDictionary_non_wrapped()
    {
        var instance = new WrapperExpando();
        var dictionary = instance.ToDictionary();
        Assert.Equal(1, dictionary.Count);
        Assert.False((bool)dictionary[nameof(WrapperExpando.HasWrappedObject)]);
    }

    public class TestClass
    {
        public string Name { get; set; }

        public string ReadOnlyFullName => this.ComputeFullName(string.Empty);

        private int PrivateAge { get; set; }

        public int GetPrivateAge() => this.PrivateAge;

        public void SetPrivateAge(int value) => this.PrivateAge = value;

        public virtual string ComputeFullName(string parentsInitials) => $"{parentsInitials} {this.Name}";
    }

    public class DerivedTestClass : TestClass
    {
        public string FamilyName { get; set; }

        public override string ComputeFullName(string parentsInitials)
        {
            return this.FamilyName + " " + base.ComputeFullName(parentsInitials);
        }
    }

    public class AgedPerson
    {
        public int Age { get; set; }

        public bool IsOld() => this.Age >= 70;
    }

    public class DerivedExpando : Expando
    {
        public int Age { get; set; }

        public bool IsOld() => this.Age >= 70;
    }

    public class WrapperExpando : ExpandoBase<object?>
    {
        public WrapperExpando()
        {
        }

        public WrapperExpando(TestClass innerObject)
            : base(innerObject)
        {
            this.HasWrappedObject = true;
        }

        public bool HasWrappedObject { get; }
    }
}
