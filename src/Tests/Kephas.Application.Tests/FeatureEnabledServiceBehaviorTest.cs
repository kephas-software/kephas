// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FeatureEnabledServiceBehaviorTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Injection;
using Kephas.Injection.ExportFactories;

namespace Kephas.Application.Tests
{
    using System.Collections.Generic;
    using Kephas.Application.Configuration;
    using Kephas.Application.Reflection;
    using Kephas.Configuration;
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
                new List<IExportFactory<IFeatureManager, FeatureManagerMetadata>> { exportFactory, enabledExportFactory });
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
                new List<IExportFactory<IFeatureManager, FeatureManagerMetadata>> { exportFactory });
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
                new List<IExportFactory<IFeatureManager, FeatureManagerMetadata>> { exportFactory });
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
                new List<IExportFactory<IFeatureManager, FeatureManagerMetadata>> { exportFactory });
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
                new List<IExportFactory<IFeatureManager, FeatureManagerMetadata>> { exportFactory });
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
                new List<IExportFactory<IFeatureManager, FeatureManagerMetadata>> { exportFactory });
            var value = behavior.GetValue(this.GetServiceBehaviorContext(exportFactory));
            Assert.IsFalse(value.Value);
        }

        private ServiceBehaviorContext<IFeatureManager> GetServiceBehaviorContext(IExportFactory<IFeatureManager, FeatureManagerMetadata> exportFactory)
        {
            var context = new ServiceBehaviorContext<IFeatureManager>(
                Substitute.For<IInjector>(),
                exportFactory,
                null);
            return context;
        }

        private IExportFactory<IFeatureManager, FeatureManagerMetadata> GetFeatureExportFactory(string name, bool isRequired = false, string[]? dependencies = null, string[]? targetApps = null)
        {
            var featureInfo = new FeatureInfo(name, "1.0", dependencies: dependencies, targetApps: targetApps, isRequired: isRequired);
            var exportFactory = new ExportFactory<IFeatureManager, FeatureManagerMetadata>(
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