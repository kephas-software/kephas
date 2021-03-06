﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimePropertyInfoTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the runtime property information test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests.Runtime
{
    using System;

    using Kephas.Runtime;

    using NUnit.Framework;

    [TestFixture]
    public class RuntimePropertyInfoTest
    {
        [Test]
        public void ToString_name_and_type()
        {
            var registry = new RuntimeTypeRegistry();
            var propInfo = new RuntimePropertyInfo(registry, typeof(string).GetProperty(nameof(string.Length)));

            var toString = propInfo.ToString();
            Assert.AreEqual("Length: System.Int32", toString);
        }

        [Test]
        public void IsStatic()
        {
            var registry = new RuntimeTypeRegistry();
            var propInfo = new RuntimePropertyInfo(registry, typeof(Test).GetProperty(nameof(Test.OnlyGet)));

            Assert.IsFalse(propInfo.IsStatic);
        }

        [Test]
        public void CanWrite()
        {
            var registry = new RuntimeTypeRegistry();
            var propInfo = new RuntimePropertyInfo(registry, typeof(Test).GetProperty(nameof(Test.OnlyGet)));

            Assert.IsTrue(propInfo.CanRead);
            Assert.IsFalse(propInfo.CanWrite);
        }

        [Test]
        public void CanRead()
        {
            var registry = new RuntimeTypeRegistry();
            var propInfo = new RuntimePropertyInfo(registry, typeof(Test).GetProperty(nameof(Test.OnlySet)));

            Assert.IsFalse(propInfo.CanRead);
            Assert.IsTrue(propInfo.CanWrite);
        }

        [Test]
        public void SetValue()
        {
            var registry = new RuntimeTypeRegistry();
            var propInfo = new RuntimePropertyInfo(registry, typeof(Test).GetProperty(nameof(Test.ReadWrite)));

            var obj = new Test();
            propInfo.SetValue(obj, "gigi");
            Assert.AreEqual("gigi", obj.ReadWrite);
        }

        [Test]
        public void SetValue_exception()
        {
            var registry = new RuntimeTypeRegistry();
            var propInfo = new RuntimePropertyInfo(registry, typeof(Test).GetProperty(nameof(Test.OnlyGet)));

            var obj = new Test();
            Assert.Throws<MemberAccessException>(() => propInfo.SetValue(obj, "gigi"));
        }

        [Test]
        public void GetValue()
        {
            var registry = new RuntimeTypeRegistry();
            var propInfo = new RuntimePropertyInfo(registry, typeof(Test).GetProperty(nameof(Test.ReadWrite)));

            var obj = new Test();
            obj.ReadWrite = "gigi";
            Assert.AreEqual("gigi", propInfo.GetValue(obj));
        }

        [Test]
        public void GetValue_Exception()
        {
            var registry = new RuntimeTypeRegistry();
            var propInfo = new RuntimePropertyInfo(registry, typeof(Test).GetProperty(nameof(Test.OnlySet)));

            var obj = new Test();
            Assert.Throws<MemberAccessException>(() => propInfo.GetValue(obj));
        }

        public class Test
        {
            public string OnlyGet { get; }

            public string OnlySet { set { } }

            public string ReadWrite { get; set; }
        }
    }
}