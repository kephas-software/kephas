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
    }
}