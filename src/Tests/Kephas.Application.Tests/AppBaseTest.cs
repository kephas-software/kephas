// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppBaseTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the application base test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Injection;

namespace Kephas.Application.Tests
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Kephas;
    using Kephas.Application;
    using Kephas.Operations;
    using Kephas.Services;
    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class AppBaseTest : ApplicationTestBase
    {
        [Test]
        public async Task ConfigureAmbientServicesAsync_ambient_services_static_instance_set()
        {
            var compositionContext = Substitute.For<IInjector>();

            IAmbientServices? ambientServices = null;
            var app = new TestApp(async b => ambientServices = b.WithCompositionContainer(compositionContext));
            var (appContext, instruction) = await app.BootstrapAsync();

            Assert.IsNotNull(ambientServices);
            Assert.AreSame(app.AmbientServices, ambientServices);
            Assert.AreSame(app.AmbientServices, appContext.AmbientServices);
        }

        [Test]
        public async Task ConfigureAmbientServicesAsync_ambient_services_explicit_instance_set()
        {
            var compositionContext = Substitute.For<IInjector>();

            IAmbientServices? ambientServices = null;
            var app = new TestApp(async b => ambientServices = b.WithCompositionContainer(compositionContext));
            var (appContext, instruction) = await app.BootstrapAsync();

            Assert.AreSame(app.AmbientServices, ambientServices);
            Assert.AreSame(app.AmbientServices, appContext.AmbientServices);
        }

        [Test]
        public async Task BootstrapAsync_appManager_invoked()
        {
            var appManager = Substitute.For<IAppManager>();

            var compositionContext = Substitute.For<IInjector>();
            compositionContext.GetExport<IAppManager>(Arg.Any<string>()).Returns(appManager);

            IAmbientServices? ambientServices = null;
            var app = new TestApp(async b => ambientServices = b.WithCompositionContainer(compositionContext));
            await app.BootstrapAsync();

            appManager.Received(1).InitializeAppAsync(Arg.Any<IAppContext>(), Arg.Any<CancellationToken>());
        }

        [Test]
        public async Task BootstrapAsync_wait_for_shutdown_exception_stops_application()
        {
            var appManager = Substitute.For<IAppManager>();
            var termAwaiter = Substitute.For<IAppMainLoop>();
            termAwaiter.Main(Arg.Any<CancellationToken>())
                .Returns<(IOperationResult result, AppShutdownInstruction instruction)>(ci => throw new InvalidOperationException("bad thing happened"));

            var compositionContext = Substitute.For<IInjector>();
            compositionContext.GetExport<IAppManager>(Arg.Any<string>())
                .Returns(appManager);
            compositionContext.GetExport<IAppMainLoop>(Arg.Any<string>())
                .Returns(termAwaiter);

            var app = new TestApp(async b => b.WithCompositionContainer(compositionContext));
            var (appContext, instruction) = await app.BootstrapAsync();

            appManager.Received(1).FinalizeAppAsync(Arg.Any<IAppContext>(), Arg.Any<CancellationToken>());
            var appResult = (IOperationResult)appContext.AppResult;
            Assert.IsNull(appResult);
        }

        [Test]
        public async Task BootstrapAsync_shutdown_instruction_stops_application()
        {
            var appManager = Substitute.For<IAppManager>();
            var mainLoop = Substitute.For<IAppMainLoop>();
            mainLoop.Main(Arg.Any<CancellationToken>())
                .Returns((new OperationResult { Value = 12 }, AppShutdownInstruction.Shutdown));

            var compositionContext = Substitute.For<IInjector>();
            compositionContext.GetExport<IAppManager>(Arg.Any<string>())
                .Returns(appManager);
            compositionContext.GetExport<IAppMainLoop>(Arg.Any<string>())
                .Returns(mainLoop);

            var app = new TestApp(async b => b.WithCompositionContainer(compositionContext));
            var (appContext, instruction) = await app.BootstrapAsync();

            appManager.Received(1).FinalizeAppAsync(Arg.Any<IAppContext>(), Arg.Any<CancellationToken>());
            var appResult = (IOperationResult)appContext.AppResult;
            Assert.AreEqual(12, appResult.Value);
        }

        [Test]
        public async Task BootstrapAsync_none_instruction_does_not_stop_application()
        {
            var appManager = Substitute.For<IAppManager>();
            var termAwaiter = Substitute.For<IAppMainLoop>();
            termAwaiter.Main(Arg.Any<CancellationToken>())
                .Returns((new OperationResult { Value = 23 }, AppShutdownInstruction.Ignore));

            var compositionContext = Substitute.For<IInjector>();
            compositionContext.GetExport<IAppManager>(Arg.Any<string>())
                .Returns(appManager);
            compositionContext.GetExport<IAppMainLoop>(Arg.Any<string>())
                .Returns(termAwaiter);

            var app = new TestApp(async b => b.WithCompositionContainer(compositionContext));
            var (appContext, instruction) = await app.BootstrapAsync();

            appManager.Received(0).FinalizeAppAsync(Arg.Any<IAppContext>(), Arg.Any<CancellationToken>());
            var appResult = (IOperationResult)appContext.AppResult;
            Assert.AreEqual(23, appResult.Value);
        }

        [Test]
        public async Task BootstrapAsync_composition()
        {
            var container = this.CreateContainer(parts: new[] { typeof(TestApp), typeof(TestMainLoop), typeof(TestShutdownFeatureManager) });
            var app = new TestApp(ambientServices: container.GetExport<IAmbientServices>());
            var (appContext, instruction) = await app.BootstrapAsync();

            Assert.AreEqual(AppShutdownInstruction.Ignore, instruction);

            await app.ShutdownAsync();
        }

        [Test]
        public async Task ShutdownAsync_appManager_invoked()
        {
            var appManager = Substitute.For<IAppManager>();

            var compositionContext = Substitute.For<IInjector>();
            compositionContext.GetExport<IAppManager>(Arg.Any<string>()).Returns(appManager);

            var ambientServices = new AmbientServices()
                .WithCompositionContainer(compositionContext);
            var app = new TestApp(ambientServices: ambientServices);
            await app.ShutdownAsync();

            appManager.Received(1).FinalizeAppAsync(Arg.Any<IAppContext>(), Arg.Any<CancellationToken>());
        }

        public class TestShutdownFeatureManager : FeatureManagerBase
        {
            private readonly IAppMainLoop awaiter;

            public TestShutdownFeatureManager(IAppMainLoop awaiter)
            {
                this.awaiter = awaiter;
            }

            protected override Task InitializeCoreAsync(IAppContext appContext, CancellationToken cancellationToken)
            {
                (this.awaiter as TestMainLoop).SignalShutdown();
                return base.InitializeCoreAsync(appContext, cancellationToken);
            }
        }

        public class TestMainLoop : IAppMainLoop, IInitializable
        {
            bool initialized = false;

            public void SignalShutdown()
            {
                if (!this.initialized)
                {
                    throw new InvalidOperationException("Awaiter not initialized");
                }
            }

            public void Initialize(IContext context = null)
            {
                this.initialized = true;
            }

            public async Task<(IOperationResult result, AppShutdownInstruction instruction)> Main(CancellationToken cancellationToken = default)
            {
                return (new OperationResult(), AppShutdownInstruction.Ignore);
            }
        }
    }

    public class TestApp : AppBase
    {
        private readonly Func<IAmbientServices, Task> asyncConfig;

        public TestApp(Func<IAmbientServices, Task> asyncConfig = null, IAmbientServices? ambientServices = null)
            : base(ambientServices ?? new AmbientServices())
        {
            this.asyncConfig = asyncConfig;
        }

        /// <summary>
        /// Configures the ambient services asynchronously.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        protected override async void BuildServicesContainer(IAmbientServices ambientServices)
        {
            if (asyncConfig != null)
            {
                await asyncConfig(ambientServices);
            }
        }
    }
}