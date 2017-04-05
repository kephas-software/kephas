namespace Kephas.Core.Tests.Application
{
    using System;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Services.Transitioning;

    using NSubstitute;

    using NUnit.Framework;

    using TaskHelper = Kephas.Threading.Tasks.TaskHelper;

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
                return this.InitializationMonitor;
            }

            public FinalizationMonitor<IFeatureManager> GetFinalizationMonitor()
            {
                return this.FinalizationMonitor;
            }

            protected override Task InitializeCoreAsync(IAppContext appContext, CancellationToken cancellationToken)
            {
                if (this.exception != null)
                {
                    throw this.exception;
                }

                return TaskHelper.CompletedTask;
            }

            protected override Task FinalizeCoreAsync(IAppContext appContext, CancellationToken cancellationToken)
            {
                if (this.exception != null)
                {
                    throw this.exception;
                }

                return TaskHelper.CompletedTask;
            }
        }
    }
}