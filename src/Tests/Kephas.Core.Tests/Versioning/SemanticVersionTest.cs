// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SemanticVersionTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the semantic version test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests.Versioning
{
    using System.Linq;

    using Kephas.Versioning;
    using NUnit.Framework;

    [TestFixture]
    public class SemanticVersionTest
    {
        [Test]
        public void Parse_complete()
        {
            var version = SemanticVersion.Parse("2.1.4-dev.12+1234");

            Assert.AreEqual(2, version.Major);
            Assert.AreEqual(1, version.Minor);
            Assert.AreEqual(4, version.Patch);
            Assert.AreEqual("dev", version.ReleaseLabels.First());
            Assert.AreEqual("12", version.ReleaseLabels.Skip(1).First());
            Assert.AreEqual("1234", version.Metadata);
        }

        [Test]
        public void CompareTo_release_greater()
        {
            var version = SemanticVersion.Parse("2.1.4-dev.122");
            var version2 = SemanticVersion.Parse("2.1.4-dev.21");

            Assert.IsTrue(version.CompareTo(version2) > 0);
        }

        [Test]
        public void GreaterThan_release()
        {
            var version = SemanticVersion.Parse("2.1.4-dev.122");
            var version2 = SemanticVersion.Parse("2.1.4-dev.21");

            Assert.IsTrue(version > version2);
        }

        [Test]
        public void LessThan_release()
        {
            var version = SemanticVersion.Parse("2.1.4-dev.122");
            var version2 = SemanticVersion.Parse("2.1.4-dev.21");

            Assert.IsTrue(version2 < version);
        }
    }
}
