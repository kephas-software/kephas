// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReflectionHelperTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Test class for <see cref="ReflectionHelper" />.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Tests.Reflection
{
    using System.Collections.Generic;

    using Kephas.Dynamic;
    using Kephas.Reflection;
    using NSubstitute;
    using NUnit.Framework;

    /// <summary>
    /// Test class for <see cref="ReflectionHelper"/>.
    /// </summary>
    [TestFixture]
    public class ReflectionHelperTest
    {
        [Test]
        public void GetNonGenericName_non_generic()
        {
            var name = ReflectionHelper.GetNonGenericFullName(typeof(string));
            Assert.AreEqual("System.String", name);
        }

        [Test]
        public void GetNonGenericName_generic()
        {
            var name = ReflectionHelper.GetNonGenericFullName(typeof(IEnumerable<>));
            Assert.AreEqual("System.Collections.Generic.IEnumerable", name);
        }

        [Test]
        public void AsRuntimeAssemblyInfo()
        {
            var assemblyInfo = ReflectionExtensions.AsRuntimeAssemblyInfo(this.GetType().Assembly);
            Assert.AreSame(assemblyInfo.GetUnderlyingAssemblyInfo(), this.GetType().Assembly);
        }

        [Test]
        public void GetPropertyName()
        {
            var propName = ReflectionHelper.GetPropertyName<ITypeInfo>(t => t.Name);
            Assert.AreEqual(nameof(ITypeInfo.Name), propName);
        }

        [Test]
        public void GetTypeInfo_non_IInstance()
        {
            var typeInfo = ReflectionExtensions.GetTypeInfo("123");
            Assert.AreSame(typeof(string).AsRuntimeTypeInfo(), typeInfo);
        }

        [Test]
        public void GetTypeInfo_IInstance()
        {
            var instance = Substitute.For<IInstance>();
            var typeInfo = Substitute.For<ITypeInfo>();
            instance.GetTypeInfo().Returns(typeInfo);

            var obj = (object)instance;
            var objTypeInfo = ReflectionExtensions.GetTypeInfo(obj);

            Assert.AreSame(typeInfo, objTypeInfo);
        }

        [TestCase(typeof(string), "Name", "String")]
        [TestCase("123", "Length", 3)]
        [Test]
        public void GetValue_simple(object target, string name, object? value)
        {
            Assert.AreEqual(value, ReflectionHelper.GetValue(target, name));
        }

        [TestCase("name", "John Doe")]
        [Test]
        public void GetValue_dynamic(string name, object? value)
        {
            var expando = new DictionaryExpando<object?>(new Dictionary<string, object?> { { "name", "John Doe" } });
            Assert.AreEqual(value, ReflectionHelper.GetValue(expando, name));
        }

        [TestCase("Name", "Jane Doe")]
        [TestCase("Age", 3)]
        [Test]
        public void SetValue_simple(string name, object? value)
        {
            var person = new Person { Name = "John Doe", Age = 23 };
            ReflectionHelper.SetValue(person, name, value);
            Assert.AreEqual(value, ReflectionHelper.GetValue(person, name));
        }

        [TestCase("Name", "Jane Doe")]
        [TestCase("Age", 3)]
        [Test]
        public void SetValue_dynamic(string name, object? value)
        {
            var person = new ObjectExpando(new Person { Name = "John Doe", Age = 23 });
            ReflectionHelper.SetValue(person, name, value);
            Assert.AreEqual(value, ReflectionHelper.GetValue(person, name));
        }

        public class Person
        {
            public string? Name { get; set; }

            public int Age { get; set; }
        }
    }
}