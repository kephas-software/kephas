// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimeFieldInfoTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the runtime field information test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests.Runtime
{
    using Kephas.Runtime;

    using NUnit.Framework;

    [TestFixture]
    public class RuntimeFieldInfoTest
    {
        [Test]
        public void ToString_name_and_type()
        {
            var fieldInfo = new RuntimeFieldInfo<TestField, string>(typeof(TestField).GetField(nameof(TestField.OnlyGet)));

            var toString = fieldInfo.ToString();
            Assert.AreEqual("OnlyGet: System.String", toString);
        }

        [Test]
        public void GetValue()
        {
            var fieldInfo = new RuntimeFieldInfo<TestField, string>(typeof(TestField).GetField(nameof(TestField.OnlyGet)));

            var obj = new TestField();
            Assert.AreEqual("get me", fieldInfo.GetValue(obj));
        }

        [Test]
        public void SetValue()
        {
            var fieldInfo = new RuntimeFieldInfo<TestField, string>(typeof(TestField).GetField(nameof(TestField.ReadWrite)));

            var obj = new TestField();
            fieldInfo.SetValue(obj, "read and write");
            Assert.AreEqual("read and write", obj.ReadWrite);
        }
    }

    public class TestField
    {
        public readonly string OnlyGet = "get me";

        public string ReadWrite;
    }
}