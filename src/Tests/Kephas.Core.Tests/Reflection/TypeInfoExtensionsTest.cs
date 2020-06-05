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
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    using Kephas.Logging;
    using Kephas.Reflection;
    using Kephas.Runtime;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class TypeInfoExtensionsTest
    {
        [Test]
        public void IsGenericType_open_generic()
        {
            var typeInfo = (ITypeInfo)new RuntimeTypeInfo(typeof(IEnumerable<>));

            Assert.IsTrue(typeInfo.IsGenericType());
        }

        [Test]
        public void IsGenericType_closed_generic()
        {
            var typeInfo = (ITypeInfo)new RuntimeTypeInfo(typeof(IEnumerable<string>));

            Assert.IsTrue(typeInfo.IsGenericType());
        }

        [Test]
        public void IsGenericTypeDefinition_non_generic()
        {
            var typeInfo = (ITypeInfo)new RuntimeTypeInfo(typeof(string));

            Assert.IsFalse(typeInfo.IsGenericTypeDefinition());
        }

        [Test]
        public void IsGenericTypeDefinition_open_generic()
        {
            var typeInfo = (ITypeInfo)new RuntimeTypeInfo(typeof(IEnumerable<>));

            Assert.IsTrue(typeInfo.IsGenericTypeDefinition());
        }

        [Test]
        public void IsGenericTypeDefinition_closed_generic()
        {
            var typeInfo = (ITypeInfo)new RuntimeTypeInfo(typeof(IEnumerable<string>));

            Assert.IsFalse(typeInfo.IsGenericTypeDefinition());
        }

        [Test]
        public void Type_contains_IsSecurityTransparent_property_DateTime()
        {
            var log = new StringBuilder();
            var logger = Substitute.For<ILogger>();
            logger.IsEnabled(Arg.Any<LogLevel>()).Returns(true);
            logger.Log(Arg.Any<LogLevel>(), Arg.Any<Exception>(), Arg.Any<string>(), Arg.Any<object[]>())
                .Returns(ci => { log.AppendLine($"{ci.Arg<LogLevel>()} {ci.Arg<string>()}-{ci.Arg<object[]>()?.FirstOrDefault()}"); return true; });
            var typeInfo = new RuntimeTypeInfo(typeof(DateTime).GetTypeInfo(), logger);

            object dateTime = DateTime.Now;
            var date = typeInfo.GetValue(dateTime, "Date");

#if NET461
            Assert.IsEmpty(log.ToString());
#else
            Assert.AreEqual("Trace Cannot compute getter delegate for {typeName}.{methodName}, falling back to reflection.-System.DateTime" + Environment.NewLine, log.ToString());
#endif
            Assert.AreEqual(((DateTime)dateTime).Date, date);
        }

        [Test]
        public void GetQualifiedFullName_version_info_stripped()
        {
            var typeInfo = typeof(string).GetTypeInfo();

            var qualifiedFullName = typeInfo.GetQualifiedFullName(stripVersionInfo: true);
#if NETCOREAPP3_1 || NETCOREAPP2_1
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
            var typeInfo = typeof(TestDerivedGeneric<string>).GetTypeInfo();
            var constructedGenericType = typeInfo.GetBaseConstructedGenericOf(typeof(TestGeneric<>).GetTypeInfo());

            Assert.AreSame(constructedGenericType, typeof(TestGeneric<string>).GetTypeInfo());
        }

        public class TestGeneric<T> { }
        public class TestDerivedGeneric<T> : TestGeneric<T> { }
    }
}