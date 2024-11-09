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
    using Kephas.Operations;
    using Kephas.Runtime;
    using Kephas.Services;
    using Kephas.Services.Builder;
    using Kephas.Testing;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class AppBaseTest : ApplicationTestBase
    {
        protected override IEnumerable<Assembly> GetAssemblies()
        {
            return new List<Assembly>(base.GetAssemblies())
            {
                typeof(IConfiguration<>).Assembly,  // Kephas.Configuration.
            };
        }

        [Test]
        public async Task ConfigureAppServicesAsync()
        {
            var injector = Substitute.For<IServiceProvider>();

            IAppServiceCollection? appServices = null;
            var app = new TestApp(b =>
            {
                appServices = b.AppServices;
                return injector;
            });
            var (appContext, instruction) = await app.RunAsync();

            Assert.AreSame(app.ServicesBuilder.AppServices, appServices);
            Assert.AreSame(appServices, appContext.AppServices);
        }

        [Test]
        public async Task RunAsync_appManager_invoked()
        {
            var appManager = Substitute.For<IAppManager>();

            var injector = Substitute.For<IServiceProvider>();
            injector.Resolve<IAppManager>().Returns(appManager);

            var app = new TestApp(b => injector);
            await app.RunAsync();

            appManager.Received(1).InitializeAsync(Arg.Any<IAppContext>(), Arg.Any<CancellationToken>());
        }

        [Test]
        public async Task RunAsync_wait_for_shutdown_exception_stops_application_callback()
        {
            var appManager = Substitute.For<IAppManager>();
            var injector = Substitute.For<IServiceProvider>();
            injector.Resolve<IAppManager>()
                .Returns(appManager);

            var app = new TestApp(b => injector, () => throw new InvalidOperationException("bad thing happened"));
            var (appContext, instruction) = await app.RunAsync();

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
                .Returns<MainLoopResult>(ci => throw new InvalidOperationException("bad thing happened"));

            var injector = Substitute.For<IServiceProvider>();
            injector.Resolve<IAppManager>()
                .Returns(appManager);
            injector.TryResolve<IAppMainLoop>()
                .Returns(termAwaiter);

            var app = new TestApp(_ => injector);
            var (appContext, instruction) = await app.RunAsync();

            appManager.Received(1).FinalizeAsync(Arg.Any<IAppContext>(), Arg.Any<CancellationToken>());
            var appResult = (IOperationResult)appContext.AppResult;
            Assert.IsNull(appResult);
        }

        [Test]
        public async Task RunAsync_shutdown_instruction_stops_application_callback()
        {
            var appManager = Substitute.For<IAppManager>();

            var injector = Substitute.For<IServiceProvider>();
            injector.Resolve<IAppManager>()
                .Returns(appManager);

            var app = new TestApp(_ => injector, () => new MainLoopResult(new OperationResult { Value = 12 }, AppShutdownInstruction.Shutdown));
            var (appContext, instruction) = await app.RunAsync();

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
                .Returns(new MainLoopResult(new OperationResult { Value = 12 }, AppShutdownInstruction.Shutdown));

            var injector = Substitute.For<IServiceProvider>();
            injector.Resolve<IAppManager>()
                .Returns(appManager);
            injector.TryResolve<IAppMainLoop>()
                .Returns(mainLoop);

            var app = new TestApp(_ => injector);
            var (appContext, instruction) = await app.RunAsync();

            appManager.Received(1).FinalizeAsync(Arg.Any<IAppContext>(), Arg.Any<CancellationToken>());
            var appResult = (IOperationResult)appContext.AppResult;
            Assert.AreEqual(12, appResult.Value);
        }

        [Test]
        public async Task RunAsync_none_instruction_does_not_stop_application_callback()
        {
            var appManager = Substitute.For<IAppManager>();

            var injector = Substitute.For<IServiceProvider>();
            injector.Resolve<IAppManager>()
                .Returns(appManager);

            var app = new TestApp(_ => injector, () => new MainLoopResult(new OperationResult { Value = 23 }, AppShutdownInstruction.Ignore));
            var (appContext, instruction) = await app.RunAsync();

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
                .Returns(new MainLoopResult(new OperationResult { Value = 23 }, AppShutdownInstruction.Ignore));

            var injector = Substitute.For<IServiceProvider>();
            injector.Resolve<IAppManager>()
                .Returns(appManager);
            injector.TryResolve<IAppMainLoop>()
                .Returns(termAwaiter);

            var app = new TestApp(_ => injector);
            var (appContext, instruction) = await app.RunAsync();

            appManager.Received(0).FinalizeAsync(Arg.Any<IAppContext>(), Arg.Any<CancellationToken>());
            var appResult = (IOperationResult)appContext.AppResult;
            Assert.AreEqual(23, appResult.Value);
        }

        [Test]
        public async Task RunAsync_injection()
        {
            var container = this.CreateServicesBuilder()
                .WithParts(typeof(TestApp), typeof(TestMainLoop), typeof(TestShutdownFeatureManager))
                .BuildWithAutofac();
            var app = new TestApp(_ => container);
            var (appContext, instruction) = await app.RunAsync();

            Assert.AreEqual(AppShutdownInstruction.Ignore, instruction);

            await app.ShutdownAsync();
        }

        [Test]
        public async Task ShutdownAsync_appManager_invoked()
        {
            var appManager = Substitute.For<IAppManager>();

            var injector = Substitute.For<IServiceProvider>();
            injector.Resolve<IAppManager>().Returns(appManager);

            var app = new TestApp(_ => injector);
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

            public async Task<MainLoopResult> Main(CancellationToken cancellationToken = default)
            {
                return new MainLoopResult(new OperationResult(), AppShutdownInstruction.Ignore);
            }
        }
    }

    public class TestApp : AppBase
    {
        private readonly Func<IAppServiceCollectionBuilder, IServiceProvider>? servicesProviderBuilder;
        private readonly Func<MainLoopResult>? main;

        public TestApp(
            Func<IAppServiceCollectionBuilder, IServiceProvider> servicesProviderBuilder,
            Func<MainLoopResult>? main = null)
        {
            this.servicesProviderBuilder = servicesProviderBuilder;
            this.main = main;
        }

        protected override IServiceProvider BuildServiceProvider(IAppServiceCollectionBuilder servicesBuilder)
        {
            if (this.servicesProviderBuilder != null)
            {
                return this.servicesProviderBuilder(servicesBuilder);
            }

            throw new InvalidOperationException("No config provided.");
        }

        protected override void ConfigureServices(IAppServiceCollectionBuilder servicesBuilder)
        {
        }

        protected override Task<MainLoopResult> Main(CancellationToken cancellationToken)
        {
            return this.main is not null
                ? Task.FromResult(this.main())
                : base.Main(cancellationToken);
        }

        private static IAppServiceCollection CreateAppServices() =>
            new AppServiceCollection().Add<IRuntimeTypeRegistry>(new RuntimeTypeRegistry(), b => b.ExternallyOwned());
    }
}