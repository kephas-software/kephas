// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimeTypeInfoTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
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
    using Kephas.Core.Tests.Runtime.RuntimeTypeInfoFactory;
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
            var registry = new RuntimeTypeRegistry();
            var runtimeTypeInfo = new RuntimeTypeInfo(registry, typeof(TestClass));
            var type = runtimeTypeInfo.Type;
            Assert.AreEqual(type, typeof(TestClass));
        }

        [Test]
        public void RuntimeTypeInfo_open_generic_properties()
        {
            var registry = new RuntimeTypeRegistry();
            var runtimeTypeInfo = new RuntimeTypeInfo(registry, typeof(ICollection<>));
            var type = runtimeTypeInfo.Type;
            Assert.AreEqual(type, typeof(ICollection<>));
            Assert.AreEqual(0, runtimeTypeInfo.Fields.Count);
            Assert.AreEqual(2, runtimeTypeInfo.Properties.Count);
            Assert.AreEqual(7, runtimeTypeInfo.Methods.Count);
        }

        [Test]
        public void RuntimeTypeInfo_open_generic_fields()
        {
            var registry = new RuntimeTypeRegistry();
            var runtimeTypeInfo = new RuntimeTypeInfo(registry, typeof(OpenGenericFields<>));
            var type = runtimeTypeInfo.Type;
            Assert.AreEqual(type, typeof(OpenGenericFields<>));
            Assert.AreEqual(1, runtimeTypeInfo.Fields.Count);
            Assert.AreEqual(0, runtimeTypeInfo.Properties.Count);
            Assert.AreEqual(4, runtimeTypeInfo.Methods.Count);
        }

        [Test]
        public void Name()
        {
            var registry = new RuntimeTypeRegistry();
            var runtimeTypeInfo = new RuntimeTypeInfo(registry, typeof(TestClass));
            Assert.AreEqual("TestClass", runtimeTypeInfo.Name);
        }

        [Test]
        public void FullName()
        {
            var registry = new RuntimeTypeRegistry();
            var runtimeTypeInfo = new RuntimeTypeInfo(registry, typeof(TestClass));
            Assert.AreEqual("Kephas.Core.Tests.Runtime.RuntimeTypeInfoTest+TestClass", runtimeTypeInfo.FullName);
        }

        [Test]
        public void QualifiedFullName()
        {
            var registry = new RuntimeTypeRegistry();
            var runtimeTypeInfo = new RuntimeTypeInfo(registry, typeof(TestClass));
            Assert.AreEqual("Kephas.Core.Tests.Runtime.RuntimeTypeInfoTest+TestClass, Kephas.Reflection.Tests", runtimeTypeInfo.QualifiedFullName);
        }

        [Test]
        public void Equals_two_instances_based_on_the_same_type()
        {
            var registry = new RuntimeTypeRegistry();
            var runtimeTypeInfo = new RuntimeTypeInfo(registry, typeof(TestClass));
            var runtimeTypeInfo2 = new RuntimeTypeInfo(registry, typeof(TestClass));
            Assert.AreEqual(runtimeTypeInfo, runtimeTypeInfo2);
        }

        [Test]
        public void Equals_two_instances_based_on_the_same_type_different_registries()
        {
            var registry = new RuntimeTypeRegistry();
            var registry2 = new RuntimeTypeRegistry();
            var runtimeTypeInfo = new RuntimeTypeInfo(registry, typeof(TestClass));
            var runtimeTypeInfo2 = new RuntimeTypeInfo(registry2, typeof(TestClass));
            Assert.AreEqual(runtimeTypeInfo, runtimeTypeInfo2);
        }

        [Test]
        public void op_Equality_two_instances_based_on_the_same_type()
        {
            var registry = new RuntimeTypeRegistry();
            var runtimeTypeInfo = new RuntimeTypeInfo(registry, typeof(TestClass));
            var runtimeTypeInfo2 = new RuntimeTypeInfo(registry, typeof(TestClass));
            Assert.IsTrue(runtimeTypeInfo == runtimeTypeInfo2);
        }

        [Test]
        public void op_Equality_two_instances_based_on_the_same_type_different_registries()
        {
            var registry = new RuntimeTypeRegistry();
            var registry2 = new RuntimeTypeRegistry();
            var runtimeTypeInfo = new RuntimeTypeInfo(registry, typeof(TestClass));
            var runtimeTypeInfo2 = new RuntimeTypeInfo(registry2, typeof(TestClass));
            Assert.IsTrue(runtimeTypeInfo == runtimeTypeInfo2);
        }

        [Test]
        public void op_Inequality_two_instances_based_on_the_same_type()
        {
            var registry = new RuntimeTypeRegistry();
            var runtimeTypeInfo = new RuntimeTypeInfo(registry, typeof(TestClass));
            var runtimeTypeInfo2 = new RuntimeTypeInfo(registry, typeof(TestClass));
            Assert.IsFalse(runtimeTypeInfo != runtimeTypeInfo2);
        }

        [Test]
        public void op_Inequality_two_instances_based_on_the_same_type_different_registries()
        {
            var registry = new RuntimeTypeRegistry();
            var registry2 = new RuntimeTypeRegistry();
            var runtimeTypeInfo = new RuntimeTypeInfo(registry, typeof(TestClass));
            var runtimeTypeInfo2 = new RuntimeTypeInfo(registry2, typeof(TestClass));
            Assert.IsFalse(runtimeTypeInfo != runtimeTypeInfo2);
        }

        [Test]
        public void GetValue_instance_null_throws()
        {
            var registry = new RuntimeTypeRegistry();
            var runtimeTypeInfo = new RuntimeTypeInfo(registry, typeof(TestClass));
            Assert.That(() => runtimeTypeInfo.GetValue(null, string.Empty), Throws.InstanceOf<Exception>());
        }

        [Test]
        public void GetValue_instance_not_null()
        {
            var registry = new RuntimeTypeRegistry();
            var runtimeTypeInfo = new RuntimeTypeInfo(registry, typeof(TestClass));
            var instance = new TestClass { Name = "noName" };
            var result = runtimeTypeInfo.GetValue(instance, "Name");
            Assert.AreEqual(instance.Name, result);
        }

        [Test]
        public void TryGetValue_instance_null_returns_undefined()
        {
            var registry = new RuntimeTypeRegistry();
            var runtimeTypeInfo = new RuntimeTypeInfo(registry, typeof(TestClass));
            Assert.Throws<ArgumentNullException>(() => runtimeTypeInfo.TryGetValue(null, string.Empty, out var result));
        }

        [Test]
        public void TryGetValue_instance_not_null_valid_property()
        {
            var registry = new RuntimeTypeRegistry();
            var runtimeTypeInfo = new RuntimeTypeInfo(registry, typeof(TestClass));
            var instance = new TestClass { Name = "NoName" };
            var success = runtimeTypeInfo.TryGetValue(instance, "Name", out var result);
            Assert.AreEqual(instance.Name, result);
            Assert.IsTrue(success);
        }

        [Test]
        public void TryGetValue_instance_not_null_invalid_property()
        {
            var registry = new RuntimeTypeRegistry();
            var runtimeTypeInfo = new RuntimeTypeInfo(registry, typeof(TestClass));
            var instance = new TestClass { Name = "NoName" };
            var success = runtimeTypeInfo.TryGetValue(instance, "nothing", out var result);
            Assert.IsNull(result);
            Assert.IsFalse(success);
        }

        [Test]
        public void SetValue_instance_null_throws()
        {
            var registry = new RuntimeTypeRegistry();
            var runtimeTypeInfo = new RuntimeTypeInfo(registry, typeof(TestClass));
            Assert.That(() => runtimeTypeInfo.SetValue(null, string.Empty, null), Throws.InstanceOf<Exception>());
        }

        [Test]
        public void SetValue_valid_instance()
        {
            var registry = new RuntimeTypeRegistry();
            var runtimeTypeInfo = new RuntimeTypeInfo(registry, typeof(TestClass));
            var instance = new TestClass();
            const string value = "someName";
            runtimeTypeInfo.SetValue(instance, "Name", value);
            Assert.AreEqual(value, instance.Name);
        }

        [Test]
        public void TrySetValue_instance_null_returns_false()
        {
            var registry = new RuntimeTypeRegistry();
            var runtimeTypeInfo = new RuntimeTypeInfo(registry, typeof(TestClass));
            var result = runtimeTypeInfo.TrySetValue(null, string.Empty, null);
            Assert.AreEqual(false, result);
        }

        [Test]
        public void Invoke_valid_instance()
        {
            var registry = new RuntimeTypeRegistry();
            var runtimeTypeInfo = new RuntimeTypeInfo(registry, typeof(TestClass));
            var instance = new TestClass { Name = "someName" };
            var list = new List<string> { "IC" };
            var ienum = (IEnumerable<object>)list;
            var result = runtimeTypeInfo.Invoke(instance, "ComputeFullName", ienum);
            Assert.AreEqual(instance.ComputeFullName("IC"), result);
        }

        [Test]
        public void Invoke_instance_null_throws()
        {
            var registry = new RuntimeTypeRegistry();
            var runtimeTypeInfo = new RuntimeTypeInfo(registry, typeof(TestClass));
            var list = new List<string>();
            var ienum = (IEnumerable<object>)list;
            Assert.That(() => runtimeTypeInfo.Invoke(null, string.Empty, ienum), Throws.InstanceOf<Exception>());
        }

        [Test]
        public void TryInvoke_instance_null_returns_undefined()
        {
            var registry = new RuntimeTypeRegistry();
            var runtimeTypeInfo = new RuntimeTypeInfo(registry, typeof(TestClass));
            var list = new List<string>();
            var ienum = (IEnumerable<object>)list;
            Assert.Throws<ArgumentNullException>(() => runtimeTypeInfo.TryInvoke(null, string.Empty, ienum, out var result));
        }

        [Test]
        public void TryInvoke_instance_non_existing_method_returns_undefined()
        {
            var registry = new RuntimeTypeRegistry();
            var runtimeTypeInfo = new RuntimeTypeInfo(registry, typeof(TestClass));
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
            var registry = new RuntimeTypeRegistry();
            var runtimeTypeInfo = new RuntimeTypeInfo(registry, typeof(IActivator));
            Assert.Throws<InvalidOperationException>(() => runtimeTypeInfo.CreateInstance());
        }

        [Test]
        public void CreateInstance_exception_private_constructor()
        {
            var registry = new RuntimeTypeRegistry();
            var runtimeTypeInfo = new RuntimeTypeInfo(registry, typeof(TestClassWithConstructor));
            Assert.Throws<MissingMethodException>(() => runtimeTypeInfo.CreateInstance(new object[] { 3, "Hello" }));
        }

        [Test]
        public void CreateInstance_no_args()
        {
            var registry = new RuntimeTypeRegistry();
            var runtimeTypeInfo = new RuntimeTypeInfo(registry, typeof(TestClass));
            var instance = runtimeTypeInfo.CreateInstance();
            Assert.IsInstanceOf<TestClass>(instance);
        }

        [Test]
        public void CreateInstance_args()
        {
            var registry = new RuntimeTypeRegistry();
            var runtimeTypeInfo = new RuntimeTypeInfo(registry, typeof(TestClassWithConstructor));
            var instance = runtimeTypeInfo.CreateInstance(3);
            Assert.IsInstanceOf<TestClassWithConstructor>(instance);
        }

        [Test]
        public void GetValue_throwOnNotFound_DynamicProperty()
        {
            var registry = new RuntimeTypeRegistry();
            var runtimeTypeInfo = new RuntimeTypeInfo(registry, typeof(TestClass));
            object instance = new TestClass();
            Assert.Throws<MemberAccessException>(() => runtimeTypeInfo.GetValue(instance, string.Empty));
        }

        [Test]
        public void Bases_SystemObject()
        {
            var registry = new RuntimeTypeRegistry();
            var runtimeTypeInfo = new RuntimeTypeInfo(registry, typeof(object));

            Assert.AreEqual(0, runtimeTypeInfo.BaseTypes.Count());
        }

        [Test]
        public void Bases_class_without_base()
        {
            var registry = new RuntimeTypeRegistry();
            var runtimeTypeInfo = new RuntimeTypeInfo(registry, typeof(TestClass));

            Assert.AreEqual(1, runtimeTypeInfo.BaseTypes.Count());
            Assert.AreEqual("System.Object", runtimeTypeInfo.BaseTypes.First().FullName);
        }

        [Test]
        public void Bases_class_with_base()
        {
            var registry = new RuntimeTypeRegistry();
            var runtimeTypeInfo = new RuntimeTypeInfo(registry, typeof(TestDerivedClass));

            Assert.AreEqual(1, runtimeTypeInfo.BaseTypes.Count());
            Assert.AreSame(registry.GetTypeInfo(typeof(TestClass)), runtimeTypeInfo.BaseTypes.First());
        }

        [Test]
        public void Bases_class_with_base_and_interfaces()
        {
            var registry = new RuntimeTypeRegistry();
            var runtimeTypeInfo = new RuntimeTypeInfo(registry, typeof(TestDerivedClassWithInterfaces));

            var bases = runtimeTypeInfo.BaseTypes.ToList();
            Assert.AreEqual(3, bases.Count);
            Assert.AreSame(registry.GetTypeInfo(typeof(TestClass)), bases[0]);
            Assert.AreSame(registry.GetTypeInfo(typeof(IEnumerable<int>)), bases[1]);
            Assert.AreSame(registry.GetTypeInfo(typeof(IEnumerable)), bases[2]);
        }

        [Test]
        public void GenericTypeDefinition_non_generic()
        {
            var registry = new RuntimeTypeRegistry();
            var runtimeTypeInfo = new RuntimeTypeInfo(registry, typeof(TestClass));

            Assert.AreSame(Array.Empty<ITypeInfo>(), runtimeTypeInfo.GenericTypeParameters);
            Assert.AreSame(Array.Empty<ITypeInfo>(), runtimeTypeInfo.GenericTypeArguments);
            Assert.IsNull(runtimeTypeInfo.GenericTypeDefinition);
        }

        [Test]
        public void GenericTypeDefinition_open_generic()
        {
            var registry = new RuntimeTypeRegistry();
            var runtimeTypeInfo = new RuntimeTypeInfo(registry, typeof(IEnumerable<>));

            Assert.AreEqual(1, runtimeTypeInfo.GenericTypeParameters.Count);
            Assert.AreEqual(0, runtimeTypeInfo.GenericTypeArguments.Count);
            Assert.AreEqual("T", runtimeTypeInfo.GenericTypeParameters[0].Name);
            Assert.IsNull(runtimeTypeInfo.GenericTypeDefinition);
        }

        [Test]
        public void GenericTypeDefinition_closed_generic()
        {
            var registry = new RuntimeTypeRegistry();
            var runtimeTypeInfoDef = new RuntimeTypeInfo(registry, typeof(IEnumerable<>));
            var runtimeTypeInfo = new RuntimeTypeInfo(registry, typeof(IEnumerable<string>));

            Assert.AreEqual(0, runtimeTypeInfo.GenericTypeParameters.Count);
            Assert.AreEqual(1, runtimeTypeInfo.GenericTypeArguments.Count);
            Assert.AreEqual("String", runtimeTypeInfo.GenericTypeArguments[0].Name);
            Assert.IsNotNull(runtimeTypeInfo.GenericTypeDefinition);
            Assert.AreEqual("IEnumerable`1", runtimeTypeInfo.GenericTypeDefinition.Name);
        }

        [Test]
        public void Fields()
        {
            var registry = new RuntimeTypeRegistry();
            var runtimeTypeInfo = new RuntimeTypeInfo(registry, typeof(TestClass));
            var fields = runtimeTypeInfo.Fields;
            Assert.IsTrue(fields.IsReadOnly);
            Assert.AreEqual(1, fields.Count);
            Assert.IsTrue(fields.ContainsKey(nameof(TestClass.PublicField)));
            Assert.IsFalse(fields.ContainsKey("PrivateField"));
        }

        [Test]
        public void Properties()
        {
            var registry = new RuntimeTypeRegistry();
            var runtimeTypeInfo = new RuntimeTypeInfo(registry, typeof(TestClass));
            var properties = runtimeTypeInfo.Properties;
            Assert.IsTrue(properties.IsReadOnly);
            Assert.AreEqual(2, properties.Count);
            Assert.IsTrue(properties.ContainsKey(nameof(TestClass.Name)));
            Assert.IsTrue(properties.ContainsKey(nameof(TestClass.ReadOnlyFullName)));
        }

        [Test]
        public void Methods()
        {
            var registry = new RuntimeTypeRegistry();
            var runtimeTypeInfo = new RuntimeTypeInfo(registry, typeof(TestClassWithOverloads));
            var methods = runtimeTypeInfo.Methods;
            Assert.IsTrue(methods.IsReadOnly);
            Assert.AreEqual(8, methods.Count);
            Assert.IsTrue(methods.ContainsKey(nameof(TestClassWithOverloads.GetType)));
            Assert.IsTrue(methods.ContainsKey(nameof(TestClassWithOverloads.Equals)));
            Assert.IsTrue(methods.ContainsKey(nameof(TestClassWithOverloads.GetHashCode)));
            Assert.IsTrue(methods.ContainsKey(nameof(TestClassWithOverloads.ToString)));
            Assert.IsTrue(methods.ContainsKey(nameof(TestClassWithOverloads.ComputeFullName)));
            Assert.IsTrue(methods.ContainsKey("get_" + nameof(TestClassWithOverloads.Name)));
            Assert.IsTrue(methods.ContainsKey("set_" + nameof(TestClassWithOverloads.Name)));
            Assert.IsTrue(methods.ContainsKey("get_" + nameof(TestClassWithOverloads.ReadOnlyFullName)));

            var overloads = methods[nameof(TestClassWithOverloads.ComputeFullName)];
            Assert.IsTrue(overloads.IsReadOnly);
            Assert.AreEqual(2, overloads.Count);
        }

        [Test]
        public void Members()
        {
            var registry = new RuntimeTypeRegistry();
            var runtimeTypeInfo = new RuntimeTypeInfo(registry, typeof(TestClassWithOverloads));
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
            var registry = new RuntimeTypeRegistry();
            var runtimeTypeInfo = new RuntimeTypeInfo(registry, typeof(TestClass));
            var member = runtimeTypeInfo.GetMember(nameof(TestClass.PublicField));

            Assert.IsNotNull(member);
            Assert.AreEqual(nameof(TestClass.PublicField), member.Name);
            Assert.IsInstanceOf<RuntimeFieldInfo>(member);
        }

        [Test]
        public void GetMember_property()
        {
            var registry = new RuntimeTypeRegistry();
            var runtimeTypeInfo = new RuntimeTypeInfo(registry, typeof(TestClass));
            var member = runtimeTypeInfo.GetMember(nameof(TestClass.Name));

            Assert.IsNotNull(member);
            Assert.AreEqual(nameof(TestClass.Name), member.Name);
            Assert.IsInstanceOf<RuntimePropertyInfo>(member);
        }

        [Test]
        public void GetMember_method()
        {
            var registry = new RuntimeTypeRegistry();
            var runtimeTypeInfo = new RuntimeTypeInfo(registry, typeof(TestClass));
            var member = runtimeTypeInfo.GetMember(nameof(TestClass.ComputeFullName));

            Assert.IsNotNull(member);
            Assert.AreEqual(nameof(TestClass.ComputeFullName), member.Name);
            Assert.IsInstanceOf<RuntimeMethodInfo>(member);
        }

        [Test]
        public void GetMember_method_overloaded()
        {
            var registry = new RuntimeTypeRegistry();
            var runtimeTypeInfo = new RuntimeTypeInfo(registry, typeof(TestClassWithOverloads));
            Assert.Throws<AmbiguousMatchException>(() => runtimeTypeInfo.GetMember(nameof(TestClass.ComputeFullName)));
        }

        [Test]
        public void DeclaringContainer()
        {
            var registry = new RuntimeTypeRegistry();
            var runtimeTypeInfo = new RuntimeTypeInfo(registry, typeof(TestClass));
            var declaringContainer = runtimeTypeInfo.DeclaringContainer as IRuntimeAssemblyInfo;

            Assert.IsNotNull(declaringContainer);
            Assert.AreSame(typeof(TestClass).Assembly, declaringContainer.GetUnderlyingAssemblyInfo());
        }

        [Test]
        public void GetAttributes_default()
        {
            var registry = new RuntimeTypeRegistry();
            var runtimeTypeInfo = new RuntimeTypeInfo(registry, typeof(GetAttributes.BaseType));
            var attrs = runtimeTypeInfo.GetAttributes<Attribute>().ToList();

            Assert.AreEqual(2, attrs.Count);
            Assert.IsTrue(attrs.OfType<GetAttributes.InheritableAttribute>().Any());
            Assert.IsTrue(attrs.OfType<GetAttributes.NonInheritableAttribute>().Any());
        }

        [Test]
        public void GetAttributes_inherited()
        {
            var registry = new RuntimeTypeRegistry();
            var runtimeTypeInfo = new RuntimeTypeInfo(registry, typeof(GetAttributes.DerivedType));
            var attrs = runtimeTypeInfo.GetAttributes<Attribute>().ToList();

            Assert.AreEqual(2, attrs.Count);
            Assert.IsTrue(attrs.OfType<GetAttributes.InheritableAttribute>().Any());
            Assert.IsTrue(attrs.OfType<GetAttributes.NonDerivedInheritableAttribute>().Any());
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

        public class OpenGenericFields<T>
        {
            [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Reviewed. Suppression is OK here.")]
            public T Value;
        }
    }

    namespace GetAttributes
    {
        [Inheritable]
        [NonInheritable]
        public class BaseType { }

        [NonDerivedInheritable]
        public class DerivedType : BaseType { }

        [AttributeUsage(AttributeTargets.Class, Inherited = true)]
        public class InheritableAttribute : Attribute { }

        [AttributeUsage(AttributeTargets.Class, Inherited = false)]
        public class NonInheritableAttribute : Attribute { }

        [AttributeUsage(AttributeTargets.Class, Inherited = false)]
        public class NonDerivedInheritableAttribute : Attribute { }
    }
}
