// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringExtensionsTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the reflection string extensions test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Tests.Reflection
{
    using NUnit.Framework;

    [TestFixture]
    public class StringExtensionsTest
    {
        [Test]
        public void ToCamelCase_one_char()
        {
            var actual = StringExtensions.ToCamelCase("A");
            Assert.AreEqual("a", actual);
        }

        [Test]
        public void ToCamelCase_common()
        {
            var actual = StringExtensions.ToCamelCase("ToCamelCase");
            Assert.AreEqual("toCamelCase", actual);
        }

        [Test]
        public void ToCamelCase_multiple_capitals()
        {
            var actual = StringExtensions.ToCamelCase("SSHKey");
            Assert.AreEqual("sshKey", actual);
        }

        [Test]
        public void ToCamelCase_all_caps()
        {
            var actual = StringExtensions.ToCamelCase("PLUGINSFOLDER");
            Assert.AreEqual("PLUGINSFOLDER", actual);
        }

        [Test]
        public void ToCamelCase_double_underscores()
        {
            var actual = StringExtensions.ToCamelCase("KEPHAS__PLUGINSFOLDER");
            Assert.AreEqual("KEPHAS__PLUGINSFOLDER", actual);
        }

        [Test]
        public void ToCamelCase_double_dashes()
        {
            var actual = StringExtensions.ToCamelCase("KEPHAS--PLUGINSFOLDER");
            Assert.AreEqual("KEPHAS--PLUGINSFOLDER", actual);
        }

        [Test]
        public void ToPascalCase_one_char()
        {
            var actual = StringExtensions.ToPascalCase("a");
            Assert.AreEqual("A", actual);
        }

        [Test]
        public void ToPascalCase_double_underscores()
        {
            var actual = StringExtensions.ToPascalCase("KEPHAS__PLUGINSFOLDER");
            Assert.AreEqual("KEPHAS__PLUGINSFOLDER", actual);
        }

        [Test]
        public void ToPascalCase_double_dashes()
        {
            var actual = StringExtensions.ToPascalCase("KEPHAS--PLUGINSFOLDER");
            Assert.AreEqual("KEPHAS--PLUGINSFOLDER", actual);
        }
    }
}