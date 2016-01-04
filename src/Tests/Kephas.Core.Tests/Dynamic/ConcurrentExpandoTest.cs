// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConcurrentExpandoTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Test class for <see cref="ConcurrentExpando" />
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests.Dynamic
{
    using System;

    using Kephas.Dynamic;

    using NUnit.Framework;

    /// <summary>
    /// Test class for <see cref="ConcurrentExpando"/>
    /// </summary>
    [TestFixture]
    public class ConcurrentExpandoTest
    {
        [Test]
        public void Constructor_default()
        {
            dynamic expando = new ConcurrentExpando();
        }

        [Test]
        public void Constructor_instance()
        {
            var instance = new TestClass();
            dynamic expando = new ConcurrentExpando(instance);
        }

        [Test]
        public void PublicProperty_default()
        {
            dynamic expando = new ConcurrentExpando();

            expando.Property = "value";
            Assert.AreEqual("value", expando.Property);
        }

        [Test]
        public void PublicProperty_instance()
        {
            var instance = new TestClass();
            dynamic expando = new ConcurrentExpando(instance);

            expando.Name = "John";
            Assert.AreEqual("John", expando.Name);
            Assert.AreEqual("John", instance.Name);
        }

        [Test]
        public void PublicProperty_derived_expando()
        {
            dynamic derived = new DerivedConcurrentExpando();
            derived.Age = 23;

            Assert.AreEqual(23, derived.Age);

            var typedDerived = (DerivedConcurrentExpando)derived;
            Assert.AreEqual(23, typedDerived.Age);
        }

        [Test]
        public void ReadOnlyProperty_instance_getter()
        {
            var instance = new TestClass();
            dynamic expando = new ConcurrentExpando(instance);

            expando.Name = "John";
            Assert.AreEqual(" John", expando.ReadOnlyFullName);
            Assert.AreEqual(" John", instance.ReadOnlyFullName);
        }


        [Test]
        public void ReadOnlyProperty_instance_setter()
        {
            var instance = new TestClass();
            dynamic expando = new ConcurrentExpando(instance);

            Assert.Throws<MemberAccessException>(() => expando.ReadOnlyFullName = "John Doe");
        }

        [Test]
        public void PrivateProperty_instance_getter()
        {
            var instance = new TestClass();
            dynamic expando = new ConcurrentExpando(instance);

            Assert.Throws<MemberAccessException>(() => { var value = expando.PrivateAge; });
        }

        [Test]
        public void PrivateProperty_instance_setter()
        {
            var instance = new TestClass();
            dynamic expando = new ConcurrentExpando(instance);

            Assert.Throws<MemberAccessException>(() => expando.PrivateAge = "John Doe");
        }

        [Test]
        public void TryInvokeMember_Func_property()
        {
            dynamic instance = new ConcurrentExpando();
            instance.GetName = (Func<int, string>)(age => $"John Doe: {age}");

            var name = instance.GetName(30);
            Assert.AreEqual("John Doe: 30", name);
        }

        [Test]
        public void TryInvokeMember_Func_property_with_instance()
        {
            dynamic instance = new ConcurrentExpando(new TestClass());
            instance.GetName = (Func<int, string>)(age => $"John Doe: {age}");

            var name = instance.GetName(30);
            Assert.AreEqual("John Doe: 30", name);
        }

        [Test]
        [TestCase(12, ExpectedResult = false)]
        [TestCase(80, ExpectedResult = true)]
        public bool TryInvokeMember_Method_with_instance(int age)
        {
            var instance = new DerivedConcurrentExpando();
            instance.Age = age;
            dynamic expando = new ConcurrentExpando(instance);

            var isOld = expando.IsOld();
            return isOld;
        }

        [Test]
        [TestCase(12, ExpectedResult = false)]
        [TestCase(80, ExpectedResult = true)]
        public bool TryInvokeMember_Method(int age)
        {
            var instance = new DerivedConcurrentExpando();
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

        public class DerivedConcurrentExpando : ConcurrentExpando
        {
            public int Age { get; set; }

            public bool IsOld()
            {
                return this.Age >= 70;
            }
        }
    }
}