// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultAppManagerTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the default application manager test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application.Tests
{
    using System;
    using System.Linq;

    using Kephas.Application;
    using Kephas.Application.Composition;
    using Kephas.Behaviors;
    using Kephas.Services.Behaviors;
    using Kephas.Testing.Composition;

    using NUnit.Framework;

    [TestFixture]
    public class DefaultAppManagerCompositionTest : ApplicationTestBase
    {
        [Test]
        public void Composition_compute_auto_feature_info()
        {
            var container = CreateContainer(parts: new[] { typeof(TestFeatureManager) });
            var appManager = (DefaultAppManager)container.GetExport<IAppManager>();

            var factoryMetadata = appManager.FeatureManagerFactories.Select(f => f.Metadata).ToList();
            Assert.AreEqual(1, factoryMetadata.Count);

            var testFeatureInfo = factoryMetadata[0].FeatureInfo;
            Assert.IsNotNull(testFeatureInfo);
            Assert.AreEqual("Test", testFeatureInfo.Name);
            Assert.AreEqual(new Version(0, 0, 0, 0), testFeatureInfo.Version);
        }

        [Test]
        public void Composition_full_feature_info()
        {
            var container = CreateContainer(parts: new[] { typeof(AnnotatedTestFeatureManager) });
            var appManager = (DefaultAppManager)container.GetExport<IAppManager>();

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
        public void Composition_enabled_feature_info()
        {
            var container = CreateContainer(parts: new[] { typeof(AnnotatedTestFeatureManager), typeof(RequiredTestFeatureManager), typeof(RequiredFeatureManagerServiceBehavior) });
            var appManager = (DefaultAppManager)container.GetExport<IAppManager>();

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