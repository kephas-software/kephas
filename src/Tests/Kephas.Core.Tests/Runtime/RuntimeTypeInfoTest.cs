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
    using System.Reflection;

    using Kephas.Activation;
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
            var runtimeTypeInfo = new RuntimeTypeInfo(typeof(TestClass));
            var type = runtimeTypeInfo.Type;
            Assert.AreEqual(type, typeof(TestClass));
        }

        [Test]
        public void Name()
        {
            var runtimeTypeInfo = new RuntimeTypeInfo(typeof(TestClass));
            Assert.AreEqual("TestClass", runtimeTypeInfo.Name);
        }

        [Test]
        public void FullName()
        {
            var runtimeTypeInfo = new RuntimeTypeInfo(typeof(TestClass));
            Assert.AreEqual("Kephas.Core.Tests.Runtime.RuntimeTypeInfoTest+TestClass", runtimeTypeInfo.FullName);
        }

        [Test]
        public void QualifiedFullName()
        {
            var runtimeTypeInfo = new RuntimeTypeInfo(typeof(TestClass));
            Assert.AreEqual("Kephas.Core.Tests.Runtime.RuntimeTypeInfoTest+TestClass, Kephas.Core.Tests", runtimeTypeInfo.QualifiedFullName);
        }

        [Test]
        public void GetValue_instance_null_throws()
        {
            var runtimeTypeInfo = new RuntimeTypeInfo(typeof(TestClass));
            Assert.That(() => runtimeTypeInfo.GetValue(null, string.Empty), Throws.InstanceOf<Exception>());
        }

        [Test]
        public void GetValue_instance_not_null()
        {
            var runtimeTypeInfo = new RuntimeTypeInfo(typeof(TestClass));
            var instance = new TestClass { Name = "noName" };
            var result = runtimeTypeInfo.GetValue(instance, "Name");
            Assert.AreEqual(instance.Name, result);
        }

        [Test]
        public void TryGetValue_instance_null_returns_undefined()
        {
            var runtimeTypeInfo = new RuntimeTypeInfo(typeof(TestClass));
            Assert.Throws<ArgumentNullException>(() => runtimeTypeInfo.TryGetValue(null, string.Empty, out var result));
        }

        [Test]
        public void TryGetValue_instance_not_null_valid_property()
        {
            var runtimeTypeInfo = new RuntimeTypeInfo(typeof(TestClass));
            var instance = new TestClass { Name = "NoName" };
            var success = runtimeTypeInfo.TryGetValue(instance, "Name", out var result);
            Assert.AreEqual(instance.Name, result);
            Assert.IsTrue(success);
        }

        [Test]
        public void TryGetValue_instance_not_null_invalid_property()
        {
            var runtimeTypeInfo = new RuntimeTypeInfo(typeof(TestClass));
            var instance = new TestClass { Name = "NoName" };
            var success = runtimeTypeInfo.TryGetValue(instance, "nothing", out var result);
            Assert.IsNull(result);
            Assert.IsFalse(success);
        }

        [Test]
        public void SetValue_instance_null_throws()
        {
            var runtimeTypeInfo = new RuntimeTypeInfo(typeof(TestClass));
            Assert.That(() => runtimeTypeInfo.SetValue(null, string.Empty, null), Throws.InstanceOf<Exception>());
        }

        [Test]
        public void SetValue_valid_instance()
        {
            var runtimeTypeInfo = new RuntimeTypeInfo(typeof(TestClass));
            var instance = new TestClass();
            const string value = "someName";
            runtimeTypeInfo.SetValue(instance, "Name", value);
            Assert.AreEqual(value, instance.Name);
        }

        [Test]
        public void TrySetValue_instance_null_returns_false()
        {
            var runtimeTypeInfo = new RuntimeTypeInfo(typeof(TestClass));
            var result = runtimeTypeInfo.TrySetValue(null, string.Empty, null);
            Assert.AreEqual(false, result);
        }

        [Test]
        public void Invoke_valid_instance()
        {
            var runtimeTypeInfo = new RuntimeTypeInfo(typeof(TestClass));
            var instance = new TestClass { Name = "someName" };
            var list = new List<string> { "IC" };
            var ienum = (IEnumerable<object>)list;
            var result = runtimeTypeInfo.Invoke(instance, "ComputeFullName", ienum);
            Assert.AreEqual(instance.ComputeFullName("IC"), result);
        }

        [Test]
        public void Invoke_instance_null_throws()
        {
            var runtimeTypeInfo = new RuntimeTypeInfo(typeof(TestClass));
            var list = new List<string>();
            var ienum = (IEnumerable<object>)list;
            Assert.That(() => runtimeTypeInfo.Invoke(null, string.Empty, ienum), Throws.InstanceOf<Exception>());
        }

        [Test]
        public void TryInvoke_instance_null_returns_undefined()
        {
            var runtimeTypeInfo = new RuntimeTypeInfo(typeof(TestClass));
            var list = new List<string>();
            var ienum = (IEnumerable<object>)list;
            Assert.Throws<ArgumentNullException>(() => runtimeTypeInfo.TryInvoke(null, string.Empty, ienum, out var result));
        }

        [Test]
        public void TryInvoke_instance_non_existing_method_returns_undefined()
        {
            var runtimeTypeInfo = new RuntimeTypeInfo(typeof(TestClass));
            var list = new List<string>();
            object instance = new TestClass();
            var ienum = (IEnumerable<object>)list;
            var success = runtimeTypeInfo.TryInvoke(instance, "blah-blah", ienum, out var result);
            Assert.IsNull(result);
            Assert.IsFalse(success);
        }

        [Test]
        public void CreateInstance_exception_interface()
        {
            var runtimeTypeInfo = new RuntimeTypeInfo(typeof(IActivator));
            Assert.Throws<InvalidOperationException>(() => runtimeTypeInfo.CreateInstance());
        }

        [Test]
        public void CreateInstance_exception_private_constructor()
        {
            var runtimeTypeInfo = new RuntimeTypeInfo(typeof(TestClassWithConstructor));
            Assert.Throws<MissingMethodException>(() => runtimeTypeInfo.CreateInstance(new object[] {3, "Hello"}));
        }

        [Test]
        public void CreateInstance_no_args()
        {
            var runtimeTypeInfo = new RuntimeTypeInfo(typeof(TestClass));
            var instance = runtimeTypeInfo.CreateInstance();
            Assert.IsInstanceOf<TestClass>(instance);
        }

        [Test]
        public void CreateInstance_args()
        {
            var runtimeTypeInfo = new RuntimeTypeInfo(typeof(TestClassWithConstructor));
            var instance = runtimeTypeInfo.CreateInstance(3);
            Assert.IsInstanceOf<TestClassWithConstructor>(instance);
        }

        [Test]
        public void GetValue_throwOnNotFound_DynamicProperty()
        {
            var runtimeTypeInfo = new RuntimeTypeInfo(typeof(TestClass));
            object instance = new TestClass();
            Assert.Throws<MemberAccessException>(() => runtimeTypeInfo.GetValue(instance, string.Empty));
        }

        [Test]
        public void Bases_SystemObject()
        {
            var runtimeTypeInfo = new RuntimeTypeInfo(typeof(object));

            Assert.AreEqual(0, runtimeTypeInfo.BaseTypes.Count());
        }

        [Test]
        public void Bases_class_without_base()
        {
            var runtimeTypeInfo = new RuntimeTypeInfo(typeof(TestClass));

            Assert.AreEqual(1, runtimeTypeInfo.BaseTypes.Count());
            Assert.AreEqual("System.Object", runtimeTypeInfo.BaseTypes.First().FullName);
        }

        [Test]
        public void Bases_class_with_base()
        {
            var runtimeTypeInfo = new RuntimeTypeInfo(typeof(TestDerivedClass));

            Assert.AreEqual(1, runtimeTypeInfo.BaseTypes.Count());
            Assert.AreSame(typeof(TestClass).AsRuntimeTypeInfo(), runtimeTypeInfo.BaseTypes.First());
        }

        [Test]
        public void Bases_class_with_base_and_interfaces()
        {
            var runtimeTypeInfo = new RuntimeTypeInfo(typeof(TestDerivedClassWithInterfaces));

            var bases = runtimeTypeInfo.BaseTypes.ToList();
            Assert.AreEqual(3, bases.Count);
            Assert.AreSame(typeof(TestClass).AsRuntimeTypeInfo(), bases[0]);
            Assert.AreSame(typeof(IEnumerable<int>).AsRuntimeTypeInfo(), bases[1]);
            Assert.AreSame(typeof(IEnumerable).AsRuntimeTypeInfo(), bases[2]);
        }

        [Test]
        public void GenericTypeDefinition_non_generic()
        {
            var runtimeTypeInfo = new RuntimeTypeInfo(typeof(TestClass));

            Assert.AreSame(ReflectionHelper.EmptyTypeInfos, runtimeTypeInfo.GenericTypeParameters);
            Assert.AreSame(ReflectionHelper.EmptyTypeInfos, runtimeTypeInfo.GenericTypeArguments);
            Assert.IsNull(runtimeTypeInfo.GenericTypeDefinition);
        }

        [Test]
        public void GenericTypeDefinition_open_generic()
        {
            var runtimeTypeInfo = new RuntimeTypeInfo(typeof(IEnumerable<>));

            Assert.AreEqual(1, runtimeTypeInfo.GenericTypeParameters.Count);
            Assert.AreEqual(0, runtimeTypeInfo.GenericTypeArguments.Count);
            Assert.AreEqual("T", runtimeTypeInfo.GenericTypeParameters[0].Name);
            Assert.IsNull(runtimeTypeInfo.GenericTypeDefinition);
        }

        [Test]
        public void GenericTypeDefinition_closed_generic()
        {
            var runtimeTypeInfoDef = new RuntimeTypeInfo(typeof(IEnumerable<>));
            var runtimeTypeInfo = new RuntimeTypeInfo(typeof(IEnumerable<string>));

            Assert.AreEqual(0, runtimeTypeInfo.GenericTypeParameters.Count);
            Assert.AreEqual(1, runtimeTypeInfo.GenericTypeArguments.Count);
            Assert.AreEqual("String", runtimeTypeInfo.GenericTypeArguments[0].Name);
            Assert.IsNotNull(runtimeTypeInfo.GenericTypeDefinition);
            Assert.AreEqual("IEnumerable`1", runtimeTypeInfo.GenericTypeDefinition.Name);
        }

        [Test]
        public void Fields()
        {
            var runtimeTypeInfo = new RuntimeTypeInfo(typeof(TestClass));
            var fields = runtimeTypeInfo.Fields;
            Assert.IsTrue(fields.IsReadOnly);
            Assert.AreEqual(1, fields.Count);
            Assert.IsTrue(fields.ContainsKey(nameof(TestClass.PublicField)));
            Assert.IsFalse(fields.ContainsKey("PrivateField"));
        }

        [Test]
        public void Properties()
        {
            var runtimeTypeInfo = new RuntimeTypeInfo(typeof(TestClass));
            var properties = runtimeTypeInfo.Properties;
            Assert.IsTrue(properties.IsReadOnly);
            Assert.AreEqual(2, properties.Count);
            Assert.IsTrue(properties.ContainsKey(nameof(TestClass.Name)));
            Assert.IsTrue(properties.ContainsKey(nameof(TestClass.ReadOnlyFullName)));
        }

        [Test]
        public void Methods()
        {
            var runtimeTypeInfo = new RuntimeTypeInfo(typeof(TestClassWithOverloads));
            var methods = runtimeTypeInfo.Methods;
            Assert.IsTrue(methods.IsReadOnly);
            Assert.AreEqual(10, methods.Count);
            Assert.IsTrue(methods.ContainsKey(nameof(TestClassWithOverloads.GetType)));
            Assert.IsTrue(methods.ContainsKey(nameof(TestClassWithOverloads.Equals)));
            Assert.IsTrue(methods.ContainsKey(nameof(TestClassWithOverloads.GetHashCode)));
            Assert.IsTrue(methods.ContainsKey(nameof(TestClassWithOverloads.ToString)));
            Assert.IsTrue(methods.ContainsKey(nameof(TestClassWithOverloads.ComputeFullName)));
            Assert.IsTrue(methods.ContainsKey("get_" + nameof(TestClassWithOverloads.Name)));
            Assert.IsTrue(methods.ContainsKey("set_" + nameof(TestClassWithOverloads.Name)));
            Assert.IsTrue(methods.ContainsKey("get_" + nameof(TestClassWithOverloads.ReadOnlyFullName)));
            Assert.IsTrue(methods.ContainsKey("Finalize"));
            Assert.IsTrue(methods.ContainsKey("MemberwiseClone"));

            var overloads = methods[nameof(TestClassWithOverloads.ComputeFullName)];
            Assert.IsTrue(overloads.IsReadOnly);
            Assert.AreEqual(2, overloads.Count);
        }

        [Test]
        public void Members()
        {
            var runtimeTypeInfo = new RuntimeTypeInfo(typeof(TestClassWithOverloads));
            var members = runtimeTypeInfo.Members;

            var fields = runtimeTypeInfo.Fields;
            var properties = runtimeTypeInfo.Properties;
            var methods = runtimeTypeInfo.Methods;

            Assert.IsTrue(members.IsReadOnly);
            Assert.AreEqual(fields.Count + properties.Count + methods.Sum(m => m.Value.Count), members.Count);
        }

        [Test]
        public void GetMember_field()
        {
            var runtimeTypeInfo = new RuntimeTypeInfo(typeof(TestClass));
            var member = runtimeTypeInfo.GetMember(nameof(TestClass.PublicField));

            Assert.IsNotNull(member);
            Assert.AreEqual(nameof(TestClass.PublicField), member.Name);
            Assert.IsInstanceOf<RuntimeFieldInfo<TestClass, string>>(member);
        }

        [Test]
        public void GetMember_property()
        {
            var runtimeTypeInfo = new RuntimeTypeInfo(typeof(TestClass));
            var member = runtimeTypeInfo.GetMember(nameof(TestClass.Name));

            Assert.IsNotNull(member);
            Assert.AreEqual(nameof(TestClass.Name), member.Name);
            Assert.IsInstanceOf<RuntimePropertyInfo<TestClass, string>>(member);
        }

        [Test]
        public void GetMember_method()
        {
            var runtimeTypeInfo = new RuntimeTypeInfo(typeof(TestClass));
            var member = runtimeTypeInfo.GetMember(nameof(TestClass.ComputeFullName));

            Assert.IsNotNull(member);
            Assert.AreEqual(nameof(TestClass.ComputeFullName), member.Name);
            Assert.IsInstanceOf<RuntimeMethodInfo>(member);
        }

        [Test]
        public void GetMember_method_overloaded()
        {
            var runtimeTypeInfo = new RuntimeTypeInfo(typeof(TestClassWithOverloads));
            Assert.Throws<AmbiguousMatchException>(() => runtimeTypeInfo.GetMember(nameof(TestClass.ComputeFullName)));
        }

        public class TestClass
        {
            public string PublicField;

            private string PrivateField;

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

        public class TestClassWithOverloads : TestClass
        {
            public virtual string ComputeFullName(string parentsInitials, string arg2)
            {
                return parentsInitials + " " + this.Name + " " + arg2;
            }
        }
    }
}
