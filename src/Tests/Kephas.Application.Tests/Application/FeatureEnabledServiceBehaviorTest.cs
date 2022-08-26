// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FeatureEnabledServiceBehaviorTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Tests.Application
{
    using System;
    using System.Collections.Generic;
    using Kephas.Application;
    using Kephas.Application.Configuration;
    using Kephas.Application.Reflection;
    using Kephas.Configuration;
    using Kephas.Injection;
    using Kephas.Services;
    using Kephas.Services.Behaviors;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class FeatureEnabledServiceBehaviorTest
    {
        [Test]
        public void GetValue_true_optional_dependency_of_enabled()
        {
            var enabledExportFactory = this.GetFeatureExportFactory("enabled", dependencies: new[] { "test" });
            var exportFactory = this.GetFeatureExportFactory("test");
            var behavior = new FeatureEnabledServiceBehavior(
                new StaticAppRuntime(appId: "test-app"),
                Substitute.For<IAppContext>(),
                this.GetAppConfiguration(new[] { "enabled" }),
                new List<Lazy<IFeatureManager, FeatureManagerMetadata>> { exportFactory, enabledExportFactory });
            var value = behavior.GetValue(this.GetServiceBehaviorContext(exportFactory));
            Assert.IsTrue(value.Value);
        }

        [Test]
        public void GetValue_true_enabled_optional_case_insensitive()
        {
            var exportFactory = this.GetFeatureExportFactory("test");
            var behavior = new FeatureEnabledServiceBehavior(
                new StaticAppRuntime(appId: "test-app"),
                Substitute.For<IAppContext>(),
                this.GetAppConfiguration(new[] { "Test" }),
                new List<Lazy<IFeatureManager, FeatureManagerMetadata>> { exportFactory });
            var value = behavior.GetValue(this.GetServiceBehaviorContext(exportFactory));
            Assert.IsTrue(value.Value);
        }

        [Test]
        public void GetValue_true_enabled_optional()
        {
            var exportFactory = this.GetFeatureExportFactory("test");
            var behavior = new FeatureEnabledServiceBehavior(
                new StaticAppRuntime(appId: "test-app"),
                Substitute.For<IAppContext>(),
                this.GetAppConfiguration(new[] { "test" }),
                new List<Lazy<IFeatureManager, FeatureManagerMetadata>> { exportFactory });
            var value = behavior.GetValue(this.GetServiceBehaviorContext(exportFactory));
            Assert.IsTrue(value.Value);
        }

        [Test]
        public void GetValue_true_optional_matching_target_app()
        {
            var exportFactory = this.GetFeatureExportFactory("test", targetApps: new[] { "test-app" });
            var behavior = new FeatureEnabledServiceBehavior(
                new StaticAppRuntime(appId: "test-app"),
                Substitute.For<IAppContext>(),
                this.GetAppConfiguration(),
                new List<Lazy<IFeatureManager, FeatureManagerMetadata>> { exportFactory });
            var value = behavior.GetValue(this.GetServiceBehaviorContext(exportFactory));
            Assert.IsTrue(value.Value);
        }

        [Test]
        public void GetValue_false_optional_non_matching_target_app()
        {
            var exportFactory = this.GetFeatureExportFactory("test", targetApps: new[] { "test-app" });
            var behavior = new FeatureEnabledServiceBehavior(
                new StaticAppRuntime(appId: "non-test-app"),
                Substitute.For<IAppContext>(),
                this.GetAppConfiguration(),
                new List<Lazy<IFeatureManager, FeatureManagerMetadata>> { exportFactory });
            var value = behavior.GetValue(this.GetServiceBehaviorContext(exportFactory));
            Assert.IsFalse(value.Value);
        }

        [Test]
        public void GetValue_false_optional_no_target_app()
        {
            var exportFactory = this.GetFeatureExportFactory("test");
            var behavior = new FeatureEnabledServiceBehavior(
                new StaticAppRuntime(appId: "test-app"),
                Substitute.For<IAppContext>(),
                this.GetAppConfiguration(),
                new List<Lazy<IFeatureManager, FeatureManagerMetadata>> { exportFactory });
            var value = behavior.GetValue(this.GetServiceBehaviorContext(exportFactory));
            Assert.IsFalse(value.Value);
        }

        private ServiceBehaviorContext<IFeatureManager, FeatureManagerMetadata> GetServiceBehaviorContext(Lazy<IFeatureManager, FeatureManagerMetadata> exportFactory)
        {
            var context = new ServiceBehaviorContext<IFeatureManager, FeatureManagerMetadata>(
                Substitute.For<IInjector>(),
                () => exportFactory.Value,
                exportFactory.Metadata);
            return context;
        }

        private Lazy<IFeatureManager, FeatureManagerMetadata> GetFeatureExportFactory(string name, bool isRequired = false, string[]? dependencies = null, string[]? targetApps = null)
        {
            var featureInfo = new FeatureInfo(name, "1.0", dependencies: dependencies, targetApps: targetApps, isRequired: isRequired);
            var exportFactory = new Lazy<IFeatureManager, FeatureManagerMetadata>(
                () => Substitute.For<IFeatureManager>(),
                new FeatureManagerMetadata(featureInfo));
            return exportFactory;
        }

        private IConfiguration<AppSettings> GetAppConfiguration(string[]? enabledFeatures = null)
        {
            var appSettingsProvider = Substitute.For<IConfiguration<AppSettings>>();
            appSettingsProvider.GetSettings(Arg.Any<IContext?>()).Returns(new AppSettings { EnabledFeatures = enabledFeatures });
            return appSettingsProvider;
        }
    }
}