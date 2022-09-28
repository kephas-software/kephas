// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultAppManagerInjectionTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Tests.Application
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Kephas.Application;
    using Kephas.Application.Configuration;
    using Kephas.Behaviors;
    using Kephas.Configuration;
    using Kephas.Injection.Builder;
    using Kephas.Logging;
    using Kephas.Services.Behaviors;
    using Kephas.Versioning;
    using NUnit.Framework;

    [TestFixture]
    public class DefaultAppManagerInjectionTest : ApplicationTestBase
    {
        public override IEnumerable<Assembly> GetAssemblies()
        {
            return new List<Assembly>(base.GetAssemblies())
            {
                typeof(IConfiguration<>).Assembly,      // Kephas.Configuration
            };
        }

        public override IServiceProvider BuildServiceProvider(
            IAmbientServices? ambientServices = null,
            IEnumerable<Assembly>? assemblies = null,
            IEnumerable<Type>? parts = null,
            Action<IInjectorBuilder>? config = null,
            ILogManager? logManager = null,
            IAppRuntime? appRuntime = null)
        {
            ambientServices ??= this.CreateAmbientServices();
            if (!ambientServices.Contains(typeof(IAppContext)))
            {
                var lazyAppContext = new Lazy<IAppContext>(() => new Kephas.Application.AppContext(ambientServices));
                ambientServices.Add<IAppContext>(() => lazyAppContext.Value);
            }

            return base.BuildServiceProvider(ambientServices, assemblies, parts, config, logManager, appRuntime);
        }

        [Test]
        public void Injection_compute_auto_feature_info()
        {
            var container = this.BuildServiceProvider(parts: new[] { typeof(TestFeatureManager) });
            var appConfiguration = container.Resolve<IConfiguration<AppSettings>>();
            appConfiguration.GetSettings().EnabledFeatures = new[] { "test" };
            var appManager = (DefaultAppManager)container.Resolve<IAppManager>();

            var factoryMetadata = appManager.FeatureManagerFactories.Select(f => f.Metadata).ToList();
            Assert.AreEqual(1, factoryMetadata.Count);

            var testFeatureInfo = factoryMetadata[0].FeatureInfo;
            Assert.IsNotNull(testFeatureInfo);
            Assert.AreEqual("Test", testFeatureInfo.Name);
            Assert.AreEqual(new SemanticVersion(0, 0, 0), testFeatureInfo.Version);
        }

        [Test]
        public void Injection_not_required()
        {
            var container = this.BuildServiceProvider(parts: new[] { typeof(TestFeatureManager) });
            var appManager = (DefaultAppManager)container.Resolve<IAppManager>();

            var factoryMetadata = appManager.FeatureManagerFactories.Select(f => f.Metadata).ToList();
            CollectionAssert.IsEmpty(factoryMetadata);
        }

        [Test]
        public void Injection_full_feature_info()
        {
            var container = this.BuildServiceProvider(parts: new[] { typeof(AnnotatedTestFeatureManager) });
            var appConfiguration = container.Resolve<IConfiguration<AppSettings>>();
            appConfiguration.GetSettings().EnabledFeatures = new[] { "Annotated test" };
            var appManager = (DefaultAppManager)container.Resolve<IAppManager>();

            var factoryMetadata = appManager.FeatureManagerFactories.Select(f => f.Metadata).ToList();
            Assert.AreEqual(1, factoryMetadata.Count);

            var testFeatureInfo = factoryMetadata[0].FeatureInfo;
            Assert.IsNotNull(testFeatureInfo);
            Assert.AreEqual("Annotated test", testFeatureInfo.Name);
            Assert.AreEqual(new SemanticVersion(1, 2, 3, "dev.2"), testFeatureInfo.Version);
            Assert.AreEqual(1, testFeatureInfo.Dependencies.Length);
            Assert.AreEqual("Test", testFeatureInfo.Dependencies[0]);
        }

        [Test]
        public void Injection_enabled_feature_info()
        {
            var container = this.BuildServiceProvider(parts: new[] { typeof(AnnotatedTestFeatureManager), typeof(RequiredTestFeatureManager), typeof(RequiredFeatureManagerServiceBehavior) });
            var appManager = (DefaultAppManager)container.Resolve<IAppManager>();

            var factoryMetadata = appManager.FeatureManagerFactories.Select(f => f.Metadata).ToList();
            Assert.AreEqual(1, factoryMetadata.Count);

            var testFeatureInfo = factoryMetadata[0].FeatureInfo;
            Assert.IsNotNull(testFeatureInfo);
            Assert.AreEqual("Required", testFeatureInfo.Name);
        }

        public class TestFeatureManager : FeatureManagerBase
        {
        }

        [FeatureInfo("Annotated test", "1.2.3-dev.2", dependencies: new[] { "Test" })]
        public class AnnotatedTestFeatureManager : FeatureManagerBase
        {
        }

        [FeatureInfo("Required", isRequired: true)]
        public class RequiredTestFeatureManager : FeatureManagerBase
        {
        }

        public class RequiredFeatureManagerServiceBehavior : EnabledServiceBehaviorRuleBase<IFeatureManager, FeatureManagerMetadata>
        {
            /// <summary>
            /// Gets the behavior value.
            /// </summary>
            /// <param name="context">The context.</param>
            /// <returns>
            /// The behavior value.
            /// </returns>
            public override IBehaviorValue<bool> GetValue(IServiceBehaviorContext<IFeatureManager, FeatureManagerMetadata> context)
            {
                return context.Metadata.FeatureInfo?.IsRequired.ToBehaviorValue() ?? BehaviorValue.False;
            }
        }
    }
}