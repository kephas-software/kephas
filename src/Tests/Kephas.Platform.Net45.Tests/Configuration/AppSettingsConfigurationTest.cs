// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppSettingsConfigurationTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the application settings configuration test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Platform.Net46.Tests.Configuration
{
    using Kephas.Configuration;
    using Kephas.Dynamic;

    using NUnit.Framework;

    [TestFixture]
    public class AppSettingsConfigurationTest
    {
        [Test]
        public void Indexer_setter()
        {
            var service = new AppSettingsConfiguration();
            var value = new object();
            service["hello"] = value;

            Assert.AreSame(value, service["hello"]);
        }

        [Test]
        public void GetSettings_with_pattern()
        {
            var service = new AppSettingsConfiguration();
            var settings = service.GetSettings<Expando>("My*");
            Assert.AreEqual("myValue", settings["MySetting"]);
            Assert.AreEqual("myOtherValue", settings["MyOtherSetting"]);
        }

        [Test]
        public void GetSettings_typed_with_pattern()
        {
            var service = new AppSettingsConfiguration();
            var settings = service.GetSettings<MySettings>("My*");
            Assert.AreEqual("myValue", settings.MySetting);
            Assert.AreEqual("myOtherValue", settings.MyOtherSetting);
        }

        [Test]
        public void GetSettings_typed_with_section_pattern()
        {
            var service = new AppSettingsConfiguration();
            var settings = service.GetSettings<Expando>(":my-section:*");
            Assert.AreEqual("v1", settings["name1"]);
        }

        public class MySettings
        {
            public string MySetting { get; set; }

            public string MyOtherSetting { get; set; }
        }
    }
}