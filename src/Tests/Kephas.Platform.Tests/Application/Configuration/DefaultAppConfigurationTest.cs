﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultAppConfigurationTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the application settings configuration test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Platform.Tests.Application.Configuration
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
            var settings = service.GetSettings<Expando>(AppSettingsSections.Default);
            Assert.AreEqual("myValue", settings["MySetting"]);
            Assert.AreEqual("myOtherValue", settings["MyOtherSetting"]);
        }

        [Test]
        public void GetSettings_typed_from_appSettings()
        {
            var service = new DefaultAppConfiguration();
            var settings = service.GetSettings<MySettings>(AppSettingsSections.Default);
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