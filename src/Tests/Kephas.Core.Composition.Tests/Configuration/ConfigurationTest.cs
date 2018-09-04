// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigurationTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the configuration test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests.Configuration
{
    using System;

    using Kephas.Application.Configuration;
    using Kephas.Configuration;
    using Kephas.Configuration.Providers;
    using Kephas.Testing.Composition.Mef;

    using NUnit.Framework;

    [TestFixture]
    public class ConfigurationTest : CompositionTestBase
    {
        [Test]
        public void Composition_Configuration_specific_provider()
        {
            // specific provider
            var container = this.CreateContainer(parts: new[] { typeof(TestConfigurationProvider) });

            var config = container.GetExport<IConfiguration<TestSettings>>();
            Assert.AreSame(TestConfigurationProvider.Settings, config.Settings);
        }

        [Test]
        public void Composition_Configuration_default_provider()
        {
            var container = this.CreateContainer();

            var config = container.GetExport<IConfiguration<TestSettings>>();
            Assert.AreNotSame(TestConfigurationProvider.Settings, config.Settings);
        }

#if NETCOREAPP2_0
#else
        [Test]
        public void Composition_Configuration_app_config_provider()
        {
            var container = this.CreateContainer();
            var ambientServices = container.GetExport<IAmbientServices>();
            ambientServices.RegisterService<IAppConfiguration>(new DefaultAppConfiguration());

            var config = container.GetExport<IConfiguration<TestSettings>>();
            Assert.AreEqual("hello", config.Settings.Say);
        }
#endif

        public class TestSettings
        {
            public string Say { get; set; }
        }

        [SettingsType(typeof(TestSettings))]
        public class TestConfigurationProvider : IConfigurationProvider
        {
            public static TestSettings Settings = new TestSettings();

            /// <summary>
            /// Gets the settings with the provided type.
            /// </summary>
            /// <param name="settingsType">Type of the settings.</param>
            /// <returns>
            /// The settings.
            /// </returns>
            public object GetSettings(Type settingsType)
            {
                return Settings;
            }
        }
    }
}