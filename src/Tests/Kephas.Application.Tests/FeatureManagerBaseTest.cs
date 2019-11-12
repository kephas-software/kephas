// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FeatureManagerBaseTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the feature manager base test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application.Tests
{
    using System;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Services.Transitions;
    using NSubstitute;

    using NUnit.Framework;

    using TaskHelper = Threading.Tasks.TaskHelper;

    [TestFixture]
    public class FeatureManagerBaseTest
    {
        [Test]
        public void InitializeAsync_exception()
        {
            var appInitializer = new TestFeatureManager(new AmbiguousMatchException());

            Assert.That(() => appInitializer.InitializeAsync(Substitute.For<IAppContext>()), Throws.TypeOf<AmbiguousMatchException>());

            Assert.IsTrue(appInitializer.GetInitializationMonitor().IsFaulted);
        }

        [Test]
        public async Task InitializeAsync_success()
        {
            var appInitializer = new TestFeatureManager();

            await appInitializer.InitializeAsync(Substitute.For<IAppContext>());

            Assert.IsTrue(appInitializer.GetInitializationMonitor().IsCompletedSuccessfully);
        }

        [Test]
        public void FinalizeAsync_exception()
        {
            var appInitializer = new TestFeatureManager(new AmbiguousMatchException());

            Assert.That(() => appInitializer.FinalizeAsync(Substitute.For<IAppContext>()), Throws.TypeOf<AmbiguousMatchException>());

            Assert.IsTrue(appInitializer.GetFinalizationMonitor().IsFaulted);
        }

        [Test]
        public async Task FinalizeAsync_success()
        {
            var appInitializer = new TestFeatureManager();

            await appInitializer.FinalizeAsync(Substitute.For<IAppContext>());

            Assert.IsTrue(appInitializer.GetFinalizationMonitor().IsCompletedSuccessfully);
        }

        private class TestFeatureManager : FeatureManagerBase
        {
            private readonly Exception exception;

            public TestFeatureManager()
            {
            }

            public TestFeatureManager(Exception exception)
            {
                this.exception = exception;
            }

            public InitializationMonitor<IFeatureManager> GetInitializationMonitor()
            {
                return InitializationMonitor;
            }

            public FinalizationMonitor<IFeatureManager> GetFinalizationMonitor()
            {
                return FinalizationMonitor;
            }

            protected override Task InitializeCoreAsync(IAppContext appContext, CancellationToken cancellationToken)
            {
                if (exception != null)
                {
                    throw exception;
                }

                return TaskHelper.CompletedTask;
            }

            protected override Task FinalizeCoreAsync(IAppContext appContext, CancellationToken cancellationToken)
            {
                if (exception != null)
                {
                    throw exception;
                }

                return TaskHelper.CompletedTask;
            }
        }
    }
}