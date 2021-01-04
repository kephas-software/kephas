// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReflectionStringExtensionsTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the reflection string extensions test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests.Reflection
{
    using Kephas.Reflection;

    using NUnit.Framework;

    [TestFixture]
    public class ReflectionStringExtensionsTest
    {
        [Test]
        public void ToCamelCase_one_char()
        {
            var actual = ReflectionStringExtensions.ToCamelCase("A");
            Assert.AreEqual("a", actual);
        }

        [Test]
        public void ToCamelCase_common()
        {
            var actual = ReflectionStringExtensions.ToCamelCase("ToCamelCase");
            Assert.AreEqual("toCamelCase", actual);
        }

        [Test]
        public void ToCamelCase_multiple_capitals()
        {
            var actual = ReflectionStringExtensions.ToCamelCase("SSHKey");
            Assert.AreEqual("sshKey", actual);
        }

        [Test]
        public void ToCamelCase_all_caps()
        {
            var actual = ReflectionStringExtensions.ToCamelCase("PLUGINSFOLDER");
            Assert.AreEqual("PLUGINSFOLDER", actual);
        }

        [Test]
        public void ToCamelCase_double_underscores()
        {
            var actual = ReflectionStringExtensions.ToCamelCase("KEPHAS__PLUGINSFOLDER");
            Assert.AreEqual("KEPHAS__PLUGINSFOLDER", actual);
        }

        [Test]
        public void ToCamelCase_double_dashes()
        {
            var actual = ReflectionStringExtensions.ToCamelCase("KEPHAS--PLUGINSFOLDER");
            Assert.AreEqual("KEPHAS--PLUGINSFOLDER", actual);
        }

        [Test]
        public void ToPascalCase_one_char()
        {
            var actual = ReflectionStringExtensions.ToPascalCase("a");
            Assert.AreEqual("A", actual);
        }

        [Test]
        public void ToPascalCase_double_underscores()
        {
            var actual = ReflectionStringExtensions.ToPascalCase("KEPHAS__PLUGINSFOLDER");
            Assert.AreEqual("KEPHAS__PLUGINSFOLDER", actual);
        }

        [Test]
        public void ToPascalCase_double_dashes()
        {
            var actual = ReflectionStringExtensions.ToPascalCase("KEPHAS--PLUGINSFOLDER");
            Assert.AreEqual("KEPHAS--PLUGINSFOLDER", actual);
        }
    }
}