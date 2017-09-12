// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MefCompositionContainerBuilderTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Tests.Composition.Mef
{
    using System;
    using System.Collections.Generic;
    using System.Composition;
    using System.Composition.Hosting;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Composition;
    using Kephas.Composition.AttributedModel;
    using Kephas.Composition.Hosting;
    using Kephas.Composition.Mef;
    using Kephas.Composition.Mef.Conventions;
    using Kephas.Composition.Mef.Hosting;
    using Kephas.Composition.Mef.ScopeFactory;
    using Kephas.Configuration;
    using Kephas.Logging;
    using Kephas.Services;
    using Kephas.Services.Composition;
    using Kephas.Testing.Composition.Mef;

    using NSubstitute;

    using NUnit.Framework;

    /// <summary>
    /// Tests for <see cref="MefCompositionContainerBuilder"/>.
    /// </summary>
    [TestFixture]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public class MefCompositionContainerBuilderTest : CompositionTestBase
    {
        [Test]
        public async Task CreateContainerAsync_simple_ambient_services_exported()
        {
            var factory = this.CreateCompositionContainerBuilder();
            var mockPlatformManager = factory.AppRuntime;


            mockPlatformManager.GetAppAssembliesAsync(Arg.Any<Func<AssemblyName, bool>>(), CancellationToken.None)
                .Returns(Task.FromResult((IEnumerable<Assembly>)new[] { typeof(ILogger).GetTypeInfo().Assembly, typeof(MefCompositionContainer).GetTypeInfo().Assembly }));

            var container = await factory
                .CreateContainerAsync();

            var loggerManager = container.GetExport<ILogManager>();
            Assert.AreEqual(factory.LogManager, loggerManager);

            var configuration = container.GetExport<IAppConfiguration>();
            Assert.AreEqual(factory.AppConfiguration, configuration);

            var platformManager = container.GetExport<IAppRuntime>();
            Assert.AreEqual(mockPlatformManager, platformManager);
        }

        [Test]
        public void CreateContainer_assemblies_not_set()
        {
            var factory = this.CreateCompositionContainerBuilder();
            Assert.Throws<InvalidOperationException>(() => factory.CreateContainer());
        }

        [Test]
        public void CreateContainer_simple_ambient_services_exported()
        {
            var factory = this.CreateCompositionContainerBuilder();
            var container = factory
                .WithAssemblies(new Assembly[0])
                .CreateContainer();

            var loggerManager = container.GetExport<ILogManager>();
            Assert.AreEqual(factory.LogManager, loggerManager);

            var configuration = container.GetExport<IAppConfiguration>();
            Assert.AreEqual(factory.AppConfiguration, configuration);

            var platformManager = container.GetExport<IAppRuntime>();
            Assert.AreEqual(factory.AppRuntime, platformManager);
        }

        [Test]
        public void CreateContainer_composed_loggers_exported()
        {
            var builder = this.CreateCompositionContainerBuilderWithStringLogger();
            var container = builder
                .WithAssembly(typeof(ICompositionContext).GetTypeInfo().Assembly)
                .WithAssembly(typeof(MefConventionsBuilder).GetTypeInfo().Assembly)
                .CreateContainer();

            var logger = container.GetExport<ILogger<MefCompositionContainerTest.ExportedClass>>();
            Assert.IsInstanceOf<TypedLogger<MefCompositionContainerTest.ExportedClass>>(logger);
        }

        [Test]
        public void GetExport_AppService_Shared()
        {
            var builder = this.CreateCompositionContainerBuilderWithStringLogger();
            var container = builder
                .WithAssembly(typeof(ICompositionContext).GetTypeInfo().Assembly)
                .WithParts(new[] { typeof(ITestAppService), typeof(TestAppService) })
                .CreateContainer();

            var exported = container.GetExport<ITestAppService>();
            var secondExported = container.GetExport<ITestAppService>();

            Assert.AreSame(exported, secondExported);
        }

        [Test]
        public void GetExport_AppService_Single_Success()
        {
            var builder = this.CreateCompositionContainerBuilderWithStringLogger();
            var container = builder
                .WithAssembly(typeof(ICompositionContext).GetTypeInfo().Assembly)
                .WithParts(new[] { typeof(ITestAppService), typeof(TestAppService) })
                .CreateContainer();

            var exported = container.GetExport<ITestAppService>();

            Assert.IsInstanceOf<TestAppService>(exported);
        }

        [Test]
        public void GetExport_AppService_Single_Override_Success()
        {
            var builder = this.CreateCompositionContainerBuilderWithStringLogger();
            var container = builder
                .WithAssembly(typeof(ICompositionContext).GetTypeInfo().Assembly)
                .WithParts(new[] { typeof(ITestAppService), typeof(TestAppService), typeof(TestOverrideAppService) })
                .CreateContainer();

            var exported = container.GetExport<ITestAppService>();

            Assert.IsInstanceOf<TestOverrideAppService>(exported);
        }

        [Test]
        public void GetExports_AppService_Multiple_Shared()
        {
            var builder = this.CreateCompositionContainerBuilderWithStringLogger();
            var container = builder
                .WithAssembly(typeof(ICompositionContext).GetTypeInfo().Assembly)
                .WithParts(new[] { typeof(ITestMultiAppService), typeof(TestMultiAppService1), typeof(TestMultiAppService2) })
                .CreateContainer();

            var exports = container.GetExports<ITestMultiAppService>().ToList();
            var exports2 = container.GetExports<ITestMultiAppService>().ToList();

            Assert.AreSame(exports[0], exports2[0]);
            Assert.AreSame(exports[1], exports2[1]);
        }

        [Test]
        public void GetExports_AppService_Multiple_Success()
        {
            var builder = this.CreateCompositionContainerBuilderWithStringLogger();
            var container = builder
                .WithAssembly(typeof(ICompositionContext).GetTypeInfo().Assembly)
                .WithParts(new[] { typeof(ITestMultiAppService), typeof(TestMultiAppService1), typeof(TestMultiAppService2) })
                .CreateContainer();

            var exports = container.GetExports<ITestMultiAppService>().ToList();

            Assert.AreEqual(2, exports.Count);
        }

        [Test]
        public void GetExports_AppService_IExportFactory_Success()
        {
            var builder = this.CreateCompositionContainerBuilderWithStringLogger();
            var container = builder
                .WithAssembly(typeof(ICompositionContext).GetTypeInfo().Assembly)
                .WithParts(new[] { typeof(ITestMultiAppService), typeof(ITestMultiAppServiceConsumer), typeof(TestMultiAppService1), typeof(TestMultiAppService2), typeof(TestMultiAppServiceConsumer) })
                .CreateContainer();

            var export = (TestMultiAppServiceConsumer)container.GetExport<ITestMultiAppServiceConsumer>();

            Assert.AreEqual(2, export.Factories.Count());
            Assert.AreEqual(2, export.MetadataFactories.Count());
        }

        [Test]
        public void GetExport_AppService_generic_export()
        {
            var builder = this.CreateCompositionContainerBuilderWithStringLogger();
            var container = builder
                .WithAssembly(typeof(ICompositionContext).GetTypeInfo().Assembly)
                .WithParts(new[] { typeof(ITestGenericExport<>), typeof(TestGenericExport) })
                .CreateContainer();

            var export = container.GetExport<ITestGenericExport<string>>();
            Assert.IsInstanceOf<TestGenericExport>(export);
        }

        [Test]
        public void GetExport_ScopeSharedAppService_no_scope()
        {
            var builder = this.CreateCompositionContainerBuilderWithStringLogger();
            var container = builder
                .WithAssembly(typeof(ICompositionContext).GetTypeInfo().Assembly)
                .WithParts(new[] { typeof(ITestScopedExport), typeof(TestScopedExport) })
                .CreateContainer();

            Assert.Throws<CompositionFailedException>(() => container.GetExport<ITestScopedExport>());
        }

        [Test]
        public void GetExport_ScopeSharedAppService_export()
        {
            var builder = this.CreateCompositionContainerBuilderWithStringLogger();
            var container = builder
                .WithAssembly(typeof(ICompositionContext).GetTypeInfo().Assembly)
                .WithParts(new[] { typeof(ITestScopedExport), typeof(TestScopedExport) })
                .CreateContainer();

            ITestScopedExport exportScope1;
            using (var scopedContext = container.CreateScopedContext())
            {
                exportScope1 = scopedContext.GetExport<ITestScopedExport>();
                Assert.IsInstanceOf<TestScopedExport>(exportScope1);

                var export = scopedContext.GetExport<ITestScopedExport>();
                Assert.AreSame(exportScope1, export);
            }

            using (var scopedContext2 = container.CreateScopedContext())
            {
                var export2 = scopedContext2.GetExport<ITestScopedExport>();
                Assert.AreNotSame(exportScope1, export2);
            }
        }

        [Test]
        public void GetExport_ScopeSharedAppService_custom_scope_export()
        {
            var builder = this.CreateCompositionContainerBuilderWithStringLogger();
            var container = builder
                .WithAssembly(typeof(ICompositionContext).GetTypeInfo().Assembly)
                .WithParts(new[] { typeof(ITestMyScopedExport), typeof(TestMyScopedExport) })
                .WithScopeFactory<MyScopeFactory>()
                .CreateContainer();

            ITestMyScopedExport exportScope1;
            using (var scopedContext = container.CreateScopedContext("my-scope"))
            {
                exportScope1 = scopedContext.GetExport<ITestMyScopedExport>();
                Assert.IsInstanceOf<TestMyScopedExport>(exportScope1);

                var export = scopedContext.GetExport<ITestMyScopedExport>();
                Assert.AreSame(exportScope1, export);
            }

            using (var scopedContext2 = container.CreateScopedContext("my-scope"))
            {
                var export2 = scopedContext2.GetExport<ITestMyScopedExport>();
                Assert.AreNotSame(exportScope1, export2);
            }
        }

        [Test]
        public void GetExport_ScopeSharedAppService_scopefactory_composed_only_once()
        {
            var builder = this.CreateCompositionContainerBuilderWithStringLogger();
            var container = builder
                .WithAssembly(typeof(ICompositionContext).GetTypeInfo().Assembly)
                .WithParts(new[] { typeof(ITestMyScopedExport), typeof(TestMyScopedExport), typeof(MyScopeFactory) })
                .WithScopeFactory<MyScopeFactory>()
                .CreateContainer();

            var scopedContext = container.CreateScopedContext("my-scope");
            Assert.AreNotSame(container, scopedContext);
            Assert.IsNotNull(scopedContext);
        }

        [Test]
        public void GetExport_AppService_no_constructor()
        {
            var builder = this.CreateCompositionContainerBuilderWithStringLogger();
            Assert.Throws<CompositionException>(() => builder
                .WithAssembly(typeof(ICompositionContext).GetTypeInfo().Assembly)
                .WithParts(new[] { typeof(IConstructorAppService), typeof(NoCompositionConstructorAppService) })
                .CreateContainer());
        }

        [Test]
        public void GetExport_AppService_ambiguous_constructor()
        {
            var builder = this.CreateCompositionContainerBuilderWithStringLogger();
            Assert.Throws<CompositionException>(() => builder
                .WithAssembly(typeof(ICompositionContext).GetTypeInfo().Assembly)
                .WithParts(new[] { typeof(IConstructorAppService), typeof(AmbiguousCompositionConstructorAppService) })
                .CreateContainer());
        }

        [Test]
        public void GetExport_AppService_largest_constructor()
        {
            var builder = this.CreateCompositionContainerBuilderWithStringLogger();
            var container = builder
                .WithAssembly(typeof(ICompositionContext).GetTypeInfo().Assembly)
                .WithParts(new[] { typeof(IConstructorAppService), typeof(LargestCompositionConstructorAppService) })
                .CreateContainer();

            var component = container.GetExport<IConstructorAppService>();
            Assert.IsNotNull(component);
        }

        [Test]
        public void GetExport_AppService_multiple_constructor()
        {
            var builder = this.CreateCompositionContainerBuilderWithStringLogger();
            Assert.Throws<CompositionException>(() => builder
                .WithAssembly(typeof(ICompositionContext).GetTypeInfo().Assembly)
                .WithParts(new[] { typeof(IConstructorAppService), typeof(MultipleCompositionConstructorAppService) })
                .CreateContainer());
        }

        [Test]
        public void GetExport_AppService_default_constructor()
        {
            var builder = this.CreateCompositionContainerBuilderWithStringLogger();
            var container = builder
                .WithAssembly(typeof(ICompositionContext).GetTypeInfo().Assembly)
                .WithParts(new[] { typeof(IConstructorAppService), typeof(DefaultConstructorAppService) })
                .CreateContainer();

            var export = container.GetExport<IConstructorAppService>();

            Assert.IsInstanceOf<DefaultConstructorAppService>(export);
        }

        [Test]
        public void GetExport_AppService_single_constructor()
        {
            var builder = this.CreateCompositionContainerBuilderWithStringLogger();
            var container = builder
                .WithAssembly(typeof(ICompositionContext).GetTypeInfo().Assembly)
                .WithParts(new[] { typeof(IConstructorAppService), typeof(SingleConstructorAppService) })
                .CreateContainer();

            var export = container.GetExport<IConstructorAppService>();

            Assert.IsInstanceOf<SingleConstructorAppService>(export);
        }

        private MefCompositionContainerBuilder CreateCompositionContainerBuilder()
        {
            var mockLoggerManager = Substitute.For<ILogManager>();
            var mockConfiguration = Substitute.For<IAppConfiguration>();
            var mockPlatformManager = Substitute.For<IAppRuntime>();

            var context = new CompositionRegistrationContext(new AmbientServices()
                                        .RegisterService(mockLoggerManager)
                                        .RegisterService(mockConfiguration)
                                        .RegisterService(mockPlatformManager));
            var factory = new MefCompositionContainerBuilder(context);
            return factory;
        }

        private MefCompositionContainerBuilder CreateCompositionContainerBuilderWithStringLogger()
        {
            var builder = this.CreateCompositionContainerBuilder();

            var mockLoggerManager = builder.LogManager;
            mockLoggerManager.GetLogger(Arg.Any<string>()).Returns(Substitute.For<ILogger>());

            return builder;
        }

        [Export]
        public class ComposedTestLogConsumer
        {
            public ComposedTestLogConsumer(ILogger<MefCompositionContainerBuilderTest.ComposedTestLogConsumer> logger)
            {
                this.Logger = logger;
            }

            /// <summary>
            /// Gets or sets the logger.
            /// </summary>
            public ILogger<ComposedTestLogConsumer> Logger { get; private set; }
        }

        public class NonComposedTestLogConsumer
        {
            /// <summary>
            /// Gets or sets the logger.
            /// </summary>
            [Kephas.Composition.AttributedModel.Import]
            public ILogger<NonComposedTestLogConsumer> Logger { get; set; }
        }

        [SharedAppServiceContract(MetadataAttributes = new[] { typeof(OverridePriorityAttribute) })]
        public interface ITestAppService { }

        [SharedAppServiceContract(AllowMultiple = true)]
        public interface ITestMultiAppService { }

        [SharedAppServiceContract]
        public interface ITestMultiAppServiceConsumer { }

        public class TestAppService : ITestAppService { }

        [OverridePriority(Priority.High)]
        public class TestOverrideAppService : ITestAppService { }

        public class TestMultiAppService1 : ITestMultiAppService { }

        public class TestMultiAppService2 : ITestMultiAppService { }

        public class TestMultiAppServiceConsumer : ITestMultiAppServiceConsumer
        {
            public IEnumerable<IExportFactory<ITestMultiAppService>> Factories { get; set; }

            public IEnumerable<IExportFactory<ITestMultiAppService, AppServiceMetadata>> MetadataFactories { get; set; }
        }

        public class TestMetadataConsumer
        {
            /// <summary>
            /// Gets or sets the test services.
            /// </summary>
            [Kephas.Composition.AttributedModel.ImportMany]
            public ICollection<ExportFactoryAdapter<ITestAppService, AppServiceMetadata>> TestServices { get; set; }
        }

        public interface IConverter { }

        [SharedAppServiceContract(MetadataAttributes = new[] { typeof(ProcessingPriorityAttribute) }, 
            ContractType = typeof(IConverter))]
        public interface IConverter<TSource, TTarget> : IConverter { }

        [ProcessingPriority(100)]
        public class StringToIntConverter : IConverter<string, int> { }

        public class SecondStringToIntConverter : IConverter<string, int> { }

        public class TestConverterConsumer
        {
            /// <summary>
            /// Gets or sets the converters.
            /// </summary>
            [Kephas.Composition.AttributedModel.ImportMany]
            public ICollection<ExportFactoryAdapter<IConverter, AppServiceMetadata>> Converters { get; set; }
        }

        [ScopeSharedAppServiceContract("my-scope")]
        public interface ITestMyScopedExport { }

        public class TestMyScopedExport : ITestMyScopedExport { }

        [SharingBoundaryScope("my-scope")]
        public class MyScopeFactory : MefScopeFactoryBase
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="MefScopeFactoryBase"/> class.
            /// </summary>
            /// <param name="scopedContextFactory">The scoped context factory.</param>
            public MyScopeFactory([SharingBoundary("my-scope")] ExportFactory<CompositionContext> scopedContextFactory)
                : base(scopedContextFactory)
            {
            }
        }

        [ScopeSharedAppServiceContract]
        public interface ITestScopedExport { }

        public class TestScopedExport : ITestScopedExport { }

        [SharedAppServiceContract]
        public interface ITestGenericExport<T> { }

        public class TestGenericExport : ITestGenericExport<string> { }

        [SharedAppServiceContract]
        public interface IConstructorAppService { }

        public class DefaultConstructorAppService : IConstructorAppService
        {

        }

        public class SingleConstructorAppService : IConstructorAppService
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="SingleConstructorAppService"/> class.
            /// </summary>
            /// <param name="compositionContainer">
            /// The composition container.
            /// </param>
            public SingleConstructorAppService(ICompositionContext compositionContainer)
            {
            }
        }

        public class AmbiguousCompositionConstructorAppService : IConstructorAppService
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="AmbiguousCompositionConstructorAppService"/>
            /// class.
            /// </summary>
            /// <param name="ambientServices">The ambient services.</param>
            public AmbiguousCompositionConstructorAppService(IAmbientServices ambientServices)
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="AmbiguousCompositionConstructorAppService"/> class.
            /// </summary>
            /// <param name="compositionContainer">
            /// The composition container.
            /// </param>
            public AmbiguousCompositionConstructorAppService(ICompositionContext compositionContainer)
            {
            }
        }

        public class LargestCompositionConstructorAppService : IConstructorAppService
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="LargestCompositionConstructorAppService"/> class.
            /// </summary>
            public LargestCompositionConstructorAppService()
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="LargestCompositionConstructorAppService"/> class.
            /// </summary>
            /// <param name="compositionContainer">
            /// The composition container.
            /// </param>
            public LargestCompositionConstructorAppService(ICompositionContext compositionContainer)
            {
            }
        }

        public class NoCompositionConstructorAppService : IConstructorAppService
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="NoCompositionConstructorAppService"/> class.
            /// </summary>
            private NoCompositionConstructorAppService()
            {
            }
        }

        public class MultipleCompositionConstructorAppService : IConstructorAppService
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="MultipleCompositionConstructorAppService"/> class.
            /// </summary>
            [CompositionConstructor]
            public MultipleCompositionConstructorAppService()
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="MultipleCompositionConstructorAppService"/> class.
            /// </summary>
            /// <param name="compositionContainer">
            /// The composition container.
            /// </param>
            [CompositionConstructor]
            public MultipleCompositionConstructorAppService(ICompositionContext compositionContainer)
            {
            }
        }
    }
}
