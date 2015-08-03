// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimeDynamicTypeTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Test class for <see cref="RuntimeDynamicType" />
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests.Dynamic
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    using Kephas.Dynamic;

    using NUnit.Framework;

    /// <summary>
    /// Test class for <see cref="RuntimeDynamicType"/>
    /// </summary>
    [TestFixture]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public class RuntimeDynamicTypeTest
    {
        [Test]
        public void RuntimeDynamicType_constructor_test()
        {
            var runtimeDynamicType = new RuntimeDynamicType(typeof(TestClass));
            var type = runtimeDynamicType.Type;
            Assert.AreEqual(type, typeof(TestClass));
        }

        [Test]
        public void GetValue_test_instance_null()
        {
            var runtimeDynamicType = new RuntimeDynamicType(typeof(TestClass));
            var result = runtimeDynamicType.GetValue(null, string.Empty);
            Assert.AreEqual(Undefined.Value, result);
        }

        [Test]
        public void GetValue_test_instance_not_null()
        {
            var runtimeDynamicType = new RuntimeDynamicType(typeof(TestClass));
            var instance = new TestClass {Name = "noName"};
            var result = runtimeDynamicType.GetValue(instance, "Name");
            Assert.AreEqual(instance.Name, result);
        }

        [Test]
        public void TryGetValue_test_instance_null()
        {
            var runtimeDynamicType = new RuntimeDynamicType(typeof(TestClass));
            var result = runtimeDynamicType.TryGetValue(null, string.Empty);
            Assert.AreEqual(Undefined.Value, result);
        }
        
        [Test]
        public void TryGetValue_test_instance_not_null_valid_property()
        {
            var runtimeDynamicType = new RuntimeDynamicType(typeof(TestClass));
            var instance = new TestClass { Name = "NoName" };
            var result = runtimeDynamicType.TryGetValue(instance, "Name");
            Assert.AreEqual(instance.Name, result);
        }

        [Test]
        public void TryGetValue_test_instance_not_null_invalid_property()
        {
            var runtimeDynamicType = new RuntimeDynamicType(typeof(TestClass));
            var instance = new TestClass { Name = "NoName" };
            var result = runtimeDynamicType.TryGetValue(instance, "nothing");
            Assert.AreEqual(Undefined.Value, result);
        }

        [Test]
        public void SetValue_test_instance_null()
        {
            var runtimeDynamicType = new RuntimeDynamicType(typeof(TestClass));
            runtimeDynamicType.SetValue(null, string.Empty, null);
        }

        [Test]
        public void SetValue_test_valid_instance()
        {
            var runtimeDynamicType = new RuntimeDynamicType(typeof(TestClass));
            var instance = new TestClass();
            const string value = "someName";
            runtimeDynamicType.SetValue(instance, "Name", value);
            Assert.AreEqual(value, instance.Name);
        }

        [Test]
        public void TrySetValue_test_instance_null()
        {
            var runtimeDynamicType = new RuntimeDynamicType(typeof(TestClass));
            var result = runtimeDynamicType.TrySetValue(null, string.Empty, null);
            Assert.AreEqual(false, result);
        }
        
        [Test]
        public void Invoke_test_valid_instance()
        {
            var runtimeDynamicType = new RuntimeDynamicType(typeof(TestClass));
            var instance = new TestClass {Name = "someName"};
            var list = new List<string> {"IC"};
            var ienum = (IEnumerable<object>) list;
            var result = runtimeDynamicType.Invoke(instance, "ComputeFullName", ienum);
            Assert.AreEqual(instance.ComputeFullName("IC"), result);
        }

        [Test]
        public void Invoke_test_instance_null()
        {
            var runtimeDynamicType = new RuntimeDynamicType(typeof(TestClass));
            var list = new List<string>();
            var ienum = (IEnumerable<object>)list;
            var result = runtimeDynamicType.Invoke(null, string.Empty, ienum);
            Assert.AreEqual(null, result);
        }

        [Test]
        public void TryInvoke_test_instance_null()
        {
            var runtimeDynamicType = new RuntimeDynamicType(typeof(TestClass));
            var list = new List<string>();
            var ienum = (IEnumerable<object>)list;
            var result = runtimeDynamicType.TryInvoke(null, string.Empty, ienum);
            Assert.AreEqual(Undefined.Value, result);
        }

        [Test]
        public void TryInvoke_test_instance()
        {
            var runtimeDynamicType = new RuntimeDynamicType(typeof(TestClass));
            var list = new List<string> ();
            object instance = new TestClass();
            var ienum = (IEnumerable<object>)list;
            var result = runtimeDynamicType.TryInvoke(instance, string.Empty, ienum);
            Assert.AreEqual(Undefined.Value, result);
        }

        [Test]
        [ExpectedException(typeof(MemberAccessException))]
        public void GetDynamicProperty_test_throwOnNotFound()
        {
            var runtimeDynamicType = new RuntimeDynamicType(typeof(TestClass));
            object instance = new TestClass();
            runtimeDynamicType.GetValue(instance, string.Empty);
        }
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
}
