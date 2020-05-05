// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VersionRangeTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests.Versioning
{
    using Kephas.Versioning;
    using NUnit.Framework;

    [TestFixture]
    public class VersionRangeTest
    {
        [Test]
        public void Parse_missing_max()
        {
            var range = VersionRange.Parse("2.*:");

            Assert.IsNull(range.MaxVersion);
            Assert.IsNotNull(range.MinVersion);
            Assert.AreEqual(range.MinVersion, SemanticVersion.Parse("2.0.0"));
        }

        [Test]
        public void Parse_missing_min()
        {
            var range = VersionRange.Parse(":2.*");

            Assert.IsNull(range.MinVersion);
            Assert.IsNotNull(range.MaxVersion);
            Assert.AreEqual(range.MaxVersion, SemanticVersion.Parse($"2.{short.MaxValue}.{short.MaxValue}"));
        }

        [Test]
        public void Parse_star_min()
        {
            var range = VersionRange.Parse("*:2.*");

            Assert.IsNotNull(range.MinVersion);
            Assert.IsNotNull(range.MaxVersion);
            Assert.AreEqual(range.MinVersion, SemanticVersion.Parse("0.0.0"));
            Assert.AreEqual(range.MaxVersion, SemanticVersion.Parse($"2.{short.MaxValue}.{short.MaxValue}"));
        }
    }
}