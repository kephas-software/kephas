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
        public async Task ConfigureAmbientServicesAsync_ambient_services_static_instance_set()
        {
            var compositionContext = Substitute.For<ICompositionContext>();

            AmbientServicesBuilder builder = null;
            var app = new TestApp(async b => builder = b.WithCompositionContainer(compositionContext));
            var appContext = await app.BootstrapAsync();

            Assert.IsNotNull(builder);
            Assert.IsNotNull(builder.AmbientServices);
            Assert.AreSame(AmbientServices.Instance, builder.AmbientServices);
            Assert.AreSame(AmbientServices.Instance, appContext.AmbientServices);
        }

        [Test]
        public async Task ConfigureAmbientServicesAsync_ambient_services_explicit_instance_set()
        {
            var compositionContext = Substitute.For<ICompositionContext>();

            AmbientServicesBuilder builder = null;
            var app = new TestApp(async b => builder = b.WithCompositionContainer(compositionContext));
            var ambientServices = new AmbientServices();
            var appContext = await app.BootstrapAsync(ambientServices: ambientServices);

            Assert.AreSame(ambientServices, builder.AmbientServices);
            Assert.AreSame(ambientServices, appContext.AmbientServices);
        }

        [Test]
        public async Task BootstrapAsync_appManager_invoked()
        {
            var appManager = Substitute.For<IAppManager>();

            var compositionContext = Substitute.For<ICompositionContext>();
            compositionContext.GetExport<IAppManager>(Arg.Any<string>()).Returns(appManager);

            AmbientServicesBuilder builder = null;
            var app = new TestApp(async b => builder = b.WithCompositionContainer(compositionContext));
            await app.BootstrapAsync();

            appManager.Received(1).InitializeAppAsync(Arg.Any<IAppContext>(), Arg.Any<CancellationToken>());
        }

        [Test]
        public async Task BootstrapAsync_signalShutdown_stops_application()
        {
            var appManager = Substitute.For<IAppManager>();

            var compositionContext = Substitute.For<ICompositionContext>();
            compositionContext.GetExport<IAppManager>(Arg.Any<string>()).Returns(appManager);

            var app = new TestApp(async b => b.WithCompositionContainer(compositionContext));
            var appContext = await app.BootstrapAsync();

            await appContext.SignalShutdown(null);

            appManager.Received(1).FinalizeAppAsync(Arg.Any<IAppContext>(), Arg.Any<CancellationToken>());
        }

        [Test]
        public async Task ShutdownAsync_appManager_invoked()
        {
            var appManager = Substitute.For<IAppManager>();

            var compositionContext = Substitute.For<ICompositionContext>();
            compositionContext.GetExport<IAppManager>(Arg.Any<string>()).Returns(appManager);

            var ambientServices = new AmbientServicesBuilder(new AmbientServices())
                .WithCompositionContainer(compositionContext).AmbientServices;
            var app = new TestApp();
            await app.ShutdownAsync(ambientServices);

            appManager.Received(1).FinalizeAppAsync(Arg.Any<IAppContext>(), Arg.Any<CancellationToken>());
        }
    }

    public class TestApp : AppBase
    {
        private readonly Func<AmbientServicesBuilder, Task> asyncConfig;

        public TestApp(Func<AmbientServicesBuilder, Task> asyncConfig = null)
        {
            this.asyncConfig = asyncConfig;
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
    }
}