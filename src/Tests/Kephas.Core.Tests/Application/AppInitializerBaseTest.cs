namespace Kephas.Core.Tests.Application
{
    using System;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Services.Transitioning;

    using NUnit.Framework;

    using Telerik.JustMock;

    using TaskHelper = Kephas.Threading.Tasks.TaskHelper;

    [TestFixture]
    public class AppInitializerBaseTest
    {
        [Test]
        public void InitializeAsync_exception()
        {
            var appInitializer = new TestAppInitializer(new AmbiguousMatchException());

            Assert.That(() => appInitializer.InitializeAsync(Mock.Create<IAppContext>()), Throws.TypeOf<AmbiguousMatchException>());

            Assert.IsTrue(appInitializer.GetInitializationMonitor().IsFaulted);
        }

        [Test]
        public async Task InitializeAsync_success()
        {
            var appInitializer = new TestAppInitializer();

            await appInitializer.InitializeAsync(Mock.Create<IAppContext>());

            Assert.IsTrue(appInitializer.GetInitializationMonitor().IsCompletedSuccessfully);
        }

        private class TestAppInitializer : AppInitializerBase
        {
            private readonly Exception exception;

            public TestAppInitializer()
            {
            }

            public TestAppInitializer(Exception exception)
            {
                this.exception = exception;
            }

            public InitializationMonitor<IAppInitializer> GetInitializationMonitor()
            {
                return this.InitializationMonitor;
            }

            /// <summary>
            /// Initializes the application asynchronously.
            /// </summary>
            /// <param name="appContext">Context for the application.</param>
            /// <param name="cancellationToken">The cancellation token.</param>
            /// <returns>
            /// A Task.
            /// </returns>
            protected override Task InitializeCoreAsync(IAppContext appContext, CancellationToken cancellationToken)
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