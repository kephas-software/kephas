// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigurationTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the configuration test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Tests.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Configuration;
    using Kephas.Configuration.Interaction;
    using Kephas.Configuration.Providers;
    using Kephas.Injection;
    using Kephas.Interaction;
    using Kephas.Serialization;
    using Kephas.Services;
    using Kephas.Testing.Injection;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class ConfigurationTest : InjectionTestBase
    {
        public override IEnumerable<Assembly> GetAssemblies()
        {
            return new List<Assembly>(base.GetAssemblies())
            {
                typeof(IEventHub).Assembly,         // Kephas.Interaction
                typeof(IInjectableFactory).Assembly,   // Kephas.Injection
                typeof(IConfiguration<>).Assembly,  // Kephas.Configuration
                typeof(ISerializationService).Assembly,  // Kephas.Serialization
            };
        }

        [Test]
        public void GetSettings_default_provider()
        {
            var context = Substitute.For<IContext>();

            var settings = new Config1();
            var configProvider1 = Substitute.For<ISettingsProvider>();
            configProvider1.GetSettings(typeof(Config1), context).Returns(settings);

            var selector = new DefaultSettingsProviderSelector(new List<IExportFactory<ISettingsProvider, SettingsProviderMetadata>>
                                                               {
                                                                   new ExportFactory<ISettingsProvider, SettingsProviderMetadata>(() => configProvider1, new SettingsProviderMetadata(null)),
                                                               });
            var configuration = new Configuration<Config1>(selector, Substitute.For<IAppRuntime>(), new Lazy<IEventHub>(() => Substitute.For<IEventHub>()));

            var result = configuration.GetSettings(context);
            Assert.AreSame(settings, result);
        }

        [Test]
        public void GetSettings_specific_provider()
        {
            var context = Substitute.For<IContext>();

            var settings1 = new Config1();
            var configProvider1 = Substitute.For<ISettingsProvider>();
            configProvider1.GetSettings(typeof(Config1), context).Returns(settings1);

            var settings2 = new Config2();
            var configProvider2 = Substitute.For<ISettingsProvider>();
            configProvider2.GetSettings(typeof(Config2), context).Returns(settings2);

            var selector = new DefaultSettingsProviderSelector(new List<IExportFactory<ISettingsProvider, SettingsProviderMetadata>>
                                                               {
                                                                   new ExportFactory<ISettingsProvider, SettingsProviderMetadata>(() => configProvider1, new SettingsProviderMetadata(typeof(Config1))),
                                                                   new ExportFactory<ISettingsProvider, SettingsProviderMetadata>(() => configProvider2, new SettingsProviderMetadata(typeof(Config2))),
                                                               });
            var configuration = new Configuration<Config2>(selector, Substitute.For<IAppRuntime>(), new Lazy<IEventHub>(() => Substitute.For<IEventHub>()));

            var result = configuration.GetSettings(context);
            Assert.AreSame(settings2, result);
        }

        [Test]
        public void Injection_Configuration_specific_provider()
        {
            // specific provider
            var container = this.CreateInjector(parts: new[] { typeof(TestConfigurationProvider) });

            var config = container.Resolve<IConfiguration<TestSettings>>();
            Assert.AreSame(TestConfigurationProvider.Settings, config.GetSettings());
        }

        [Test]
        public async Task Injection_Configuration_change_signal_skipped_when_not_changed()
        {
            // specific provider
            var container = this.CreateInjector(parts: new[] { typeof(TestConfigurationProvider) });
            var eventHub = container.Resolve<IEventHub>();
            var appRuntime = container.Resolve<IAppRuntime>();
            var configChanged = 0;
            using var subscription = eventHub.Subscribe<ConfigurationChangedSignal>(
                (s, ctx) =>
                    configChanged += s.SourceAppInstanceId == appRuntime.GetAppInstanceId()
                                     && s.SettingsType == typeof(TestSettings).FullName ? 1 : 0);

            var config = container.Resolve<IConfiguration<TestSettings>>();
            await config.UpdateSettingsAsync();

            Assert.AreEqual(0, configChanged);
        }

        [Test]
        public async Task Injection_Configuration_change_signal_when_changed()
        {
            // specific provider
            var container = this.CreateInjector(parts: new[] { typeof(TestConfigurationProvider) });
            var eventHub = container.Resolve<IEventHub>();
            var appRuntime = container.Resolve<IAppRuntime>();
            var configChanged = 0;
            using var subscription = eventHub.Subscribe<ConfigurationChangedSignal>(
                (s, ctx) =>
                    configChanged += s.SourceAppInstanceId == appRuntime.GetAppInstanceId()
                                     && s.SettingsType == typeof(TestSettings).FullName ? 1 : 0);

            var config = container.Resolve<IConfiguration<TestSettings>>();
            await config.UpdateSettingsAsync(new TestSettings());

            Assert.AreEqual(1, configChanged);
        }

        public class TestSettings
        {
            public string Say { get; set; }
        }

        [SettingsType(typeof(TestSettings))]
        public class TestConfigurationProvider : ISettingsProvider
        {
            public static TestSettings Settings = new TestSettings();

            public object? GetSettings(Type settingsType, IContext? context)
            {
                return Settings;
            }

            public async Task UpdateSettingsAsync(object settings, IContext? context, CancellationToken cancellationToken = default)
            {
                await Task.Yield();
            }
        }

        public class Config1
        {
        }

        public class Config2
        {
        }
    }
}