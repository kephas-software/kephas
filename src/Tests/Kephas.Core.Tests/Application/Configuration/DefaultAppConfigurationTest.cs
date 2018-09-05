// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultAppConfigurationTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the application settings configuration test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#if NETCOREAPP2_0
#else

namespace Kephas.Core.Tests.Application.Configuration
{
    using Kephas.Application.Configuration;
    using Kephas.Dynamic;

    using NUnit.Framework;

    [TestFixture]
    public class DefaultAppConfigurationTest
    {
        [Test]
        public void Indexer_setter()
        {
            var service = new DefaultAppConfiguration();
            var value = new object();
            service["hello"] = value;

            Assert.AreSame(value, service["hello"]);
        }

        [Test]
        public void GetSettings_from_appSettings()
        {
            var service = new DefaultAppConfiguration();
            var settings = service.GetSettings<Expando>(AppConfigurationSections.AppSettings);
            Assert.AreEqual("myValue", settings["MySetting"]);
            Assert.AreEqual("myOtherValue", settings["MyOtherSetting"]);
        }

        [Test]
        public void GetSettings_from_appSettings_read_write()
        {
            var service = new DefaultAppConfiguration();
            var settings = service.GetAppSettings();
            Assert.AreEqual("myValue", settings["MySetting"]);
            Assert.AreEqual("myOtherValue", settings["MyOtherSetting"]);

            settings["MySettings"] = "myNewValue";
            Assert.AreEqual("myNewValue", settings["MySettings"]);
        }

        [Test]
        public void GetSettings_typed_from_appSettings()
        {
            var service = new DefaultAppConfiguration();
            var settings = service.GetSettings<MySettings>(AppConfigurationSections.AppSettings);
            Assert.AreEqual("myValue", settings.MySetting);
            Assert.AreEqual("myOtherValue", settings.MyOtherSetting);
        }

        [Test]
        public void GetSettings_typed_from_section()
        {
            var service = new DefaultAppConfiguration();
            var settings = service.GetSettings<Expando>("my-section");
            Assert.AreEqual("v1", settings["name1"]);
        }

        [Test]
        public void GetSettings_not_found_then_empty()
        {
            var service = new DefaultAppConfiguration();
            var settings = service.GetSettings<Expando>("not-found");
            Assert.IsNull(settings);
        }

        [Test]
        public void Indexer_sets_runtime_settings()
        {
            var service = new DefaultAppConfiguration();
            var serverSettings = new Expando();
            service["Server"] = serverSettings;
            serverSettings["ServerUrl"] = "http://server.com";
            serverSettings["ServerPort"] = 101;
            var settings = service.GetSettings<RuntimeSettings>("Server");
            Assert.AreEqual("http://server.com", settings.ServerUrl);
            Assert.AreEqual(101, settings.ServerPort);
        }

        public class MySettings
        {
            public string MySetting { get; set; }

            public string MyOtherSetting { get; set; }
        }

        public class RuntimeSettings
        {
            public string ServerUrl { get; set; }

            public int ServerPort { get; set; }
        }
    }
}

#endif