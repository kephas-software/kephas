// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeInfoExtensionsTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the type information extensions test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests.Reflection
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Kephas.Graphs;
    using Kephas.Reflection;
    using Kephas.Runtime;

    using NUnit.Framework;

    [TestFixture]
    public class TypeInfoExtensionsTest
    {
        [Test]
        public void IsGenericType_non_generic()
        {
            var typeInfo = RuntimeTypeInfo.CreateRuntimeTypeInfo(typeof(string));

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

            var qualifiedFullName = typeInfo.GetQualifiedFullName(stripVersionInfo: true);
#if NETCOREAPP2_0
            Assert.AreEqual("System.String, System.Private.CoreLib", qualifiedFullName);
#else
            Assert.AreEqual("System.String, mscorlib", qualifiedFullName);
#endif
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
            Assert.AreEqual(nameof(IRuntimeElementInfo.GetUnderlyingElementInfo), declaredMembers[0].Name);
        }

        [Test]
        public void GetBaseConstructedGenericOf_interface()
        {
            var typeInfo = typeof(string).GetTypeInfo();
            var constructedGenericType = typeInfo.GetBaseConstructedGenericOf(typeof(IEnumerable<>).GetTypeInfo());

            Assert.AreSame(constructedGenericType, typeof(IEnumerable<char>).GetTypeInfo());
        }

        [Test]
        public void GetBaseConstructedGenericOf_class()
        {
            var typeInfo = typeof(UnorientedGraph<string>).GetTypeInfo();
            var constructedGenericType = typeInfo.GetBaseConstructedGenericOf(typeof(Graph<>).GetTypeInfo());

            Assert.AreSame(constructedGenericType, typeof(Graph<string>).GetTypeInfo());
        }
    }
}