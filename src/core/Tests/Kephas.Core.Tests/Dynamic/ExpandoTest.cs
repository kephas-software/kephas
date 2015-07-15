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

        public class TestClass
        {
            public string Name { get; set; }

            public string ReadOnlyFullName
            {
                get
                {
                    return this.ComputeFullName(string.Empty);
                }
            }

            private int PrivateAge { get; set; }

            public virtual string ComputeFullName(string parentsInitials)
            {
                return parentsInitials + " " + this.Name;
            }
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
        }
    }
}