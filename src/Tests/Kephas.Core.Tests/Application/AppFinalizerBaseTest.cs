// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppFinalizerBaseTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the application finalizer base test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests.Application
{
    using System;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Services.Transitioning;
    using Kephas.Threading.Tasks;

    using NSubstitute;

    using NUnit.Framework;

    public class AppFinalizerBaseTest
    {
        [Test]
        public void InitializeAsync_exception()
        {
            var appInitializer = new TestAppFinalizer(new AmbiguousMatchException());

            Assert.That(() => appInitializer.FinalizeAsync(Substitute.For<IAppContext>()), Throws.TypeOf<AmbiguousMatchException>());

            Assert.IsTrue(appInitializer.GetInitializationMonitor().IsFaulted);
        }

        [Test]
        public async Task InitializeAsync_success()
        {
            var appInitializer = new TestAppFinalizer();

            await appInitializer.FinalizeAsync(Substitute.For<IAppContext>());

            Assert.IsTrue(appInitializer.GetInitializationMonitor().IsCompletedSuccessfully);
        }
        private class TestAppFinalizer : AppFinalizerBase
        {
            private readonly Exception exception;

            public TestAppFinalizer()
            {
            }

            public TestAppFinalizer(Exception exception)
            {
                this.exception = exception;
            }

            public InitializationMonitor<IAppFinalizer> GetInitializationMonitor()
            {
                return this.InitializationMonitor;
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