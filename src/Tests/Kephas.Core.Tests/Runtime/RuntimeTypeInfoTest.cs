// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimeTypeInfoTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Test class for <see cref="RuntimeTypeInfo" />
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests.Runtime
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    using Kephas.Activation;
    using Kephas.Dynamic;
    using Kephas.Reflection;
    using Kephas.Runtime;

    using NUnit.Framework;

    /// <summary>
    /// Test class for <see cref="RuntimeTypeInfo"/>
    /// </summary>
    [TestFixture]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public class RuntimeTypeInfoTest
    {
        [Test]
        public void RuntimeTypeInfo_constructor_test()
        {
            var dynamicTypeInfo = new RuntimeTypeInfo(typeof(TestClass));
            var type = dynamicTypeInfo.Type;
            Assert.AreEqual(type, typeof(TestClass));
        }

        [Test]
        public void Name_test()
        {
            var dynamicTypeInfo = new RuntimeTypeInfo(typeof(TestClass));
            Assert.AreEqual("TestClass", dynamicTypeInfo.Name);
        }

        [Test]
        public void FullName_test()
        {
            var dynamicTypeInfo = new RuntimeTypeInfo(typeof(TestClass));
            Assert.AreEqual("Kephas.Core.Tests.Runtime.RuntimeTypeInfoTest+TestClass", dynamicTypeInfo.FullName);
        }

        [Test]
        public void QualifiedFullName_test()
        {
            var dynamicTypeInfo = new RuntimeTypeInfo(typeof(TestClass));
            Assert.AreEqual("Kephas.Core.Tests.Runtime.RuntimeTypeInfoTest+TestClass, Kephas.Core.Tests", dynamicTypeInfo.QualifiedFullName);
        }

        [Test]
        public void GetValue_instance_null_throws()
        {
            var dynamicTypeInfo = new RuntimeTypeInfo(typeof(TestClass));
            Assert.That(() => dynamicTypeInfo.GetValue(null, string.Empty), Throws.InstanceOf<Exception>());
        }

        [Test]
        public void GetValue_instance_not_null()
        {
            var dynamicTypeInfo = new RuntimeTypeInfo(typeof(TestClass));
            var instance = new TestClass { Name = "noName" };
            var result = dynamicTypeInfo.GetValue(instance, "Name");
            Assert.AreEqual(instance.Name, result);
        }

        [Test]
        public void TryGetValue_instance_null_returns_undefined()
        {
            var dynamicTypeInfo = new RuntimeTypeInfo(typeof(TestClass));
            Assert.Throws<ArgumentNullException>(() => dynamicTypeInfo.TryGetValue(null, string.Empty, out var result));
        }

        [Test]
        public void TryGetValue_instance_not_null_valid_property()
        {
            var dynamicTypeInfo = new RuntimeTypeInfo(typeof(TestClass));
            var instance = new TestClass { Name = "NoName" };
            var success = dynamicTypeInfo.TryGetValue(instance, "Name", out var result);
            Assert.AreEqual(instance.Name, result);
            Assert.IsTrue(success);
        }

        [Test]
        public void TryGetValue_instance_not_null_invalid_property()
        {
            var dynamicTypeInfo = new RuntimeTypeInfo(typeof(TestClass));
            var instance = new TestClass { Name = "NoName" };
            var success = dynamicTypeInfo.TryGetValue(instance, "nothing", out var result);
            Assert.IsNull(result);
            Assert.IsFalse(success);
        }

        [Test]
        public void SetValue_instance_null_throws()
        {
            var dynamicTypeInfo = new RuntimeTypeInfo(typeof(TestClass));
            Assert.That(() => dynamicTypeInfo.SetValue(null, string.Empty, null), Throws.InstanceOf<Exception>());
        }

        [Test]
        public void SetValue_valid_instance()
        {
            var dynamicTypeInfo = new RuntimeTypeInfo(typeof(TestClass));
            var instance = new TestClass();
            const string value = "someName";
            dynamicTypeInfo.SetValue(instance, "Name", value);
            Assert.AreEqual(value, instance.Name);
        }

        [Test]
        public void TrySetValue_instance_null_returns_false()
        {
            var dynamicTypeInfo = new RuntimeTypeInfo(typeof(TestClass));
            var result = dynamicTypeInfo.TrySetValue(null, string.Empty, null);
            Assert.AreEqual(false, result);
        }

        [Test]
        public void Invoke_valid_instance()
        {
            var dynamicTypeInfo = new RuntimeTypeInfo(typeof(TestClass));
            var instance = new TestClass { Name = "someName" };
            var list = new List<string> { "IC" };
            var ienum = (IEnumerable<object>)list;
            var result = dynamicTypeInfo.Invoke(instance, "ComputeFullName", ienum);
            Assert.AreEqual(instance.ComputeFullName("IC"), result);
        }

        [Test]
        public void Invoke_instance_null_throws()
        {
            var dynamicTypeInfo = new RuntimeTypeInfo(typeof(TestClass));
            var list = new List<string>();
            var ienum = (IEnumerable<object>)list;
            Assert.That(() => dynamicTypeInfo.Invoke(null, string.Empty, ienum), Throws.InstanceOf<Exception>());
        }

        [Test]
        public void TryInvoke_instance_null_returns_undefined()
        {
            var dynamicTypeInfo = new RuntimeTypeInfo(typeof(TestClass));
            var list = new List<string>();
            var ienum = (IEnumerable<object>)list;
            Assert.Throws<ArgumentNullException>(() => dynamicTypeInfo.TryInvoke(null, string.Empty, ienum, out var result));
        }

        [Test]
        public void TryInvoke_instance_non_existing_method_returns_undefined()
        {
            var dynamicTypeInfo = new RuntimeTypeInfo(typeof(TestClass));
            var list = new List<string>();
            object instance = new TestClass();
            var ienum = (IEnumerable<object>)list;
            var success = dynamicTypeInfo.TryInvoke(instance, "blah-blah", ienum, out var result);
            Assert.IsNull(result);
            Assert.IsFalse(success);
        }

        [Test]
        public void CreateInstance_exception_interface()
        {
            var dynamicTypeInfo = new RuntimeTypeInfo(typeof(IActivator));
            Assert.Throws<InvalidOperationException>(() => dynamicTypeInfo.CreateInstance());
        }

        [Test]
        public void CreateInstance_exception_private_constructor()
        {
            var dynamicTypeInfo = new RuntimeTypeInfo(typeof(TestClassWithConstructor));
            Assert.Throws<MissingMethodException>(() => dynamicTypeInfo.CreateInstance(new object[] {3, "Hello"}));
        }

        [Test]
        public void CreateInstance_no_args()
        {
            var dynamicTypeInfo = new RuntimeTypeInfo(typeof(TestClass));
            var instance = dynamicTypeInfo.CreateInstance();
            Assert.IsInstanceOf<TestClass>(instance);
        }

        [Test]
        public void CreateInstance_args()
        {
            var dynamicTypeInfo = new RuntimeTypeInfo(typeof(TestClassWithConstructor));
            var instance = dynamicTypeInfo.CreateInstance(3);
            Assert.IsInstanceOf<TestClassWithConstructor>(instance);
        }

        [Test]
        public void GetValue_throwOnNotFound_DynamicProperty()
        {
            var dynamicTypeInfo = new RuntimeTypeInfo(typeof(TestClass));
            object instance = new TestClass();
            Assert.Throws<MemberAccessException>(() => dynamicTypeInfo.GetValue(instance, string.Empty));
        }

        [Test]
        public void Bases_SystemObject()
        {
            var dynamicTypeInfo = new RuntimeTypeInfo(typeof(object));

            Assert.AreEqual(0, dynamicTypeInfo.BaseTypes.Count());
        }

        [Test]
        public void Bases_class_without_base()
        {
            var dynamicTypeInfo = new RuntimeTypeInfo(typeof(TestClass));

            Assert.AreEqual(1, dynamicTypeInfo.BaseTypes.Count());
            Assert.AreEqual("System.Object", dynamicTypeInfo.BaseTypes.First().FullName);
        }

        [Test]
        public void Bases_class_with_base()
        {
            var dynamicTypeInfo = new RuntimeTypeInfo(typeof(TestDerivedClass));

            Assert.AreEqual(1, dynamicTypeInfo.BaseTypes.Count());
            Assert.AreSame(typeof(TestClass).AsRuntimeTypeInfo(), dynamicTypeInfo.BaseTypes.First());
        }

        [Test]
        public void Bases_class_with_base_and_interfaces()
        {
            var dynamicTypeInfo = new RuntimeTypeInfo(typeof(TestDerivedClassWithInterfaces));

            var bases = dynamicTypeInfo.BaseTypes.ToList();
            Assert.AreEqual(3, bases.Count);
            Assert.AreSame(typeof(TestClass).AsRuntimeTypeInfo(), bases[0]);
            Assert.AreSame(typeof(IEnumerable<int>).AsRuntimeTypeInfo(), bases[1]);
            Assert.AreSame(typeof(IEnumerable).AsRuntimeTypeInfo(), bases[2]);
        }

        [Test]
        public void GenericTypeDefinition_non_generic()
        {
            var dynamicTypeInfo = new RuntimeTypeInfo(typeof(TestClass));

            Assert.AreSame(ReflectionHelper.EmptyTypeInfos, dynamicTypeInfo.GenericTypeParameters);
            Assert.AreSame(ReflectionHelper.EmptyTypeInfos, dynamicTypeInfo.GenericTypeArguments);
            Assert.IsNull(dynamicTypeInfo.GenericTypeDefinition);
        }

        [Test]
        public void GenericTypeDefinition_open_generic()
        {
            var dynamicTypeInfo = new RuntimeTypeInfo(typeof(IEnumerable<>));

            Assert.AreEqual(1, dynamicTypeInfo.GenericTypeParameters.Count);
            Assert.AreEqual(0, dynamicTypeInfo.GenericTypeArguments.Count);
            Assert.AreEqual("T", dynamicTypeInfo.GenericTypeParameters[0].Name);
            Assert.IsNull(dynamicTypeInfo.GenericTypeDefinition);
        }

        [Test]
        public void GenericTypeDefinition_closed_generic()
        {
            var dynamicTypeInfoDef = new RuntimeTypeInfo(typeof(IEnumerable<>));
            var dynamicTypeInfo = new RuntimeTypeInfo(typeof(IEnumerable<string>));

            Assert.AreEqual(0, dynamicTypeInfo.GenericTypeParameters.Count);
            Assert.AreEqual(1, dynamicTypeInfo.GenericTypeArguments.Count);
            Assert.AreEqual("String", dynamicTypeInfo.GenericTypeArguments[0].Name);
            Assert.IsNotNull(dynamicTypeInfo.GenericTypeDefinition);
            Assert.AreEqual("IEnumerable`1", dynamicTypeInfo.GenericTypeDefinition.Name);
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

        public class TestDerivedClass : TestClass
        {
        }


        public class TestDerivedClassWithInterfaces : TestClass, IEnumerable<int>
        {
            public IEnumerator<int> GetEnumerator()
            {
                throw new NotImplementedException();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }
        }
    }
}
