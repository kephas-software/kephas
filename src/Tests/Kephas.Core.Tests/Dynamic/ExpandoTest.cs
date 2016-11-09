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

            Assert.Throws<MemberAccessException>(() => { var value = expando.PrivateAge; });
        }

        [Test]
        public void PrivateProperty_instance_setter()
        {
            var instance = new TestClass();
            dynamic expando = new Expando(instance);

            Assert.Throws<MemberAccessException>(() => expando.PrivateAge = "John Doe");
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
        public async Task Dynamic_Property_non_concurrent()
        {
            dynamic expando = new Expando();
            Exception exception = null;
            Action accessor = () =>
            {
                try
                {
                    for (var i = 0; i <= 2000; i++)
                    {
                        expando["Prop" + Thread.CurrentThread.Name + i] = Thread.CurrentThread.Name + i;
                    }
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
            };

            var tasks = new List<Task>();
            for (var j = 0; j < 200; j++)
            {
                tasks.Add(Task.Factory.StartNew(accessor));
            }

            // normally it should crash
            await Task.WhenAll(tasks);

            if (exception == null)
            {
                Assert.Inconclusive("If the inner dictionary did not crash it doesn't mean it is thread safe.");
            }
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