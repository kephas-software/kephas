namespace Kephas.Core.Tests.Reflection
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

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

        [Test]
        public void GetQualifiedFullName_version_info_stripped()
        {
            var typeInfo = typeof(string).GetTypeInfo();

            Assert.AreEqual("System.String, mscorlib", typeInfo.GetQualifiedFullName(stripVersionInfo: true));
        }

        [Test]
        public void GetQualifiedFullName_version_info_not_stripped()
        {
            var typeInfo = typeof(string).GetTypeInfo();

            Assert.AreEqual(typeInfo.AssemblyQualifiedName, typeInfo.GetQualifiedFullName(stripVersionInfo: false));
        }

        [Test]
        public void GetDeclaredMembers()
        {
            var typeInfo = typeof(IRuntimeElementInfo).AsRuntimeTypeInfo();

            var declaredMembers = typeInfo.GetDeclaredMembers().ToList();
            Assert.AreEqual(1, declaredMembers.Count);
            Assert.AreEqual(nameof(IRuntimeElementInfo.GetUnderlyingMemberInfo), declaredMembers[0].Name);
        }
    }
}