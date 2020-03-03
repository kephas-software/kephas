// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LicensingManagerTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the licensing manager base test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#nullable enable

namespace Kephas.Core.Tests.Licensing
{
    using System;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Licensing;
    using NUnit.Framework;

    [TestFixture]
    public class DefaultLicensingManagerTest
    {
        [Test]
        public async Task CheckLicenseAsync_identity_equal()
        {
            var licenseData = new LicenseData(
                Guid.NewGuid().ToString(),
                "my-app",
                "1.0",
                "standard",
                "you",
                "me");
            var licensingManager = new DefaultLicensingManager(_ => licenseData);
            var result = await licensingManager.CheckLicenseAsync(new AppIdentity("my-app", "1.0"));
            Assert.IsTrue(result.IsLicensed, "Should be licensed.");
            Assert.AreEqual(1, result.Messages.Count);
        }

        [Test]
        public void CheckLicense_identity_equal()
        {
            var licenseData = new LicenseData(
                Guid.NewGuid().ToString(),
                "my-app",
                "1.0",
                "standard",
                "you",
                "me");
            var licensingManager = new DefaultLicensingManager(_ => licenseData);
            var result = licensingManager.CheckLicense(new AppIdentity("my-app", "1.0"));
            Assert.IsTrue(result.IsLicensed, "Should be licensed.");
            Assert.AreEqual(1, result.Messages.Count);
        }

        [Test]
        public void CheckLicense_appid_pattern()
        {
            var licenseData = new LicenseData(
                Guid.NewGuid().ToString(),
                "my-ap*",
                "1.0",
                "standard",
                "you",
                "me");
            var licensingManager = new DefaultLicensingManager(_ => licenseData);
            var result = licensingManager.CheckLicense(new AppIdentity("my-app", "1.0"));
            Assert.IsTrue(result.IsLicensed, "Should be licensed.");
            Assert.AreEqual(1, result.Messages.Count);
        }

        [Test]
        public void CheckLicense_appid_pattern_mismatch()
        {
            var licenseData = new LicenseData(
                Guid.NewGuid().ToString(),
                "my-dear*",
                "1.0",
                "standard",
                "you",
                "me");
            var licensingManager = new DefaultLicensingManager(_ => licenseData);
            var result = licensingManager.CheckLicense(new AppIdentity("my-app", "1.0"));
            Assert.IsFalse(result.IsLicensed, "Should not be licensed.");
            Assert.AreEqual(1, result.Messages.Count);
        }

        [Test]
        public void CheckLicense_version_range()
        {
            var licenseData = new LicenseData(
                Guid.NewGuid().ToString(),
                "my-app",
                "0-1",
                "standard",
                "you",
                "me");
            var licensingManager = new DefaultLicensingManager(_ => licenseData);
            var result = licensingManager.CheckLicense(new AppIdentity("my-app", "1.5"));
            Assert.IsTrue(result.IsLicensed, "Should be licensed.");
            Assert.AreEqual(1, result.Messages.Count);
        }

        [Test]
        public void CheckLicense_version_range_out_of_range()
        {
            var licenseData = new LicenseData(
                Guid.NewGuid().ToString(),
                "my-app",
                "0-1.4",
                "standard",
                "you",
                "me");
            var licensingManager = new DefaultLicensingManager(_ => licenseData);
            var result = licensingManager.CheckLicense(new AppIdentity("my-app", "1.5"));
            Assert.IsFalse(result.IsLicensed, "Should not be licensed.");
            Assert.AreEqual(1, result.Messages.Count);
        }

        [Test]
        public void CheckLicense_version_range_mismatch_single_version()
        {
            var licenseData = new LicenseData(
                Guid.NewGuid().ToString(),
                "my-app",
                "0",
                "standard",
                "you",
                "me");
            var licensingManager = new DefaultLicensingManager(_ => licenseData);
            var result = licensingManager.CheckLicense(new AppIdentity("my-app", "1.5"));
            Assert.IsFalse(result.IsLicensed, "Should not be licensed.");
            Assert.AreEqual(1, result.Messages.Count);
        }

        [Test]
        public void CheckLicense_valid_period_open_end()
        {
            var licenseData = new LicenseData(
                Guid.NewGuid().ToString(),
                "my-app",
                "1.0",
                "standard",
                "you",
                "me",
                DateTime.Now.Date,
                null);
            var licensingManager = new DefaultLicensingManager(_ => licenseData);
            var result = licensingManager.CheckLicense(new AppIdentity("my-app", "1.0"));
            Assert.IsTrue(result.IsLicensed, "Should be licensed.");
            Assert.AreEqual(1, result.Messages.Count);
        }

        [Test]
        public void CheckLicense_invalid_period_open_end()
        {
            var licenseData = new LicenseData(
                Guid.NewGuid().ToString(),
                "my-app",
                "1.0",
                "standard",
                "you",
                "me",
                DateTime.Now.Date.AddDays(1),
                null);
            var licensingManager = new DefaultLicensingManager(_ => licenseData);
            var result = licensingManager.CheckLicense(new AppIdentity("my-app", "1.0"));
            Assert.IsFalse(result.IsLicensed, "Should not be licensed.");
            Assert.AreEqual(1, result.Messages.Count);
        }

        [Test]
        public void CheckLicense_valid_period_open_begin()
        {
            var licenseData = new LicenseData(
                Guid.NewGuid().ToString(),
                "my-app",
                "1.0",
                "standard",
                "you",
                "me",
                null,
                DateTime.Now.Date.AddDays(1));
            var licensingManager = new DefaultLicensingManager(_ => licenseData);
            var result = licensingManager.CheckLicense(new AppIdentity("my-app", "1.0"));
            Assert.IsTrue(result.IsLicensed, "Should be licensed.");
            Assert.AreEqual(1, result.Messages.Count);
        }

        [Test]
        public void CheckLicense_invalid_period_open_begin()
        {
            var licenseData = new LicenseData(
                Guid.NewGuid().ToString(),
                "my-app",
                "1.0",
                "standard",
                "you",
                "me",
                null,
                DateTime.Now.Date.AddDays(-1));
            var licensingManager = new DefaultLicensingManager(_ => licenseData);
            var result = licensingManager.CheckLicense(new AppIdentity("my-app", "1.0"));
            Assert.IsFalse(result.IsLicensed, "Should not be licensed.");
            Assert.AreEqual(1, result.Messages.Count);
        }

        [Test]
        public void CheckLicense_valid_period_all()
        {
            var licenseData = new LicenseData(
                Guid.NewGuid().ToString(),
                "my-app",
                "1.0",
                "standard",
                "you",
                "me",
                DateTime.Now.Date.AddDays(-1),
                DateTime.Now.Date.AddDays(1));
            var licensingManager = new DefaultLicensingManager(_ => licenseData);
            var result = licensingManager.CheckLicense(new AppIdentity("my-app", "1.0"));
            Assert.IsTrue(result.IsLicensed, "Should be licensed.");
            Assert.AreEqual(1, result.Messages.Count);
        }
    }
}
