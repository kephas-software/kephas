namespace Kephas.Core.Tests.Reflection
{
    using System.Collections.Generic;

    using Kephas.Dynamic;
    using Kephas.Reflection;
    using Kephas.Runtime;

    using NUnit.Framework;

    [TestFixture]
    public class TypeInfoExtensionsTest
    {
        [Test]
        public void IsGenericType_non_generic()
        {
            var typeInfo = new RuntimeTypeInfo(typeof(string));

            Assert.IsFalse(typeInfo.IsGenericType());
        }

        [Test]
        public void IsGenericType_open_generic()
        {
            var typeInfo = new RuntimeTypeInfo(typeof(IEnumerable<>));

            Assert.IsTrue(typeInfo.IsGenericType());
        }

        [Test]
        public void IsGenericType_closed_generic()
        {
            var typeInfo = new RuntimeTypeInfo(typeof(IEnumerable<string>));

            Assert.IsTrue(typeInfo.IsGenericType());
        }

        [Test]
        public void IsGenericTypeDefinition_non_generic()
        {
            var typeInfo = new RuntimeTypeInfo(typeof(string));

            Assert.IsFalse(typeInfo.IsGenericTypeDefinition());
        }

        [Test]
        public void IsGenericTypeDefinition_open_generic()
        {
            var typeInfo = new RuntimeTypeInfo(typeof(IEnumerable<>));

            Assert.IsTrue(typeInfo.IsGenericTypeDefinition());
        }

        [Test]
        public void IsGenericTypeDefinition_closed_generic()
        {
            var typeInfo = new RuntimeTypeInfo(typeof(IEnumerable<string>));

            Assert.IsFalse(typeInfo.IsGenericTypeDefinition());
        }
    }
}