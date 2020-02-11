// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultAppManagerTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the default application appManager test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Application.Composition;
    using Kephas.Application.Reflection;
    using Kephas.Composition;
    using Kephas.Composition.ExportFactories;
    using Kephas.Composition.ExportFactoryImporters;
    using Kephas.Logging;
    using Kephas.Services;
    using Kephas.Services.Behaviors;
    using Kephas.Services.Behaviors.Composition;
    using Kephas.Services.Composition;

    using NSubstitute;
    using NSubstitute.Core;

    using NUnit.Framework;

    [TestFixture]
    public class DefaultAppManagerTest
    {
        [Test]
        public void Constructor_order_dependencies_with_processing_priority_with_cyclic_dependencies()
        {
            var featureManager1 = CreateFeatureManager();
            var featureManager2 = CreateFeatureManager();
            var featureManager3 = CreateFeatureManager();

            Assert.Throws<InvalidOperationException>(() =>
                new DefaultAppManager(
                    Substitute.For<IAppRuntime>(),
                    GetCompositionContext(),
                    GetServiceBehaviorProvider(),
                    new List<IExportFactory<IAppLifecycleBehavior, AppServiceMetadata>>(),
                    new[]
                        {
                            CreateFeatureManagerFactory(featureManager1, "1", "1.0", dependencies: new[] { "3" }),
                            CreateFeatureManagerFactory(featureManager2, "2", "1.0", dependencies: new[] { "1" }),
                            // make the manager 3 with a lower priority than 1 and 2 which actually depend on it
                            CreateFeatureManagerFactory(featureManager3, "3", "1.0", processingPriority: Priority.Low),
                        },
                    null));
        }

        [Test]
        public async Task InitializeAppAsync_right_app_behavior_order()
        {
            var order = new List<int>();

            var featureManager1 = Substitute.For<IFeatureManager>();
            featureManager1.InitializeAsync(Arg.Any<IAppContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(0))
                .AndDoes(_ => order.Add(1));

            var featureManager2 = Substitute.For<IFeatureManager>();
            featureManager2.InitializeAsync(Arg.Any<IAppContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(0))
                .AndDoes(_ => order.Add(2));

            var behavior1 = Substitute.For<IAppLifecycleBehavior>();
            behavior1.BeforeAppInitializeAsync(Arg.Any<IAppContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(0))
                .AndDoes(_ => order.Add(11));
            behavior1.AfterAppInitializeAsync(Arg.Any<IAppContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(0))
                .AndDoes(_ => order.Add(21));

            var behavior2 = Substitute.For<IAppLifecycleBehavior>();
            behavior2.BeforeAppInitializeAsync(Arg.Any<IAppContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(0))
                .AndDoes(_ => order.Add(12));
            behavior2.AfterAppInitializeAsync(Arg.Any<IAppContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(0))
                .AndDoes(_ => order.Add(22));

            var appManager = new DefaultAppManager(
                Substitute.For<IAppRuntime>(),
                GetCompositionContext(),
                GetServiceBehaviorProvider(),
                new[]
                    {
                        new ExportFactory<IAppLifecycleBehavior, AppServiceMetadata>(() => behavior1, new FeatureManagerMetadata(processingPriority: 2)),
                        new ExportFactory<IAppLifecycleBehavior, AppServiceMetadata>(() => behavior2, new FeatureManagerMetadata(processingPriority: 1)),
                    },
                new[]
                    {
                        new ExportFactory<IFeatureManager, FeatureManagerMetadata>(() => featureManager1, new FeatureManagerMetadata(processingPriority: 2)),
                        new ExportFactory<IFeatureManager, FeatureManagerMetadata>(() => featureManager2, new FeatureManagerMetadata(processingPriority: 1)),
                    },
                null);

            await appManager.InitializeAppAsync(Substitute.For<IAppContext>(), CancellationToken.None);

            Assert.AreEqual(6, order.Count);
            Assert.AreEqual(12, order[0]);
            Assert.AreEqual(11, order[1]);
            Assert.AreEqual(2, order[2]);
            Assert.AreEqual(1, order[3]);
            Assert.AreEqual(22, order[4]);
            Assert.AreEqual(21, order[5]);
        }

        [Test]
        public async Task InitializeAppAsync_right_featureManager_order_processingPriority()
        {
            var order = new List<int>();

            var featureManager1 = Substitute.For<IFeatureManager>();
            featureManager1.InitializeAsync(Arg.Any<IAppContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(0))
                .AndDoes(_ => order.Add(1));

            var featureManager2 = Substitute.For<IFeatureManager>();
            featureManager2.InitializeAsync(Arg.Any<IAppContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(0))
                .AndDoes(_ => order.Add(2));

            var appManager = new DefaultAppManager(
                Substitute.For<IAppRuntime>(),
                GetCompositionContext(),
                GetServiceBehaviorProvider(),
                new List<IExportFactory<IAppLifecycleBehavior, AppServiceMetadata>>(),
                new[]
                    {
                        new ExportFactory<IFeatureManager, FeatureManagerMetadata>(() => featureManager1, new FeatureManagerMetadata(processingPriority: 2)),
                        new ExportFactory<IFeatureManager, FeatureManagerMetadata>(() => featureManager2, new FeatureManagerMetadata(processingPriority: 1)),
                    },
                null);

            await appManager.InitializeAppAsync(Substitute.For<IAppContext>(), CancellationToken.None);

            Assert.AreEqual(2, order.Count);
            Assert.AreEqual(2, order[0]);
            Assert.AreEqual(1, order[1]);
        }

        [Test]
        public async Task InitializeAppAsync_right_featureManager_order_processingPriority_marginal_values()
        {
            var order = new List<int>();

            var featureManager1 = Substitute.For<IFeatureManager>();
            featureManager1.InitializeAsync(Arg.Any<IAppContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(0))
                .AndDoes(_ => order.Add(1));

            var featureManager2 = Substitute.For<IFeatureManager>();
            featureManager2.InitializeAsync(Arg.Any<IAppContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(0))
                .AndDoes(_ => order.Add(2));

            var featureManager3 = Substitute.For<IFeatureManager>();
            featureManager3.InitializeAsync(Arg.Any<IAppContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(0))
                .AndDoes(_ => order.Add(3));

            var appManager = new DefaultAppManager(
                Substitute.For<IAppRuntime>(),
                GetCompositionContext(),
                GetServiceBehaviorProvider(),
                new List<IExportFactory<IAppLifecycleBehavior, AppServiceMetadata>>(),
                new[]
                    {
                        new ExportFactory<IFeatureManager, FeatureManagerMetadata>(() => featureManager1, new FeatureManagerMetadata(processingPriority: 0)),
                        new ExportFactory<IFeatureManager, FeatureManagerMetadata>(() => featureManager2, new FeatureManagerMetadata(processingPriority: int.MaxValue)),
                        new ExportFactory<IFeatureManager, FeatureManagerMetadata>(() => featureManager3, new FeatureManagerMetadata(processingPriority: int.MinValue)),
                    },
                null);

            await appManager.InitializeAppAsync(Substitute.For<IAppContext>(), CancellationToken.None);

            Assert.AreEqual(3, order.Count);
            Assert.AreEqual(3, order[0]);
            Assert.AreEqual(1, order[1]);
            Assert.AreEqual(2, order[2]);
        }

        [Test]
        public async Task InitializeAppAsync_right_featureManager_order_dependencies()
        {
            var order = new List<int>();

            var featureManager1 = CreateFeatureManager(initialization: _ => order.Add(1));
            var featureManager2 = CreateFeatureManager(initialization: _ => order.Add(2));
            var featureManager3 = CreateFeatureManager(initialization: _ => order.Add(3));

            var appManager = new DefaultAppManager(
                Substitute.For<IAppRuntime>(),
                GetCompositionContext(),
                GetServiceBehaviorProvider(),
                new List<IExportFactory<IAppLifecycleBehavior, AppServiceMetadata>>(),
                new[]
                    {
                        CreateFeatureManagerFactory(featureManager1, "1", "1.0", dependencies: new[] { "3" }),
                        CreateFeatureManagerFactory(featureManager2, "2", "1.0", dependencies: new[] { "3" }),
                        CreateFeatureManagerFactory(featureManager3, "3", "1.0"),
                    },
                null);

            await appManager.InitializeAppAsync(Substitute.For<IAppContext>(), CancellationToken.None);

            Assert.AreEqual(3, order.Count);
            Assert.AreEqual(3, order[0]);
            Assert.AreEqual(1, order[1]);
            Assert.AreEqual(2, order[2]);
        }

        [Test]
        public async Task InitializeAppAsync_features_set_in_app_manifest()
        {
            var featureManager1 = CreateFeatureManager();
            var featureManager2 = CreateFeatureManager();
            var featureManager3 = CreateFeatureManager();

            var factory1 = CreateFeatureManagerFactory(featureManager1, "1", "1.0", dependencies: new[] { "3" }, isRequired: true);
            var factory2 = CreateFeatureManagerFactory(featureManager2, "2", "1.0", dependencies: new[] { "3" }, isRequired: false);
            var factory3 = CreateFeatureManagerFactory(featureManager3, "3", "1.0", isRequired: true);

            IEnumerable<IFeatureInfo> features = new List<IFeatureInfo>();
            var appRuntime = Substitute.For<IAppRuntime>();
            appRuntime.When(a => a[ApplicationExtensions.FeaturesKey] = Arg.Any<object>())
                .Do(ci => features = (IEnumerable<IFeatureInfo>)ci.Arg<object>());
            appRuntime[ApplicationExtensions.FeaturesKey].Returns(features);
            var appManager = new DefaultAppManager(
                appRuntime,
                GetCompositionContext(),
                GetServiceBehaviorProvider(),
                new List<IExportFactory<IAppLifecycleBehavior, AppServiceMetadata>>(),
                new[] { factory1, factory2, factory3 },
                null);

            await appManager.InitializeAppAsync(Substitute.For<IAppContext>(), CancellationToken.None);

            Assert.AreEqual(3, appRuntime.GetFeatures().Count());

            var featureList = appRuntime.GetFeatures().ToList();
            Assert.AreEqual("3", featureList[0].Name);
            Assert.AreEqual("1", featureList[1].Name);
            Assert.AreEqual("2", featureList[2].Name);
        }

        [Test]
        public async Task InitializeAppAsync_ok_failed_optional_feature()
        {
            var order = new List<int>();

            var featureManager1 = CreateFeatureManager(initialization: _ => order.Add(1));
            var featureManager2 = CreateFeatureManager(initialization: _ => throw new InvalidOperationException("bad guy"));
            var featureManager3 = CreateFeatureManager(initialization: _ => order.Add(3));

            var factory1 = CreateFeatureManagerFactory(featureManager1, "1", "1.0", dependencies: new[] { "3" }, isRequired: true);
            var factory2 = CreateFeatureManagerFactory(featureManager2, "2", "1.0", dependencies: new[] { "3" }, isRequired: false);
            var factory3 = CreateFeatureManagerFactory(featureManager3, "3", "1.0", isRequired: true);

            var sb = new StringBuilder();
            var logger = Substitute.For<ILogger<IAppManager>>();
            logger
                .WhenForAnyArgs(l => l.Log(LogLevel.Debug, null, null, null))
                .Do(ci => sb.AppendLine($"{ci.Arg<object[]>()?.FirstOrDefault()}-{ci.Arg<LogLevel>()}: {ci.Arg<string>()} ({ci.Arg<Exception>()?.GetType().Name})"));

            var appManager = new DefaultAppManager(
                Substitute.For<IAppRuntime>(),
                GetCompositionContext(),
                GetServiceBehaviorProvider(),
                new List<IExportFactory<IAppLifecycleBehavior, AppServiceMetadata>>(),
                new[] { factory1, factory2, factory3 },
                null);

            appManager.Logger = logger;
            await appManager.InitializeAppAsync(Substitute.For<IAppContext>(), CancellationToken.None);

            Assert.AreEqual(2, order.Count);
            Assert.AreEqual(3, order[0]);
            Assert.AreEqual(1, order[1]);

            var log = sb.ToString();
            if (!log.Contains("Error: {feature} ({featureKind}) failed to initialize. See the inner exception for more details. (InvalidOperationException)"))
            {
                Assert.Warn($"The log does not contain 'Error: {{feature}} ({{featureKind}}) failed to initialize. See the inner exception for more details. (InvalidOperationException)'. Log: {log}.");
            }

            Assert.IsTrue(log.Contains("(InvalidOperationException)"));
        }

        [Test]
        public async Task InitializeAppAsync_exception_failed_required_feature()
        {
            var order = new List<int>();

            var featureManager1 = CreateFeatureManager(initialization: ci => throw new InvalidOperationException());

            var factory1 = CreateFeatureManagerFactory(featureManager1, "1", "1.0", isRequired: true);

            var sb = new StringBuilder();
            var logger = Substitute.For<ILogger<IAppManager>>();
            logger
                .WhenForAnyArgs(l => l.Log(LogLevel.Debug, null, null, null))
                .Do(ci => sb.AppendLine($"{ci.Arg<object[]>()?.FirstOrDefault()}-{ci.Arg<LogLevel>()}: {ci.Arg<string>()} ({ci.Arg<Exception>()?.GetType().Name})"));

            var appManager = new DefaultAppManager(
                Substitute.For<IAppRuntime>(),
                GetCompositionContext(),
                GetServiceBehaviorProvider(),
                new List<IExportFactory<IAppLifecycleBehavior, AppServiceMetadata>>(),
                new[] { factory1 },
                null);

            appManager.Logger = logger;
            Assert.That(() => appManager.InitializeAppAsync(Substitute.For<IAppContext>(), CancellationToken.None), Throws.InvalidOperationException);

            var log = sb.ToString();
            if (!log.Contains("Error: {feature} ({featureKind}) failed to initialize. See the inner exception for more details. (InvalidOperationException)"))
            {
                Assert.Warn($"The log does not contain 'Error: {{feature}} ({{featureKind}}) failed to initialize. See the inner exception for more details. (InvalidOperationException)'. Log: {log}.");
            }

            Assert.IsTrue(log.Contains("(InvalidOperationException)"));
            Assert.IsTrue(log.Contains("Error: The application's initialize procedure encountered an exception"));
        }

        [Test]
        public async Task InitializeAppAsync_right_behavior_order()
        {
            var order = new List<string>();

            var featureManager1 = Substitute.For<IFeatureManager>();
            featureManager1.InitializeAsync(Arg.Any<IAppContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(0));

            var featureManager2 = Substitute.For<IFeatureManager>();
            featureManager2.InitializeAsync(Arg.Any<IAppContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(0));

            var behavior1 = CreateFeatureLifecycleBehavior(
                _ => order.Add("Before1"),
                _ => order.Add("After1"));

            var behavior2 = CreateFeatureLifecycleBehavior(
                _ => order.Add("Before2"),
                _ => order.Add("After2"));

            var appManager = new DefaultAppManager(
                Substitute.For<IAppRuntime>(),
                GetCompositionContext(),
                GetServiceBehaviorProvider(),
                new List<IExportFactory<IAppLifecycleBehavior, AppServiceMetadata>>(),
                new[]
                    {
                        new ExportFactory<IFeatureManager, FeatureManagerMetadata>(() => featureManager1, new FeatureManagerMetadata()),
                        new ExportFactory<IFeatureManager, FeatureManagerMetadata>(() => featureManager2, new FeatureManagerMetadata()),
                    },
                new[]
                    {
                        new ExportFactory<IFeatureLifecycleBehavior, FeatureLifecycleBehaviorMetadata>(() => behavior1, new FeatureLifecycleBehaviorMetadata(processingPriority: 2)),
                        new ExportFactory<IFeatureLifecycleBehavior, FeatureLifecycleBehaviorMetadata>(() => behavior2, new FeatureLifecycleBehaviorMetadata(processingPriority: 1)),
                    });

            await appManager.InitializeAppAsync(Substitute.For<IAppContext>(), CancellationToken.None);

            Assert.AreEqual(8, order.Count);
            Assert.AreEqual("Before2", order[0]);
            Assert.AreEqual("Before1", order[1]);
            Assert.AreEqual("After1", order[2]);
            Assert.AreEqual("After2", order[3]);
            Assert.AreEqual("Before2", order[4]);
            Assert.AreEqual("Before1", order[5]);
            Assert.AreEqual("After1", order[6]);
            Assert.AreEqual("After2", order[7]);
        }

        [Test]
        public async Task InitializeAppAsync_right_feature_behavior_order_with_filter()
        {
            var order = new List<int>();

            var finalizer1 = CreateFeatureManager(initialization: _ => order.Add(1));
            var finalizer2 = CreateFeatureManager(initialization: _ => order.Add(2));
            var finalizer3 = CreateFeatureManager(initialization: _ => order.Add(3));

            var behaviors = new List<string>();

            var behavior1 = CreateFeatureLifecycleBehavior(
                    ci => behaviors.Add("BeforeInit 1"),
                    ci => behaviors.Add("AfterInit 1"));
            var behavior2 = CreateFeatureLifecycleBehavior(
                    ci => behaviors.Add("BeforeInit 2"),
                    ci => behaviors.Add("AfterInit 2"));
            var behavior3 = CreateFeatureLifecycleBehavior(
                    ci => behaviors.Add("BeforeInit 3"),
                    ci => behaviors.Add("AfterInit 3"));

            var appManager = new DefaultAppManager(
                Substitute.For<IAppRuntime>(),
                GetCompositionContext(),
                GetServiceBehaviorProvider(),
                new List<IExportFactory<IAppLifecycleBehavior, AppServiceMetadata>>(),
                new[]
                    {
                        new ExportFactory<IFeatureManager, FeatureManagerMetadata>(() => finalizer1, new FeatureManagerMetadata(new FeatureInfo("1", "1.0", dependencies: new[] { "3" }))),
                        new ExportFactory<IFeatureManager, FeatureManagerMetadata>(() => finalizer2, new FeatureManagerMetadata(new FeatureInfo("2", "1.0", dependencies: new[] { "1" }))),
                        new ExportFactory<IFeatureManager, FeatureManagerMetadata>(() => finalizer3, new FeatureManagerMetadata(new FeatureInfo("3", "1.0"))),
                    },
                new[]
                    {
                        new ExportFactory<IFeatureLifecycleBehavior, FeatureLifecycleBehaviorMetadata>(() => behavior1, new FeatureLifecycleBehaviorMetadata("1")),
                        new ExportFactory<IFeatureLifecycleBehavior, FeatureLifecycleBehaviorMetadata>(() => behavior2, new FeatureLifecycleBehaviorMetadata("2")),
                        new ExportFactory<IFeatureLifecycleBehavior, FeatureLifecycleBehaviorMetadata>(() => behavior3, new FeatureLifecycleBehaviorMetadata()),
                    });

            await appManager.InitializeAppAsync(Substitute.For<IAppContext>(), CancellationToken.None);

            Assert.AreEqual(3, order.Count);
            Assert.AreEqual(3, order[0]);
            Assert.AreEqual(1, order[1]);
            Assert.AreEqual(2, order[2]);

            Assert.AreEqual(10, behaviors.Count);
            Assert.AreEqual("BeforeInit 3", behaviors[0]);
            Assert.AreEqual("AfterInit 3", behaviors[1]);
            Assert.AreEqual("BeforeInit 1", behaviors[2]);
            Assert.AreEqual("BeforeInit 3", behaviors[3]);
            Assert.AreEqual("AfterInit 1", behaviors[4]);
            Assert.AreEqual("AfterInit 3", behaviors[5]);
            Assert.AreEqual("BeforeInit 2", behaviors[6]);
            Assert.AreEqual("BeforeInit 3", behaviors[7]);
            Assert.AreEqual("AfterInit 2", behaviors[8]);
            Assert.AreEqual("AfterInit 3", behaviors[9]);
        }

        [Test]
        public async Task FinalizeAppAsync_right_app_behavior_order()
        {
            var order = new List<int>();

            var featureManager1 = Substitute.For<IFeatureManager>();
            featureManager1.FinalizeAsync(Arg.Any<IAppContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(0))
                .AndDoes(_ => order.Add(1));

            var featureManager2 = Substitute.For<IFeatureManager>();
            featureManager2.FinalizeAsync(Arg.Any<IAppContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(0))
                .AndDoes(_ => order.Add(2));

            var behavior1 = Substitute.For<IAppLifecycleBehavior>();
            behavior1.BeforeAppFinalizeAsync(Arg.Any<IAppContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(0))
                .AndDoes(_ => order.Add(11));
            behavior1.AfterAppFinalizeAsync(Arg.Any<IAppContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(0))
                .AndDoes(_ => order.Add(21));

            var behavior2 = Substitute.For<IAppLifecycleBehavior>();
            behavior2.BeforeAppFinalizeAsync(Arg.Any<IAppContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(0))
                .AndDoes(_ => order.Add(12));
            behavior2.AfterAppFinalizeAsync(Arg.Any<IAppContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(0))
                .AndDoes(_ => order.Add(22));

            var appManager = new DefaultAppManager(
                Substitute.For<IAppRuntime>(),
                GetCompositionContext(),
                GetServiceBehaviorProvider(),
                new[]
                    {
                        new ExportFactory<IAppLifecycleBehavior, AppServiceMetadata>(() => behavior1, new FeatureManagerMetadata(processingPriority: 2)),
                        new ExportFactory<IAppLifecycleBehavior, AppServiceMetadata>(() => behavior2, new FeatureManagerMetadata(processingPriority: 1)),
                    },
                new[]
                    {
                        new ExportFactory<IFeatureManager, FeatureManagerMetadata>(() => featureManager1, new FeatureManagerMetadata(processingPriority: 2)),
                        new ExportFactory<IFeatureManager, FeatureManagerMetadata>(() => featureManager2, new FeatureManagerMetadata(processingPriority: 1)),
                    },
                null);

            await appManager.FinalizeAppAsync(Substitute.For<IAppContext>(), CancellationToken.None);

            Assert.AreEqual(6, order.Count);
            Assert.AreEqual(11, order[0]);
            Assert.AreEqual(12, order[1]);
            Assert.AreEqual(1, order[2]);
            Assert.AreEqual(2, order[3]);
            Assert.AreEqual(21, order[4]);
            Assert.AreEqual(22, order[5]);
        }

        [Test]
        public async Task FinalizeAppAsync_right_featureManager_order_processingPriority()
        {
            var order = new List<int>();

            var finalizer1 = Substitute.For<IFeatureManager>();
            finalizer1.FinalizeAsync(Arg.Any<IAppContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(0))
                .AndDoes(_ => order.Add(1));

            var finalizer2 = Substitute.For<IFeatureManager>();
            finalizer2.FinalizeAsync(Arg.Any<IAppContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(0))
                .AndDoes(_ => order.Add(2));

            var appManager = new DefaultAppManager(
                Substitute.For<IAppRuntime>(),
                GetCompositionContext(),
                GetServiceBehaviorProvider(),
                new List<IExportFactory<IAppLifecycleBehavior, AppServiceMetadata>>(),
                new[]
                    {
                        new ExportFactory<IFeatureManager, FeatureManagerMetadata>(() => finalizer1, new FeatureManagerMetadata(processingPriority: 2)),
                        new ExportFactory<IFeatureManager, FeatureManagerMetadata>(() => finalizer2, new FeatureManagerMetadata(processingPriority: 1)),
                    },
                null);

            await appManager.FinalizeAppAsync(Substitute.For<IAppContext>(), CancellationToken.None);

            Assert.AreEqual(2, order.Count);
            Assert.AreEqual(1, order[0]);
            Assert.AreEqual(2, order[1]);
        }

        [Test]
        public async Task FinalizeAppAsync_right_featureManager_order_processingPriority_marginal_values()
        {
            var order = new List<int>();

            var finalizer1 = Substitute.For<IFeatureManager>();
            finalizer1.FinalizeAsync(Arg.Any<IAppContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(0))
                .AndDoes(_ => order.Add(1));

            var finalizer2 = Substitute.For<IFeatureManager>();
            finalizer2.FinalizeAsync(Arg.Any<IAppContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(0))
                .AndDoes(_ => order.Add(2));

            var finalizer3 = Substitute.For<IFeatureManager>();
            finalizer3.FinalizeAsync(Arg.Any<IAppContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(0))
                .AndDoes(_ => order.Add(3));

            var appManager = new DefaultAppManager(
                Substitute.For<IAppRuntime>(),
                GetCompositionContext(),
                GetServiceBehaviorProvider(),
                new List<IExportFactory<IAppLifecycleBehavior, AppServiceMetadata>>(),
                new[]
                    {
                        new ExportFactory<IFeatureManager, FeatureManagerMetadata>(() => finalizer1, new FeatureManagerMetadata(processingPriority: 0)),
                        new ExportFactory<IFeatureManager, FeatureManagerMetadata>(() => finalizer2, new FeatureManagerMetadata(processingPriority: int.MaxValue)),
                        new ExportFactory<IFeatureManager, FeatureManagerMetadata>(() => finalizer3, new FeatureManagerMetadata(processingPriority: int.MinValue)),
                    },
                null);

            await appManager.FinalizeAppAsync(Substitute.For<IAppContext>(), CancellationToken.None);

            Assert.AreEqual(3, order.Count);
            Assert.AreEqual(2, order[0]);
            Assert.AreEqual(1, order[1]);
            Assert.AreEqual(3, order[2]);
        }

        [Test]
        public async Task FinalizeAppAsync_right_featureManager_order_dependencies()
        {
            var order = new List<int>();

            var finalizer1 = CreateFeatureManager(finalization: _ => order.Add(1));
            var finalizer2 = CreateFeatureManager(finalization: _ => order.Add(2));
            var finalizer3 = CreateFeatureManager(finalization: _ => order.Add(3));

            var appManager = new DefaultAppManager(
                Substitute.For<IAppRuntime>(),
                GetCompositionContext(),
                GetServiceBehaviorProvider(),
                new List<IExportFactory<IAppLifecycleBehavior, AppServiceMetadata>>(),
                new[]
                    {
                        new ExportFactory<IFeatureManager, FeatureManagerMetadata>(() => finalizer1, new FeatureManagerMetadata(new FeatureInfo("1", "1.0", dependencies: new[] { "3" }))),
                        new ExportFactory<IFeatureManager, FeatureManagerMetadata>(() => finalizer2, new FeatureManagerMetadata(new FeatureInfo("2", "1.0", dependencies: new[] { "1" }))),
                        new ExportFactory<IFeatureManager, FeatureManagerMetadata>(() => finalizer3, new FeatureManagerMetadata(new FeatureInfo("3", "1.0"))),
                    },
                null);

            await appManager.FinalizeAppAsync(Substitute.For<IAppContext>(), CancellationToken.None);

            Assert.AreEqual(3, order.Count);
            Assert.AreEqual(2, order[0]);
            Assert.AreEqual(1, order[1]);
            Assert.AreEqual(3, order[2]);
        }

        [Test]
        public async Task FinalizeAppAsync_right_feature_behavior_order_with_filter()
        {
            var order = new List<int>();

            var finalizer1 = CreateFeatureManager(finalization: _ => order.Add(1));
            var finalizer2 = CreateFeatureManager(finalization: _ => order.Add(2));
            var finalizer3 = CreateFeatureManager(finalization: _ => order.Add(3));

            var behaviors = new List<string>();

            var behavior1 = CreateFeatureLifecycleBehavior(
                    beforeFinalize: ci => behaviors.Add("BeforeFin 1"),
                    afterFinalize: ci => behaviors.Add("AfterFin 1"));
            var behavior2 = CreateFeatureLifecycleBehavior(
                    beforeFinalize: ci => behaviors.Add("BeforeFin 2"),
                    afterFinalize: ci => behaviors.Add("AfterFin 2"));
            var behavior3 = CreateFeatureLifecycleBehavior(
                    beforeFinalize: ci => behaviors.Add("BeforeFin 3"),
                    afterFinalize: ci => behaviors.Add("AfterFin 3"));

            var appManager = new DefaultAppManager(
                Substitute.For<IAppRuntime>(),
                GetCompositionContext(),
                GetServiceBehaviorProvider(),
                new List<IExportFactory<IAppLifecycleBehavior, AppServiceMetadata>>(),
                new[]
                    {
                        new ExportFactory<IFeatureManager, FeatureManagerMetadata>(() => finalizer1, new FeatureManagerMetadata(new FeatureInfo("1", "1.0", dependencies: new[] { "3" }))),
                        new ExportFactory<IFeatureManager, FeatureManagerMetadata>(() => finalizer2, new FeatureManagerMetadata(new FeatureInfo("2", "1.0", dependencies: new[] { "1" }))),
                        new ExportFactory<IFeatureManager, FeatureManagerMetadata>(() => finalizer3, new FeatureManagerMetadata(new FeatureInfo("3", "1.0"))),
                    },
                new[]
                    {
                        new ExportFactory<IFeatureLifecycleBehavior, FeatureLifecycleBehaviorMetadata>(() => behavior1, new FeatureLifecycleBehaviorMetadata("1")),
                        new ExportFactory<IFeatureLifecycleBehavior, FeatureLifecycleBehaviorMetadata>(() => behavior2, new FeatureLifecycleBehaviorMetadata("2")),
                        new ExportFactory<IFeatureLifecycleBehavior, FeatureLifecycleBehaviorMetadata>(() => behavior3, new FeatureLifecycleBehaviorMetadata()),
                    });

            await appManager.FinalizeAppAsync(Substitute.For<IAppContext>(), CancellationToken.None);

            Assert.AreEqual(3, order.Count);
            Assert.AreEqual(2, order[0]);
            Assert.AreEqual(1, order[1]);
            Assert.AreEqual(3, order[2]);

            Assert.AreEqual(10, behaviors.Count);
            Assert.AreEqual("BeforeFin 2", behaviors[0]);
            Assert.AreEqual("BeforeFin 3", behaviors[1]);
            Assert.AreEqual("AfterFin 2", behaviors[2]);
            Assert.AreEqual("AfterFin 3", behaviors[3]);
            Assert.AreEqual("BeforeFin 1", behaviors[4]);
            Assert.AreEqual("BeforeFin 3", behaviors[5]);
            Assert.AreEqual("AfterFin 1", behaviors[6]);
            Assert.AreEqual("AfterFin 3", behaviors[7]);
            Assert.AreEqual("BeforeFin 3", behaviors[8]);
            Assert.AreEqual("AfterFin 3", behaviors[9]);
        }

        [Test]
        public async Task FinalizeAppAsync_right_behavior_order()
        {
            var order = new List<string>();

            var featureManager1 = CreateFeatureManager();
            var featureManager2 = CreateFeatureManager();

            var behavior1 = CreateFeatureLifecycleBehavior(
                beforeFinalize: _ => order.Add("Before1"),
                afterFinalize: _ => order.Add("After1"));

            var behavior2 = CreateFeatureLifecycleBehavior(
                beforeFinalize: _ => order.Add("Before2"),
                afterFinalize: _ => order.Add("After2"));

            var appManager = new DefaultAppManager(
                Substitute.For<IAppRuntime>(),
                GetCompositionContext(),
                GetServiceBehaviorProvider(),
                new List<IExportFactory<IAppLifecycleBehavior, AppServiceMetadata>>(),
                new[]
                    {
                        new ExportFactory<IFeatureManager, FeatureManagerMetadata>(() => featureManager1, new FeatureManagerMetadata()),
                        new ExportFactory<IFeatureManager, FeatureManagerMetadata>(() => featureManager2, new FeatureManagerMetadata()),
                    },
                new[]
                    {
                        new ExportFactory<IFeatureLifecycleBehavior, FeatureLifecycleBehaviorMetadata>(() => behavior1, new FeatureLifecycleBehaviorMetadata(processingPriority: 2)),
                        new ExportFactory<IFeatureLifecycleBehavior, FeatureLifecycleBehaviorMetadata>(() => behavior2, new FeatureLifecycleBehaviorMetadata(processingPriority: 1)),
                    });

            await appManager.FinalizeAppAsync(Substitute.For<IAppContext>(), CancellationToken.None);

            Assert.AreEqual(8, order.Count);
            Assert.AreEqual("Before1", order[0]);
            Assert.AreEqual("Before2", order[1]);
            Assert.AreEqual("After1", order[2]);
            Assert.AreEqual("After2", order[3]);
            Assert.AreEqual("Before1", order[4]);
            Assert.AreEqual("Before2", order[5]);
            Assert.AreEqual("After1", order[6]);
            Assert.AreEqual("After2", order[7]);
        }

        private ExportFactory<IFeatureManager, FeatureManagerMetadata> CreateFeatureManagerFactory(
            IFeatureManager featureManager,
            string name,
            string version = null,
            bool isRequired = false,
            string[] dependencies = null,
            Priority processingPriority = Priority.Normal)
        {
            return new ExportFactory<IFeatureManager, FeatureManagerMetadata>(
                () => featureManager,
                new FeatureManagerMetadata(new FeatureInfo(name, version, isRequired, dependencies), processingPriority: (int)processingPriority));
        }

        private IFeatureManager CreateFeatureManager(Action<CallInfo> initialization = null, Action<CallInfo> finalization = null)
        {
            var featureManager = Substitute.For<IFeatureManager>();
            featureManager.InitializeAsync(Arg.Any<IAppContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(0))
                .AndDoes(_ => initialization?.Invoke(_));
            featureManager.FinalizeAsync(Arg.Any<IAppContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(0))
                .AndDoes(_ => finalization?.Invoke(_));

            return featureManager;
        }

        private IFeatureLifecycleBehavior CreateFeatureLifecycleBehavior(
            Action<CallInfo> beforeInit = null,
            Action<CallInfo> afterInit = null,
            Action<CallInfo> beforeFinalize = null,
            Action<CallInfo> afterFinalize = null)
        {
            var featureManager = Substitute.For<IFeatureLifecycleBehavior>();
            featureManager.BeforeInitializeAsync(Arg.Any<IAppContext>(), Arg.Any<FeatureManagerMetadata>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(0))
                .AndDoes(_ => beforeInit?.Invoke(_));
            featureManager.AfterInitializeAsync(Arg.Any<IAppContext>(), Arg.Any<FeatureManagerMetadata>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(0))
                .AndDoes(_ => afterInit?.Invoke(_));
            featureManager.BeforeFinalizeAsync(Arg.Any<IAppContext>(), Arg.Any<FeatureManagerMetadata>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(0))
                .AndDoes(_ => beforeFinalize?.Invoke(_));
            featureManager.AfterFinalizeAsync(Arg.Any<IAppContext>(), Arg.Any<FeatureManagerMetadata>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(0))
                .AndDoes(_ => afterFinalize?.Invoke(_));

            return featureManager;
        }

        private IServiceBehaviorProvider GetServiceBehaviorProvider()
        {
            var provider = new DefaultServiceBehaviorProvider(Substitute.For<ICompositionContext>(), new List<IExportFactory<IEnabledServiceBehaviorRule, ServiceBehaviorRuleMetadata>>());
            return provider;
        }

        private ICompositionContext GetCompositionContext()
        {
            var exporter = Substitute.For<ICollectionExportFactoryImporter>();
            exporter.ExportFactories.Returns(new IExportFactory<IEnabledServiceBehaviorRule, ServiceBehaviorRuleMetadata>[0]);

            var compositionContext = Substitute.For<ICompositionContext>();
            compositionContext.GetExport(typeof(ICollectionExportFactoryImporter<IEnabledServiceBehaviorRule, ServiceBehaviorRuleMetadata>), null)
                .Returns(exporter);

            return compositionContext;
        }
    }
}