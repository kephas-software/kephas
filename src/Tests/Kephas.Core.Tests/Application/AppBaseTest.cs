// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppBaseTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the application base test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests.Application
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Composition;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class AppBaseTest
    {
        [Test]
        public async Task ConfigureAmbientServicesAsync_ambient_services_set()
        {
            var compositionContext = Substitute.For<ICompositionContext>();

            AmbientServicesBuilder builder = null;
            var app = new TestApp(async b => builder = b.WithCompositionContainer(compositionContext));
            await app.StartApplicationAsync();

            Assert.IsNotNull(builder);
            Assert.IsNotNull(builder.AmbientServices);
        }

        [Test]
        public async Task StartApplicationAsync_appManager_invoked()
        {
            var appManager = Substitute.For<IAppManager>();

            var compositionContext = Substitute.For<ICompositionContext>();
            compositionContext.GetExport<IAppManager>(Arg.Any<string>()).Returns(appManager);

            AmbientServicesBuilder builder = null;
            var app = new TestApp(async b => builder = b.WithCompositionContainer(compositionContext));
            await app.StartApplicationAsync();

            appManager.Received(1).InitializeAppAsync(Arg.Any<IAppContext>(), Arg.Any<CancellationToken>());
        }

        [Test]
        public async Task StartApplicationAsync_runAsync_invoked()
        {
            var appManager = Substitute.For<IAppManager>();

            var compositionContext = Substitute.For<ICompositionContext>();
            compositionContext.GetExport<IAppManager>(Arg.Any<string>()).Returns(appManager);

            string runCalled = null;
            AmbientServicesBuilder builder = null;
            var app = new TestApp(async b => builder = b.WithCompositionContainer(compositionContext), async svc => runCalled = "run called");
            await app.StartApplicationAsync();

            Assert.AreEqual("run called", runCalled);
        }

        [Test]
        public async Task StopApplicationAsync_appManager_invoked()
        {
            var appManager = Substitute.For<IAppManager>();

            var compositionContext = Substitute.For<ICompositionContext>();
            compositionContext.GetExport<IAppManager>(Arg.Any<string>()).Returns(appManager);

            var ambientServices = new AmbientServicesBuilder(new AmbientServices())
                .WithCompositionContainer(compositionContext).AmbientServices;
            var app = new TestApp();
            await app.StopApplicationAsync(ambientServices);

            appManager.Received(1).FinalizeAppAsync(Arg.Any<IAppContext>(), Arg.Any<CancellationToken>());
        }
    }

    public class TestApp : AppBase
    {
        private readonly Func<AmbientServicesBuilder, Task> asyncConfig;

        private readonly Func<IAmbientServices, Task> asyncRun;

        public TestApp(Func<AmbientServicesBuilder, Task> asyncConfig = null, Func<IAmbientServices, Task> asyncRun = null)
        {
            this.asyncConfig = asyncConfig;
            this.asyncRun = asyncRun;
        }

        /// <summary>
        /// Configures the ambient services asynchronously.
        /// </summary>
        /// <param name="appArgs">The application arguments.</param>
        /// <param name="ambientServicesBuilder">The ambient services builder.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// The asynchronous result.
        /// </returns>
        protected override async Task ConfigureAmbientServicesAsync(
            string[] appArgs,
            AmbientServicesBuilder ambientServicesBuilder,
            CancellationToken cancellationToken)
        {
            if (this.asyncConfig != null)
            {
                await this.asyncConfig(ambientServicesBuilder);
            }
        }

        /// <summary>
        /// Executes the application main functionality asynchronously.
        /// </summary>
        /// <remarks>
        /// This method should be overwritten to provide a meaningful content.
        /// </remarks>
        /// <param name="appArgs">The application arguments.</param>
        /// <param name="ambientServices">The configured ambient services.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// The asynchronous result.
        /// </returns>
        protected override Task RunAsync(string[] appArgs, IAmbientServices ambientServices, CancellationToken cancellationToken)
        {
            if (this.asyncRun != null)
            {
                return this.asyncRun(ambientServices);
            }

            return base.RunAsync(appArgs, ambientServices, cancellationToken);
        }
    }
}