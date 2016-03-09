// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DynamicTypeInfoTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Test class for <see cref="DynamicTypeInfo" />
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests.Dynamic
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    using Kephas.Activation;
    using Kephas.Dynamic;

    using NUnit.Framework;

    /// <summary>
    /// Test class for <see cref="DynamicTypeInfo"/>
    /// </summary>
    [TestFixture]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public class DynamicTypeInfoTest
    {
        [Test]
        public void DynamicTypeInfo_constructor_test()
        {
            var dynamicTypeInfo = new DynamicTypeInfo(typeof(TestClass));
            var type = dynamicTypeInfo.Type;
            Assert.AreEqual(type, typeof(TestClass));
        }

        [Test]
        public void Name_test()
        {
            var dynamicTypeInfo = new DynamicTypeInfo(typeof(TestClass));
            Assert.AreEqual("TestClass", dynamicTypeInfo.Name);
        }

        [Test]
        public void GetValue_instance_null_throws()
        {
            var dynamicTypeInfo = new DynamicTypeInfo(typeof(TestClass));
            Assert.That(() => dynamicTypeInfo.GetValue(null, string.Empty), Throws.InstanceOf<Exception>());
        }

        [Test]
        public void GetValue_instance_not_null()
        {
            var dynamicTypeInfo = new DynamicTypeInfo(typeof(TestClass));
            var instance = new TestClass { Name = "noName" };
            var result = dynamicTypeInfo.GetValue(instance, "Name");
            Assert.AreEqual(instance.Name, result);
        }

        [Test]
        public void TryGetValue_instance_null_returns_undefined()
        {
            var dynamicTypeInfo = new DynamicTypeInfo(typeof(TestClass));
            var result = dynamicTypeInfo.TryGetValue(null, string.Empty);
            Assert.AreEqual(Undefined.Value, result);
        }

        [Test]
        public void TryGetValue_instance_not_null_valid_property()
        {
            var dynamicTypeInfo = new DynamicTypeInfo(typeof(TestClass));
            var instance = new TestClass { Name = "NoName" };
            var result = dynamicTypeInfo.TryGetValue(instance, "Name");
            Assert.AreEqual(instance.Name, result);
        }

        [Test]
        public void TryGetValue_instance_not_null_invalid_property()
        {
            var dynamicTypeInfo = new DynamicTypeInfo(typeof(TestClass));
            var instance = new TestClass { Name = "NoName" };
            var result = dynamicTypeInfo.TryGetValue(instance, "nothing");
            Assert.AreEqual(Undefined.Value, result);
        }

        [Test]
        public void SetValue_instance_null_throws()
        {
            var dynamicTypeInfo = new DynamicTypeInfo(typeof(TestClass));
            Assert.That(() => dynamicTypeInfo.SetValue(null, string.Empty, null), Throws.InstanceOf<Exception>());
        }

        [Test]
        public void SetValue_valid_instance()
        {
            var dynamicTypeInfo = new DynamicTypeInfo(typeof(TestClass));
            var instance = new TestClass();
            const string value = "someName";
            dynamicTypeInfo.SetValue(instance, "Name", value);
            Assert.AreEqual(value, instance.Name);
        }

        [Test]
        public void TrySetValue_instance_null_returns_false()
        {
            var dynamicTypeInfo = new DynamicTypeInfo(typeof(TestClass));
            var result = dynamicTypeInfo.TrySetValue(null, string.Empty, null);
            Assert.AreEqual(false, result);
        }

        [Test]
        public void Invoke_valid_instance()
        {
            var dynamicTypeInfo = new DynamicTypeInfo(typeof(TestClass));
            var instance = new TestClass { Name = "someName" };
            var list = new List<string> { "IC" };
            var ienum = (IEnumerable<object>)list;
            var result = dynamicTypeInfo.Invoke(instance, "ComputeFullName", ienum);
            Assert.AreEqual(instance.ComputeFullName("IC"), result);
        }

        [Test]
        public void Invoke_instance_null_throws()
        {
            var dynamicTypeInfo = new DynamicTypeInfo(typeof(TestClass));
            var list = new List<string>();
            var ienum = (IEnumerable<object>)list;
            Assert.That(() => dynamicTypeInfo.Invoke(null, string.Empty, ienum), Throws.InstanceOf<Exception>());
        }

        [Test]
        public void TryInvoke_instance_null_returns_undefined()
        {
            var dynamicTypeInfo = new DynamicTypeInfo(typeof(TestClass));
            var list = new List<string>();
            var ienum = (IEnumerable<object>)list;
            var result = dynamicTypeInfo.TryInvoke(null, string.Empty, ienum);
            Assert.AreEqual(Undefined.Value, result);
        }

        [Test]
        public void TryInvoke_instance_non_existing_method_returns_undefined()
        {
            var dynamicTypeInfo = new DynamicTypeInfo(typeof(TestClass));
            var list = new List<string>();
            object instance = new TestClass();
            var ienum = (IEnumerable<object>)list;
            var result = dynamicTypeInfo.TryInvoke(instance, "blah-blah", ienum);
            Assert.AreEqual(Undefined.Value, result);
        }

        [Test]
        public void CreateInstance_exception_interface()
        {
            var dynamicTypeInfo = new DynamicTypeInfo(typeof(IActivator));
            Assert.Throws<MissingMethodException>(() => dynamicTypeInfo.CreateInstance());
        }

        [Test]
        public void CreateInstance_exception_private_constructor()
        {
            var dynamicTypeInfo = new DynamicTypeInfo(typeof(TestClassWithConstructor));
            Assert.Throws<MissingMethodException>(() => dynamicTypeInfo.CreateInstance(new object[] {3, "Hello"}));
        }

        [Test]
        public void CreateInstance_no_args()
        {
            var dynamicTypeInfo = new DynamicTypeInfo(typeof(TestClass));
            var instance = dynamicTypeInfo.CreateInstance();
            Assert.IsInstanceOf<TestClass>(instance);
        }

        [Test]
        public void CreateInstance_args()
        {
            var dynamicTypeInfo = new DynamicTypeInfo(typeof(TestClassWithConstructor));
            var instance = dynamicTypeInfo.CreateInstance(3);
            Assert.IsInstanceOf<TestClass>(instance);
        }

        [Test]
        public void GetDynamicProperty_throwOnNotFound()
        {
            var dynamicTypeInfo = new DynamicTypeInfo(typeof(TestClass));
            object instance = new TestClass();
            Assert.Throws<MemberAccessException>(() => dynamicTypeInfo.GetValue(instance, string.Empty));
        }

        public class TestClass
        {
            public string Name { get; set; }

            public string ReadOnlyFullName => this.ComputeFullName(string.Empty);

            private int PrivateAge { get; set; }

            public virtual string ComputeFullName(string parentsInitials)
            {
                return parentsInitials + " " + this.Name;
            }

            public static explicit operator Func<object, object, object>(TestClass v)
            {
                throw new NotImplementedException();
            }
        }

        public class TestClassWithConstructor
        {
            public TestClassWithConstructor(int age)
            {
            }

            private TestClassWithConstructor(int age, string name)
            {
            }
        }
    }
}
