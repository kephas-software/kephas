// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LicenseDataTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the license data test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests.Licensing
{
    using System;
    using System.Collections.Generic;

    using Kephas.Licensing;
    using NUnit.Framework;

    [TestFixture]
    public class LicenseDataTest
    {
        [Test]
        public void LicenseData_id_empty()
        {
            Assert.Throws<ArgumentNullException>(() => new LicenseData(null, "test", "1.0-2.0", "standard", "you", "me"));
        }

        [Test]
        public void ToString_no_custom_data()
        {
            var licenseData = new LicenseData("lic-id", "test", "1.0-2.0", "standard", "you", "me");
            var expectedString = "lic-id\ntest\n1.0-2.0\nstandard\nyou\nme\n\n\n\n1858223752";
            Assert.AreEqual(expectedString, licenseData.ToString());
        }

        [Test]
        public void ToString_custom_data()
        {
            var licenseData = new LicenseData(
                "lic-id",
                "test",
                "1.0-2.0",
                "standard",
                "you",
                "me",
                DateTime.Parse("2020-01-12"),
                DateTime.Parse("2021-01-11"),
                new Dictionary<string, string> {
                    { "SubscriptionId", "987654321" },
                    { "Description", "Good boy, may receive one year for free" },
                });
            var expectedString = "lic-id\ntest\n1.0-2.0\nstandard\nyou\nme\n2020-01-12\n2021-01-11\nSubscriptionId:987654321\nDescription:Good boy, may receive one year for free\n1905388808";
            Assert.AreEqual(expectedString, licenseData.ToString());
        }

        [Test]
        public void Parse_invalid_checksum()
        {
            var licenseString = "lic-id\ntest\n1.0-3.0\nstandard\nyou\nme\n\n\n\n1858223752";
            Assert.Throws<InvalidLicenseDataException>(() => LicenseData.Parse(licenseString));
        }

        [Test]
        public void Parse_no_custom_data()
        {
            var licenseString = "lic-id\ntest\n1.0-2.0\nstandard\nyou\nme\n\n\n\n1858223752";
            var licenseData = LicenseData.Parse(licenseString);
            Assert.AreEqual("lic-id", licenseData.Id);
            Assert.AreEqual("test", licenseData.AppId);
            Assert.AreEqual("1.0-2.0", licenseData.AppVersionRange);
            Assert.AreEqual("standard", licenseData.LicenseType);
            Assert.AreEqual("you", licenseData.LicensedTo);
            Assert.AreEqual("me", licenseData.LicensedBy);
        }

        [Test]
        public void Parse_custom_data()
        {
            var licenseString = "lic-id\ntest\n1.0-2.0\nstandard\nyou\nme\n2020-01-12\n2021-01-11\nSubscriptionId:987654321\nDescription:Good boy, may receive one year for free\n1905388808";
            var licenseData = LicenseData.Parse(licenseString);
            Assert.AreEqual("lic-id", licenseData.Id);
            Assert.AreEqual("test", licenseData.AppId);
            Assert.AreEqual("1.0-2.0", licenseData.AppVersionRange);
            Assert.AreEqual("standard", licenseData.LicenseType);
            Assert.AreEqual("you", licenseData.LicensedTo);
            Assert.AreEqual("me", licenseData.LicensedBy);
            Assert.AreEqual(DateTime.Parse("2020-01-12"), licenseData.ValidFrom);
            Assert.AreEqual(DateTime.Parse("2021-01-11"), licenseData.ValidTo);

            var data = licenseData.Data;
            Assert.AreEqual(2, data.Count);
            Assert.AreEqual("987654321", data["SubscriptionId"]);
            Assert.AreEqual("Good boy, may receive one year for free", data["Description"]);
        }
    }
}
