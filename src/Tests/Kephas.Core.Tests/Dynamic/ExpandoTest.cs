// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExpandoTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Test class for <see cref="Expando" />
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests.Dynamic
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Data;
    using Kephas.Dynamic;

    using NSubstitute;

    using NUnit.Framework;

    /// <summary>
    /// Test class for <see cref="Expando"/>
    /// </summary>
    [TestFixture]
    public class ExpandoTest
    {
        [Test]
        public void Constructor_default()
        {
            dynamic expando = new Expando();
        }

        [Test]
        public void Constructor_instance()
        {
            var instance = new TestClass();
            dynamic expando = new Expando(instance);
        }

        [Test]
        public void Constructor_dictionary()
        {
            var dict = new Dictionary<string, object>();
            dynamic expando = new Expando(dict);

            expando.Hi = "there";

            Assert.AreEqual("there", dict["Hi"]);
        }

        [Test]
        public void Constructor_dictionary_as_object()
        {
            var dict = new Dictionary<string, object>();
            dynamic expando = new Expando((object)dict);

            expando.Hi = "there";

            Assert.AreEqual("there", dict["Hi"]);
        }

        [Test]
        public void PublicProperty_default()
        {
            dynamic expando = new Expando();

            expando.Property = "value";
            Assert.AreEqual("value", expando.Property);
        }

        [Test]
        public void PublicProperty_instance()
        {
            var instance = new TestClass();
            dynamic expando = new Expando(instance);

            expando.Name = "John";
            Assert.AreEqual("John", expando.Name);
            Assert.AreEqual("John", instance.Name);
        }

        [Test]
        public void PublicProperty_derived_expando()
        {
            dynamic derived = new DerivedExpando();
            derived.Age = 23;

            Assert.AreEqual(23, derived.Age);

            var typedDerived = (DerivedExpando)derived;
            Assert.AreEqual(23, typedDerived.Age);
        }

        [Test]
        public void ReadOnlyProperty_instance_getter()
        {
            var instance = new TestClass();
            dynamic expando = new Expando(instance);

            expando.Name = "John";
            Assert.AreEqual(" John", expando.ReadOnlyFullName);
            Assert.AreEqual(" John", instance.ReadOnlyFullName);
        }


        [Test]
        public void ReadOnlyProperty_instance_setter()
        {
            var instance = new TestClass();
            dynamic expando = new Expando(instance);

            Assert.Throws<MemberAccessException>(() => expando.ReadOnlyFullName = "John Doe");
        }

        [Test]
        public void PrivateProperty_instance_getter()
        {
            var instance = new TestClass();
            dynamic expando = new Expando(instance);
            var value = expando.PrivateAge;
            instance.SetPrivateAge(10);
            var objValue = instance.GetPrivateAge();

            Assert.AreEqual(10, objValue);
            Assert.AreEqual(null, value);
        }

        [Test]
        public void PrivateProperty_instance_setter()
        {
            var instance = new TestClass();
            dynamic expando = new Expando(instance);
            var value = expando.PrivateAge;
            expando.PrivateAge = 10;
            var objValue = instance.GetPrivateAge();

            Assert.AreNotEqual(value, objValue);
            Assert.AreEqual(10, expando.PrivateAge);
            Assert.AreEqual(0, instance.GetPrivateAge());
        }

        [Test]
        public void TryInvokeMember_Func_property()
        {
            dynamic instance = new Expando();
            instance.GetName = (Func<int, string>)(age => $"John Doe: {age}");

            var name = instance.GetName(30);
            Assert.AreEqual("John Doe: 30", name);
        }

        [Test]
        public void TryInvokeMember_Func_property_with_instance()
        {
            dynamic instance = new Expando(new TestClass());
            instance.GetName = (Func<int, string>)(age => $"John Doe: {age}");

            var name = instance.GetName(30);
            Assert.AreEqual("John Doe: 30", name);
        }

        [Test]
        [TestCase(12)]
        [TestCase(15.3)]
        [TestCase("23.45")]
        public void Dynamic_Property(object age)
        {
            dynamic expando = new Expando();
            expando.Age = age;

            Assert.AreEqual(age, expando.Age);
        }

        [Test]
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

        [Test]
        public void Dynamic_Property_non_existing()
        {
            dynamic expando = new Expando();
            var age = expando.Age;
            Assert.IsNull(age);
        }

        [Test]
        public void Indexer_Property_non_existing()
        {
            var expando = new Expando();
            var age = expando["Age"];
            Assert.IsNull(age);
        }

        [Test]
        public void IsDefined_Property_existing_in_dictionary()
        {
            var expando = new Expando();
            expando["Age"] = 12;
            Assert.IsTrue(expando.HasMember("Age"));
        }

        [Test]
        public void IsDefined_Property_existing_in_object()
        {
            var expando = new Expando(Substitute.For<IIdentifiable>());
            Assert.IsTrue(expando.HasMember(nameof(IIdentifiable.Id)));
        }

        [Test]
        public void IsDefined_Property_existing_in_dictionary_not_object()
        {
            var expando = new Expando(Substitute.For<IIdentifiable>());
            expando["Age"] = 12;
            Assert.IsTrue(expando.HasMember("Age"));
        }

        [Test]
        public void IsDefined_Property_non_existing()
        {
            var expando = new Expando();
            Assert.IsFalse(expando.HasMember("Age"));
        }

        [Test]
        public void IsDefined_Property_non_existing_in_object()
        {
            var expando = new Expando(Substitute.For<IIdentifiable>());
            Assert.IsFalse(expando.HasMember("Age"));
        }

        [Test]
        [TestCase(12, ExpectedResult = false)]
        [TestCase(80, ExpectedResult = true)]
        public bool TryInvokeMember_Method_with_instance(int age)
        {
            var instance = new DerivedExpando();
            instance.Age = age;
            dynamic expando = new Expando(instance);

            var isOld = expando.IsOld();
            return isOld;
        }

        [Test]
        [TestCase(12, ExpectedResult = false)]
        [TestCase(80, ExpectedResult = true)]
        public bool TryInvokeMember_Method(int age)
        {
            var instance = new DerivedExpando();
            instance.Age = age;
            dynamic expando = instance.ToDynamic();

            var isOld = expando.IsOld();
            return isOld;
        }

        [Test]
        public void ToDictionary_wrapped()
        {
            var instance = new WrapperExpando(new TestClass());
            var dictionary = instance.ToDictionary();
            Assert.AreEqual(3, dictionary.Count);
            Assert.IsTrue((bool)dictionary[nameof(WrapperExpando.HasWrappedObject)]);
        }

        [Test]
        public void ToDictionary_non_wrapped()
        {
            var instance = new WrapperExpando();
            var dictionary = instance.ToDictionary();
            Assert.AreEqual(1, dictionary.Count);
            Assert.IsFalse((bool)dictionary[nameof(WrapperExpando.HasWrappedObject)]);
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

        public class DerivedExpando : Expando
        {
            public int Age { get; set; }

            public bool IsOld()
            {
                return this.Age >= 70;
            }
        }

        public class WrapperExpando : Expando
        {
            public WrapperExpando()
            {
            }

            public WrapperExpando(object innerObject)
                : base(innerObject)
            {
            }

            public bool HasWrappedObject => this.GetInnerObjectTypeInfo() != null;
        }
    }
}