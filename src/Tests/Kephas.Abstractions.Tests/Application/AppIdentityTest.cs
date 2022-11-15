// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppIdentityTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the application identity test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Tests.Application
{
    using Kephas.Application;
    using NUnit.Framework;

    [TestFixture]
    public class AppIdentityTest
    {
        [Test]
        public void IsMatch_different_names()
        {
            var appid1 = new AppIdentity("gigi");

            Assert.IsFalse(appid1.IsMatch(new AppIdentity("belogea")));
        }

        [Test]
        public void IsMatch_same_name_different_versions()
        {
            var appid1 = new AppIdentity("gigi", "1.0.0");

            Assert.IsFalse(appid1.IsMatch(new AppIdentity("gigi", "2.0.0")));
        }

        [Test]
        public void IsMatch_same_name_checker_version_null()
        {
            var appid1 = new AppIdentity("gigi");

            Assert.IsTrue(appid1.IsMatch(new AppIdentity("Gigi", "2.0.0")));
        }

        [Test]
        public void IsMatch_same_name_same_version()
        {
            var appid1 = new AppIdentity("gigi", "1.0-dev");

            Assert.IsTrue(appid1.IsMatch(new AppIdentity("Gigi", "1.0-DEV")));
        }
    }
}
