// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PermissionTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Security.Permissions.Tests
{
    using NUnit.Framework;

    [TestFixture]
    public class PermissionTest
    {
        [Test]
        public void Parse_with_token()
        {
            var pi = Permission.Parse("token");

            Assert.AreEqual("token", pi.TokenName);
            Assert.IsNull(pi.Scope);
            Assert.IsNull(pi.Sections);
        }

        [Test]
        public void Parse_with_scope()
        {
            var pi = Permission.Parse("token:scope");

            Assert.AreEqual("token", pi.TokenName);
            Assert.AreEqual("scope", pi.Scope);
            Assert.IsNull(pi.Sections);
        }

        [Test]
        public void Parse_with_sections()
        {
            var pi = Permission.Parse("token:scope:s1:s2");

            Assert.AreEqual("token", pi.TokenName);
            Assert.AreEqual("scope", pi.Scope);
            CollectionAssert.AreEqual(new[] { "s1", "s2" }, pi.Sections);
        }

        [Test]
        public void ToString_with_token()
        {
            var pi = Permission.Parse("token");
            Assert.AreEqual("token", pi.ToString());
        }

        [Test]
        public void ToString_with_scope()
        {
            var pi = Permission.Parse("token:scope");
            Assert.AreEqual("token:scope", pi.ToString());
        }

        [Test]
        public void ToString_with_sections()
        {
            var pi = Permission.Parse("token:scope:s1:s2");
            Assert.AreEqual("token:scope:s1:s2", pi.ToString());
        }

        [Test]
        public void Equals()
        {
            Assert.AreEqual(Permission.Parse("token:scope"), Permission.Parse("token:scope"));
        }

        [Test]
        public void NotEquals()
        {
            Assert.AreNotEqual(Permission.Parse("token:scope"), Permission.Parse("token:scope:a1"));
        }
    }
}