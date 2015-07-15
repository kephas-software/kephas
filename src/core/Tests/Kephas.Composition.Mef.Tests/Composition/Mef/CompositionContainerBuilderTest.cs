// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompositionContainerBuilderTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Kephas.Composition.Mef
{
    using System;
    using System.Collections.Generic;
    using System.Composition;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    using Kephas.Composition.AttributedModel;
    using Kephas.Composition.Mef.Conventions;
    using Kephas.Composition.Mef.Hosting;
    using Kephas.Configuration;
    using Kephas.Logging;
    using Kephas.Runtime;
    using Kephas.Services;
    using Kephas.Services.Composition;

    using NUnit.Framework;

    using Telerik.JustMock;

    /// <summary>
    /// Tests for <see cref="CompositionContainerBuilder"/>.
    /// </summary>
    [TestFixture]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public class CompositionContainerBuilderTest : CompositionTestBase
    {
        [Test]
        public async Task CreateContainerAsync_simple_ambient_services_exported()
        {
            var mockLoggerManager = Mock.Create<ILogManager>();
            var mockConfigurationManager = Mock.Create<IConfigurationManager>();
            var mockPlatformManager = Mock.Create<IPlatformManager>();

            Mock.Arrange(() => mockPlatformManager.GetAppAssembliesAsync())
                .Returns(() => Task.FromResult((IEnumerable<Assembly>)new[] { typeof(ILogger).Assembly, typeof(CompositionContainer).Assembly }));

            var factory = new CompositionContainerBuilder(mockLoggerManager, mockConfigurationManager, mockPlatformManager);
            var container = await factory
                .CreateContainerAsync();

            var loggerManager = container.GetExport<ILogManager>();
            Assert.AreEqual(mockLoggerManager, loggerManager);

            var configurationManager = container.GetExport<IConfigurationManager>();
            Assert.AreEqual(mockConfigurationManager, configurationManager);

            var platformManager = container.GetExport<IPlatformManager>();
            Assert.AreEqual(mockPlatformManager, platformManager);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void CreateContainer_assemblies_not_set()
        {
            var mockLoggerManager = Mock.Create<ILogManager>();
            var mockConfigurationManager = Mock.Create<IConfigurationManager>();
            var mockPlatformManager = Mock.Create<IPlatformManager>();

            var factory = new CompositionContainerBuilder(mockLoggerManager, mockConfigurationManager, mockPlatformManager);
            var container = factory.CreateContainer();
        }

        [Test]
        public void CreateContainer_simple_ambient_services_exported()
        {
            var mockLoggerManager = Mock.Create<ILogManager>();
            var mockConfigurationManager = Mock.Create<IConfigurationManager>();
            var mockPlatformManager = Mock.Create<IPlatformManager>();

            var factory = new CompositionContainerBuilder(mockLoggerManager, mockConfigurationManager, mockPlatformManager);
            var container = factory
                .WithAssemblies(new Assembly[0])
                .CreateContainer();

            var loggerManager = container.GetExport<ILogManager>();
            Assert.AreEqual(mockLoggerManager, loggerManager);

            var configurationManager = container.GetExport<IConfigurationManager>();
            Assert.AreEqual(mockConfigurationManager, configurationManager);

            var platformManager = container.GetExport<IPlatformManager>();
            Assert.AreEqual(mockPlatformManager, platformManager);
        }

        [Test]
        public void CreateContainer_composed_loggers_exported()
        {
            var mockLoggerManager = Mock.Create<ILogManager>();
            var mockConfigurationManager = Mock.Create<IConfigurationManager>();
            var mockPlatformManager = Mock.Create<IPlatformManager>();

            Mock.Arrange(() => mockLoggerManager.GetLogger(Arg.IsAny<string>()))
                .Returns(Mock.Create<ILogger>);

            var registrar = new AppServiceConventionsRegistrar();
            var conventionsBuilder = new MefConventionsBuilder();
            registrar.RegisterConventions(
                conventionsBuilder, 
                new[]
                    {
                        typeof(ILogger<>).GetTypeInfo(), 
                        typeof(NullLogger<>).GetTypeInfo(), 
                        typeof(ComposedTestLogConsumer).GetTypeInfo(), 
                    });
            var factory = new CompositionContainerBuilder(mockLoggerManager, mockConfigurationManager, mockPlatformManager);
            var container = factory
                .WithAssembly(typeof(ICompositionContainer).Assembly)
                .WithAssembly(typeof(MefConventionsBuilder).Assembly)
                .WithConventions(conventionsBuilder)
                .WithParts(new[] { typeof(ComposedTestLogConsumer), typeof(NullLogger<>) })
                .CreateContainer();

            var consumer = container.GetExport<ComposedTestLogConsumer>();
            Assert.IsNotNull(consumer.Logger);
        }

        [Test]
        public void CreateContainer_non_composed_loggers_exported()
        {
            var builder = this.CreateCompositionContainerBuilder();
            var container = builder
                .WithAssembly(typeof(MefConventionsBuilder).Assembly)
                .CreateContainer();

            var consumer = new NonComposedTestLogConsumer();
            container.SatisfyImports(consumer);

            Assert.IsNotNull(consumer.Logger);
        }

        [Test]
        public void GetExport_AppService_Shared()
        {
            var builder = this.CreateCompositionContainerBuilder();
            var container = builder
                .WithAssembly(typeof(ICompositionContainer).Assembly)
                .WithParts(new[] { typeof(ITestAppService), typeof(TestAppService) })
                .CreateContainer();

            var exported = container.GetExport<ITestAppService>();
            var secondExported = container.GetExport<ITestAppService>();

            Assert.AreSame(exported, secondExported);
        }

        [Test]
        public void GetExport_AppService_Single_Success()
        {
            var builder = this.CreateCompositionContainerBuilder();
            var container = builder
                .WithAssembly(typeof(ICompositionContainer).Assembly)
                .WithParts(new[] { typeof(ITestAppService), typeof(TestAppService) })
                .CreateContainer();

            var exported = container.GetExport<ITestAppService>();

            Assert.IsInstanceOf<TestAppService>(exported);
        }

        [Test]
        public void GetExport_AppService_Single_Override_Success()
        {
            var builder = this.CreateCompositionContainerBuilder();
            var container = builder
                .WithAssembly(typeof(ICompositionContainer).Assembly)
                .WithParts(new[] { typeof(ITestAppService), typeof(TestAppService), typeof(TestOverrideAppService) })
                .CreateContainer();

            var exported = container.GetExport<ITestAppService>();

            Assert.IsInstanceOf<TestOverrideAppService>(exported);
        }

        [Test]
        public void GetExports_AppService_Multiple_Shared()
        {
            var builder = this.CreateCompositionContainerBuilder();
            var container = builder
                .WithAssembly(typeof(ICompositionContainer).Assembly)
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
            var builder = this.CreateCompositionContainerBuilder();
            var container = builder
                .WithAssembly(typeof(ICompositionContainer).Assembly)
                .WithParts(new[] { typeof(ITestMultiAppService), typeof(TestMultiAppService1), typeof(TestMultiAppService2) })
                .CreateContainer();

            var exports = container.GetExports<ITestMultiAppService>().ToList();

            Assert.AreEqual(2, exports.Count);
        }

        [Test]
        public void SatisfyImports_AppService_metadata()
        {
            var builder = this.CreateCompositionContainerBuilder();
            var container = builder
                .WithAssembly(typeof(ICompositionContainer).Assembly)
                .WithParts(new[] { typeof(ITestAppService), typeof(TestAppService), typeof(TestOverrideAppService) })
                .CreateContainer();

            var importer = new TestMetadataConsumer();
            container.SatisfyImports(importer);

            Assert.IsNotNull(importer.TestServices);
            var metadata = importer.TestServices.First().Metadata;
            Assert.AreEqual(Priority.High, metadata.OverridePriority);
        }

        [Test]
        public void SatisfyImports_AppService_metadata_complex()
        {
            var builder = this.CreateCompositionContainerBuilder();
            var container = builder
                .WithAssembly(typeof(ICompositionContainer).Assembly)
                .WithParts(new[] { typeof(IConverter), typeof(IConverter<,>), typeof(StringToIntConverter), typeof(SecondStringToIntConverter) })
                .CreateContainer();

            var importer = new TestConverterConsumer();
            container.SatisfyImports(importer);

            Assert.IsNotNull(importer.Converters);
            var orderedConverters = importer.Converters.ToList();
            orderedConverters.Sort((f1, f2) => f1.Metadata.ProcessingPriority - f2.Metadata.ProcessingPriority);

            Assert.AreEqual(2, orderedConverters.Count);
            Assert.AreEqual(0, orderedConverters[0].Metadata.ProcessingPriority);
            Assert.AreEqual(100, orderedConverters[1].Metadata.ProcessingPriority);
        }

        [Test]
        public void GetExport_AppService_generic_export()
        {
            var builder = this.CreateCompositionContainerBuilder();
            var container = builder
                .WithAssembly(typeof(ICompositionContainer).Assembly)
                .WithParts(new[] { typeof(ITestGenericExport<>), typeof(TestGenericExport) })
                .CreateContainer();

            var export = container.GetExport<ITestGenericExport<string>>();
            Assert.IsInstanceOf<TestGenericExport>(export);
        }

        [Test]
        [ExpectedException(typeof(CompositionException))]
        public void GetExport_AppService_no_constructor()
        {
            var builder = this.CreateCompositionContainerBuilder();
            var container = builder
                .WithAssembly(typeof(ICompositionContainer).Assembly)
                .WithParts(new[] { typeof(IConstructorAppService), typeof(NoCompositionConstructorAppService) })
                .CreateContainer();

            var export = container.GetExport<IConstructorAppService>();
        }

        [Test]
        [ExpectedException(typeof(CompositionException))]
        public void GetExport_AppService_multiple_constructor()
        {
            var builder = this.CreateCompositionContainerBuilder();
            var container = builder
                .WithAssembly(typeof(ICompositionContainer).Assembly)
                .WithParts(new[] { typeof(IConstructorAppService), typeof(MultipleCompositionConstructorAppService) })
                .CreateContainer();

            var export = container.GetExport<IConstructorAppService>();
        }

        [Test]
        public void GetExport_AppService_default_constructor()
        {
            var builder = this.CreateCompositionContainerBuilder();
            var container = builder
                .WithAssembly(typeof(ICompositionContainer).Assembly)
                .WithParts(new[] { typeof(IConstructorAppService), typeof(DefaultConstructorAppService) })
                .CreateContainer();

            var export = container.GetExport<IConstructorAppService>();

            Assert.IsInstanceOf<DefaultConstructorAppService>(export);
        }

        [Test]
        public void GetExport_AppService_single_constructor()
        {
            var builder = this.CreateCompositionContainerBuilder();
            var container = builder
                .WithAssembly(typeof(ICompositionContainer).Assembly)
                .WithParts(new[] { typeof(IConstructorAppService), typeof(SingleConstructorAppService) })
                .CreateContainer();

            var export = container.GetExport<IConstructorAppService>();

            Assert.IsInstanceOf<SingleConstructorAppService>(export);
        }

        private CompositionContainerBuilder CreateCompositionContainerBuilder()
        {
            var mockLoggerManager = Mock.Create<ILogManager>();
            var mockConfigurationManager = Mock.Create<IConfigurationManager>();
            var mockPlatformManager = Mock.Create<IPlatformManager>();

            Mock.Arrange(() => mockLoggerManager.GetLogger(Arg.IsAny<string>()))
                .Returns(Mock.Create<ILogger>);

            var factory = new CompositionContainerBuilder(mockLoggerManager, mockConfigurationManager, mockPlatformManager);
            return factory;
        }

        [Export]
        public class ComposedTestLogConsumer
        {
            /// <summary>
            /// Gets or sets the logger.
            /// </summary>
            public ILogger<ComposedTestLogConsumer> Logger { get; set; }
        }

        public class NonComposedTestLogConsumer
        {
            /// <summary>
            /// Gets or sets the logger.
            /// </summary>
            [Import]
            public ILogger<NonComposedTestLogConsumer> Logger { get; set; }
        }

        [SharedAppServiceContract(MetadataAttributes = new[] { typeof(OverridePriorityAttribute) })]
        public interface ITestAppService { }

        [SharedAppServiceContract(AllowMultiple = true)]
        public interface ITestMultiAppService { }

        public class TestAppService : ITestAppService { }

        [OverridePriority(Priority.High)]
        public class TestOverrideAppService : ITestAppService { }

        public class TestMultiAppService1 : ITestMultiAppService { }

        public class TestMultiAppService2 : ITestMultiAppService { }

        public class TestMetadataConsumer
        {
            /// <summary>
            /// Gets or sets the test services.
            /// </summary>
            [ImportMany]
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
            [ImportMany]
            public ICollection<ExportFactoryAdapter<IConverter, AppServiceMetadata>> Converters { get; set; }
        }

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
            public SingleConstructorAppService(ICompositionContainer compositionContainer)
            {
            }
        }

        public class NoCompositionConstructorAppService : IConstructorAppService
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="NoCompositionConstructorAppService"/> class.
            /// </summary>
            public NoCompositionConstructorAppService()
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="NoCompositionConstructorAppService"/> class.
            /// </summary>
            /// <param name="compositionContainer">
            /// The composition container.
            /// </param>
            public NoCompositionConstructorAppService(ICompositionContainer compositionContainer)
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
            public MultipleCompositionConstructorAppService(ICompositionContainer compositionContainer)
            {
            }
        }
    }
}
