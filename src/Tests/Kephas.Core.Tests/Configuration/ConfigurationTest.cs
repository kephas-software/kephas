// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigurationTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the configuration test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests.Configuration
{
    using System.Collections.Generic;

    using Kephas.Composition;
    using Kephas.Composition.ExportFactories;
    using Kephas.Configuration;
    using Kephas.Configuration.Composition;
    using Kephas.Configuration.Providers;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class ConfigurationTest
    {
        [Test]
        public void GetSettings_default_provider()
        {
            var settings = new Config1();
            var configProvider1 = Substitute.For<IConfigurationProvider>();
            configProvider1.GetSettings(typeof(Config1)).Returns(settings);

            var configuration = new Configuration<Config1>(new List<IExportFactory<IConfigurationProvider, ConfigurationProviderMetadata>>
                                                               {
                                                                   new ExportFactory<IConfigurationProvider, ConfigurationProviderMetadata>(() => configProvider1, new ConfigurationProviderMetadata(null))
                                                               });

            var result = configuration.Settings;
            Assert.AreSame(settings, result);
        }

        [Test]
        public void GetSettings_specific_provider()
        {
            var settings1 = new Config1();
            var configProvider1 = Substitute.For<IConfigurationProvider>();
            configProvider1.GetSettings(typeof(Config1)).Returns(settings1);

            var settings2 = new Config2();
            var configProvider2 = Substitute.For<IConfigurationProvider>();
            configProvider2.GetSettings(typeof(Config2)).Returns(settings2);

            var configuration = new Configuration<Config2>(new List<IExportFactory<IConfigurationProvider, ConfigurationProviderMetadata>>
                                                               {
                                                                   new ExportFactory<IConfigurationProvider, ConfigurationProviderMetadata>(() => configProvider1, new ConfigurationProviderMetadata(typeof(Config1))),
                                                                   new ExportFactory<IConfigurationProvider, ConfigurationProviderMetadata>(() => configProvider2, new ConfigurationProviderMetadata(typeof(Config2)))
                                                               });

            var result = configuration.Settings;
            Assert.AreSame(settings2, result);
        }

        public class Config1
        {
        }

        public class Config2
        {
        }
    }
}