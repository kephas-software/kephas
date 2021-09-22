// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultAppManagerCompositionTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the default application manager test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Reflection;
using Kephas.Application.Configuration;
using Kephas.Configuration;
using Kephas.Injection;
using Kephas.Injection.Lite.Hosting;
using Kephas.Logging;

namespace Kephas.Application.Tests
{
    using System;
    using System.Linq;

    using Kephas.Application;
    using Kephas.Behaviors;
    using Kephas.Services.Behaviors;
    using NUnit.Framework;

    [TestFixture]
    public class DefaultAppManagerCompositionTest : ApplicationTestBase
    {
        public override IInjector CreateInjector(
            IAmbientServices? ambientServices = null,
            IEnumerable<Assembly>? assemblies = null,
            IEnumerable<Type>? parts = null,
            Action<LiteInjectorBuilder>? config = null,
            ILogManager? logManager = null,
            IAppRuntime? appRuntime = null)
        {
            ambientServices ??= new AmbientServices();
            if (!ambientServices.IsRegistered(typeof(IAppContext)))
            {
                var lazyAppContext = new Lazy<IAppContext>(() => new Kephas.Application.AppContext(ambientServices));
                ambientServices.Register<IAppContext>(() => lazyAppContext.Value);
            }

            return base.CreateInjector(ambientServices, assemblies, parts, config, logManager, appRuntime);
        }

        [Test]
        public void Injection_compute_auto_feature_info()
        {
            var container = this.CreateInjector(parts: new[] { typeof(TestFeatureManager) });
            var appConfiguration = container.Resolve<IConfiguration<AppSettings>>();
            appConfiguration.GetSettings().EnabledFeatures = new[] { "test" };
            var appManager = (DefaultAppManager)container.Resolve<IAppManager>();

            var factoryMetadata = appManager.FeatureManagerFactories.Select(f => f.Metadata).ToList();
            Assert.AreEqual(1, factoryMetadata.Count);

            var testFeatureInfo = factoryMetadata[0].FeatureInfo;
            Assert.IsNotNull(testFeatureInfo);
            Assert.AreEqual("Test", testFeatureInfo.Name);
            Assert.AreEqual(new Version(0, 0, 0, 0), testFeatureInfo.Version);
        }

        [Test]
        public void Injection_not_required()
        {
            var container = this.CreateInjector(parts: new[] { typeof(TestFeatureManager) });
            var appManager = (DefaultAppManager)container.Resolve<IAppManager>();

            var factoryMetadata = appManager.FeatureManagerFactories.Select(f => f.Metadata).ToList();
            CollectionAssert.IsEmpty(factoryMetadata);
        }

        [Test]
        public void Injection_full_feature_info()
        {
            var container = this.CreateInjector(parts: new[] { typeof(AnnotatedTestFeatureManager) });
            var appConfiguration = container.Resolve<IConfiguration<AppSettings>>();
            appConfiguration.GetSettings().EnabledFeatures = new[] { "Annotated test" };
            var appManager = (DefaultAppManager)container.Resolve<IAppManager>();

            var factoryMetadata = appManager.FeatureManagerFactories.Select(f => f.Metadata).ToList();
            Assert.AreEqual(1, factoryMetadata.Count);

            var testFeatureInfo = factoryMetadata[0].FeatureInfo;
            Assert.IsNotNull(testFeatureInfo);
            Assert.AreEqual("Annotated test", testFeatureInfo.Name);
            Assert.AreEqual(new Version(1, 2, 3, 4), testFeatureInfo.Version);
            Assert.AreEqual(1, testFeatureInfo.Dependencies.Length);
            Assert.AreEqual("Test", testFeatureInfo.Dependencies[0]);
        }

        [Test]
        public void Injection_enabled_feature_info()
        {
            var container = this.CreateInjector(parts: new[] { typeof(AnnotatedTestFeatureManager), typeof(RequiredTestFeatureManager), typeof(RequiredFeatureManagerServiceBehavior) });
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

        [FeatureInfo("Annotated test", "1.2.3.4", dependencies: new[] { "Test" })]
        public class AnnotatedTestFeatureManager : FeatureManagerBase
        {
        }

        [FeatureInfo("Required", isRequired: true)]
        public class RequiredTestFeatureManager : FeatureManagerBase
        {
        }

        public class RequiredFeatureManagerServiceBehavior : EnabledServiceBehaviorRuleBase<IFeatureManager>
        {
            /// <summary>
            /// Gets the behavior value.
            /// </summary>
            /// <param name="context">The context.</param>
            /// <returns>
            /// The behavior value.
            /// </returns>
            public override IBehaviorValue<bool> GetValue(IServiceBehaviorContext<IFeatureManager> context)
            {
                return (context.Metadata as FeatureManagerMetadata)?.FeatureInfo.IsRequired.ToBehaviorValue() ?? BehaviorValue.False;
            }
        }
    }
}