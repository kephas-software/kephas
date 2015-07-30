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

    using Kephas.Dynamic;
    using Kephas.Extensions;

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
        [ExpectedException(typeof(MemberAccessException))]
        public void ReadOnlyProperty_instance_setter()
        {
            var instance = new TestClass();
            dynamic expando = new Expando(instance);

            expando.ReadOnlyFullName = "John Doe";
        }

        [Test]
        [ExpectedException(typeof(MemberAccessException))]
        public void PrivateProperty_instance_getter()
        {
            var instance = new TestClass();
            dynamic expando = new Expando(instance);

            var age = expando.PrivateAge;
        }

        [Test]
        [ExpectedException(typeof(MemberAccessException))]
        public void PrivateProperty_instance_setter()
        {
            var instance = new TestClass();
            dynamic expando = new Expando(instance);

            expando.PrivateAge = "John Doe";
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

        public class TestClass
        {
            public string Name { get; set; }

            public string ReadOnlyFullName => this.ComputeFullName(string.Empty);

            private int PrivateAge { get; set; }

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
    }
}