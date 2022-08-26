// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppBaseTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the application base test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Tests.Application
{
    using System;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using Kephas;
    using Kephas.Application;
    using Kephas.Configuration;
    using Kephas.Injection;
    using Kephas.Operations;
    using Kephas.Runtime;
    using Kephas.Services;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class AppBaseTest : ApplicationTestBase
    {
        public override IEnumerable<Assembly> GetAssemblies()
        {
            return new List<Assembly>(base.GetAssemblies())
            {
                typeof(IConfiguration<>).Assembly,  // Kephas.Configuration.
            };
        }

        [Test]
        public async Task ConfigureAmbientServicesAsync_ambient_services_static_instance_set()
        {
            var injector = Substitute.For<IInjector>();

            IAmbientServices? ambientServices = null;
            var app = new TestApp(async b => ambientServices = b.WithInjector(injector));
            var (appContext, instruction) = await app.RunAsync();

            Assert.IsNotNull(ambientServices);
            Assert.AreSame(app.AmbientServices, ambientServices);
            Assert.AreSame(app.AmbientServices, appContext.AmbientServices);
        }

        [Test]
        public async Task ConfigureAmbientServicesAsync_ambient_services_explicit_instance_set()
        {
            var injector = Substitute.For<IInjector>();

            IAmbientServices? ambientServices = null;
            var app = new TestApp(async b => ambientServices = b.WithInjector(injector));
            var (appContext, instruction) = await app.RunAsync();

            Assert.AreSame(app.AmbientServices, ambientServices);
            Assert.AreSame(app.AmbientServices, appContext.AmbientServices);
        }

        [Test]
        public async Task RunAsync_appManager_invoked()
        {
            var appManager = Substitute.For<IAppManager>();

            var injector = Substitute.For<IInjector>();
            injector.Resolve<IAppManager>().Returns(appManager);

            IAmbientServices? ambientServices = null;
            var app = new TestApp(async b => ambientServices = b.WithInjector(injector));
            await app.RunAsync();

            appManager.Received(1).InitializeAsync(Arg.Any<IAppContext>(), Arg.Any<CancellationToken>());
        }

        [Test]
        public async Task RunAsync_wait_for_shutdown_exception_stops_application_callback()
        {
            var appManager = Substitute.For<IAppManager>();
            var injector = Substitute.For<IInjector>();
            injector.Resolve<IAppManager>()
                .Returns(appManager);

            var app = new TestApp(async b => b.WithInjector(injector));
            var (appContext, instruction) = await app.RunAsync(_ => throw new InvalidOperationException("bad thing happened"));

            appManager.Received(1).FinalizeAsync(Arg.Any<IAppContext>(), Arg.Any<CancellationToken>());
            var appResult = (IOperationResult)appContext.AppResult;
            Assert.IsNull(appResult);
        }

        [Test]
        public async Task RunAsync_wait_for_shutdown_exception_stops_application()
        {
            var appManager = Substitute.For<IAppManager>();
            var termAwaiter = Substitute.For<IAppMainLoop>();
            termAwaiter.Main(Arg.Any<CancellationToken>())
                .Returns<(IOperationResult result, AppShutdownInstruction instruction)>(ci => throw new InvalidOperationException("bad thing happened"));

            var injector = Substitute.For<IInjector>();
            injector.Resolve<IAppManager>()
                .Returns(appManager);
            injector.TryResolve<IAppMainLoop>()
                .Returns(termAwaiter);

            var app = new TestApp(async b => b.WithInjector(injector));
            var (appContext, instruction) = await app.RunAsync();

            appManager.Received(1).FinalizeAsync(Arg.Any<IAppContext>(), Arg.Any<CancellationToken>());
            var appResult = (IOperationResult)appContext.AppResult;
            Assert.IsNull(appResult);
        }

        [Test]
        public async Task RunAsync_shutdown_instruction_stops_application_callback()
        {
            var appManager = Substitute.For<IAppManager>();

            var injector = Substitute.For<IInjector>();
            injector.Resolve<IAppManager>()
                .Returns(appManager);

            var app = new TestApp(async b => b.WithInjector(injector));
            var (appContext, instruction) = await app.RunAsync(async _ => (new OperationResult { Value = 12 }, AppShutdownInstruction.Shutdown));

            appManager.Received(1).FinalizeAsync(Arg.Any<IAppContext>(), Arg.Any<CancellationToken>());
            var appResult = (IOperationResult)appContext.AppResult;
            Assert.AreEqual(12, appResult.Value);
        }

        [Test]
        public async Task RunAsync_shutdown_instruction_stops_application()
        {
            var appManager = Substitute.For<IAppManager>();
            var mainLoop = Substitute.For<IAppMainLoop>();
            mainLoop.Main(Arg.Any<CancellationToken>())
                .Returns((new OperationResult { Value = 12 }, AppShutdownInstruction.Shutdown));

            var injector = Substitute.For<IInjector>();
            injector.Resolve<IAppManager>()
                .Returns(appManager);
            injector.TryResolve<IAppMainLoop>()
                .Returns(mainLoop);

            var app = new TestApp(async b => b.WithInjector(injector));
            var (appContext, instruction) = await app.RunAsync();

            appManager.Received(1).FinalizeAsync(Arg.Any<IAppContext>(), Arg.Any<CancellationToken>());
            var appResult = (IOperationResult)appContext.AppResult;
            Assert.AreEqual(12, appResult.Value);
        }

        [Test]
        public async Task RunAsync_none_instruction_does_not_stop_application_callback()
        {
            var appManager = Substitute.For<IAppManager>();

            var injector = Substitute.For<IInjector>();
            injector.Resolve<IAppManager>()
                .Returns(appManager);

            var app = new TestApp(async b => b.WithInjector(injector));
            var (appContext, instruction) = await app.RunAsync(async _ => (new OperationResult { Value = 23 }, AppShutdownInstruction.Ignore));

            appManager.Received(0).FinalizeAsync(Arg.Any<IAppContext>(), Arg.Any<CancellationToken>());
            var appResult = (IOperationResult)appContext.AppResult;
            Assert.AreEqual(23, appResult.Value);
        }

        [Test]
        public async Task RunAsync_none_instruction_does_not_stop_application()
        {
            var appManager = Substitute.For<IAppManager>();
            var termAwaiter = Substitute.For<IAppMainLoop>();
            termAwaiter.Main(Arg.Any<CancellationToken>())
                .Returns((new OperationResult { Value = 23 }, AppShutdownInstruction.Ignore));

            var injector = Substitute.For<IInjector>();
            injector.Resolve<IAppManager>()
                .Returns(appManager);
            injector.TryResolve<IAppMainLoop>()
                .Returns(termAwaiter);

            var app = new TestApp(async b => b.WithInjector(injector));
            var (appContext, instruction) = await app.RunAsync();

            appManager.Received(0).FinalizeAsync(Arg.Any<IAppContext>(), Arg.Any<CancellationToken>());
            var appResult = (IOperationResult)appContext.AppResult;
            Assert.AreEqual(23, appResult.Value);
        }

        [Test]
        public async Task RunAsync_injection()
        {
            var container = this.CreateInjector(parts: new[] { typeof(TestApp), typeof(TestMainLoop), typeof(TestShutdownFeatureManager) });
            var app = new TestApp(ambientServices: container.Resolve<IAmbientServices>());
            var (appContext, instruction) = await app.RunAsync();

            Assert.AreEqual(AppShutdownInstruction.Ignore, instruction);

            await app.ShutdownAsync();
        }

        [Test]
        public async Task ShutdownAsync_appManager_invoked()
        {
            var appManager = Substitute.For<IAppManager>();

            var injector = Substitute.For<IInjector>();
            injector.Resolve<IAppManager>().Returns(appManager);

            var ambientServices = this.CreateAmbientServices()
                .WithInjector(injector);
            var app = new TestApp(ambientServices: ambientServices);
            await app.ShutdownAsync();

            appManager.Received(1).FinalizeAsync(Arg.Any<IAppContext>(), Arg.Any<CancellationToken>());
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

    public class TestApp : AppBase<AmbientServices>
    {
        private readonly Func<IAmbientServices, Task>? asyncConfig;

        public TestApp(Func<IAmbientServices, Task>? asyncConfig = null, IAmbientServices? ambientServices = null)
            : base(ambientServices ?? CreateAmbientServices())
        {
            this.asyncConfig = asyncConfig;
        }

        /// <summary>
        /// Configures the ambient services asynchronously.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        protected override async void BuildServicesContainer(IAmbientServices ambientServices)
        {
            if (this.asyncConfig != null)
            {
                await this.asyncConfig(ambientServices);
            }
        }

        private static IAmbientServices CreateAmbientServices() =>
            new AmbientServices(typeRegistry: new RuntimeTypeRegistry());
    }
}