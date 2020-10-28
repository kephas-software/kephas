// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FeatureEnabledServiceBehaviorTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application.Tests
{
    using System.Collections.Generic;

    using Kephas.Application.Composition;
    using Kephas.Application.Configuration;
    using Kephas.Application.Reflection;
    using Kephas.Composition;
    using Kephas.Composition.ExportFactories;
    using Kephas.Services.Behaviors;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class FeatureEnabledServiceBehaviorTest
    {
        [Test]
        public void GetValue_true_enabled_optional_dependency_case_insensitive()
        {
            var exportFactory = this.GetFeatureExportFactory("test");
            var behavior = new FeatureEnabledServiceBehavior(
                new StaticAppRuntime(appId: "test-app"),
                this.GetAppSettingsProvider(new[] { "Test" }),
                new List<IExportFactory<IFeatureManager, FeatureManagerMetadata>> { exportFactory });
            var value = behavior.GetValue(this.GetServiceBehaviorContext(exportFactory));
            Assert.IsTrue(value.Value);
        }

        [Test]
        public void GetValue_true_enabled_optional_dependency()
        {
            var exportFactory = this.GetFeatureExportFactory("test");
            var behavior = new FeatureEnabledServiceBehavior(
                new StaticAppRuntime(appId: "test-app"),
                this.GetAppSettingsProvider(new[] { "test" }),
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
                this.GetAppSettingsProvider(),
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
                this.GetAppSettingsProvider(),
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
                this.GetAppSettingsProvider(),
                new List<IExportFactory<IFeatureManager, FeatureManagerMetadata>> { exportFactory });
            var value = behavior.GetValue(this.GetServiceBehaviorContext(exportFactory));
            Assert.IsFalse(value.Value);
        }

        private ServiceBehaviorContext<IFeatureManager> GetServiceBehaviorContext(IExportFactory<IFeatureManager, FeatureManagerMetadata> exportFactory)
        {
            var context = new ServiceBehaviorContext<IFeatureManager>(
                Substitute.For<ICompositionContext>(),
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

        private IAppSettingsProvider GetAppSettingsProvider(string[]? enabledFeatures = null)
        {
            var appSettingsProvider = Substitute.For<IAppSettingsProvider>();
            appSettingsProvider.GetAppSettings().Returns(new AppSettings { EnabledFeatures = enabledFeatures });
            return appSettingsProvider;
        }
    }
}