// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompositionContainerBuilderTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Tests for <see cref="CompositionContainerBuilder" />
// </summary>
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

    using Kephas.Configuration;
    using Kephas.Logging;
    using Kephas.Logging.Composition;
    using Kephas.Runtime;
    using Kephas.Services;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Telerik.JustMock;

    /// <summary>
    /// Tests for <see cref="CompositionContainerBuilder"/>
    /// </summary>
    [TestClass]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public class CompositionContainerBuilderTest : CompositionTestBase
    {
        [TestMethod]
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

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void CreateContainer_assemblies_not_set()
        {
            var mockLoggerManager = Mock.Create<ILogManager>();
            var mockConfigurationManager = Mock.Create<IConfigurationManager>();
            var mockPlatformManager = Mock.Create<IPlatformManager>();

            var factory = new CompositionContainerBuilder(mockLoggerManager, mockConfigurationManager, mockPlatformManager);
            var container = factory.CreateContainer();
        }

        [TestMethod]
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

        [TestMethod]
        public void CreateContainer_composed_loggers_exported()
        {
            var mockLoggerManager = Mock.Create<ILogManager>();
            var mockConfigurationManager = Mock.Create<IConfigurationManager>();
            var mockPlatformManager = Mock.Create<IPlatformManager>();

            Mock.Arrange(() => mockLoggerManager.GetLogger(Arg.IsAny<string>()))
                .Returns(Mock.Create<ILogger>);

            var factory = new CompositionContainerBuilder(mockLoggerManager, mockConfigurationManager, mockPlatformManager);
            var container = factory
                .WithAssembly(typeof(LogConventionsRegistrar).Assembly)
                .WithPart(typeof(ComposedTestLogConsumer))
                .CreateContainer();

            var consumer = container.GetExport<ComposedTestLogConsumer>();
            Assert.IsNotNull(consumer.Logger);
        }

        [TestMethod]
        public void CreateContainer_non_composed_loggers_exported()
        {
            var builder = this.CreateCompositionContainerBuilder();
            var container = builder
                .WithAssembly(typeof(LogConventionsRegistrar).Assembly)
                .CreateContainer();

            var consumer = new NonComposedTestLogConsumer();
            container.SatisfyImports(consumer);

            Assert.IsNotNull(consumer.Logger);
        }

        [TestMethod]
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

        [TestMethod]
        public void GetExport_AppService_Single_Success()
        {
            var builder = this.CreateCompositionContainerBuilder();
            var container = builder
                .WithAssembly(typeof(ICompositionContainer).Assembly)
                .WithParts(new[] { typeof(ITestAppService), typeof(TestAppService) })
                .CreateContainer();

            var exported = container.GetExport<ITestAppService>();

            Assert.IsInstanceOfType(exported, typeof(TestAppService));
        }

        [TestMethod]
        public void GetExport_AppService_Single_Override_Success()
        {
            var builder = this.CreateCompositionContainerBuilder();
            var container = builder
                .WithAssembly(typeof(ICompositionContainer).Assembly)
                .WithParts(new[] { typeof(ITestAppService), typeof(TestAppService), typeof(TestOverrideAppService) })
                .CreateContainer();

            var exported = container.GetExport<ITestAppService>();

            Assert.IsInstanceOfType(exported, typeof(TestOverrideAppService));
        }

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
        public void GetExport_AppService_generic_export()
        {
            var builder = this.CreateCompositionContainerBuilder();
            var container = builder
                .WithAssembly(typeof(ICompositionContainer).Assembly)
                .WithParts(new[] { typeof(ITestGenericExport<>), typeof(TestGenericExport<>) })
                .CreateContainer();

            var export = container.GetExport<ITestGenericExport<string>>();
            Assert.IsInstanceOfType(export, typeof(TestGenericExport<string>));
        }

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
        public void GetExport_AppService_default_constructor()
        {
            var builder = this.CreateCompositionContainerBuilder();
            var container = builder
                .WithAssembly(typeof(ICompositionContainer).Assembly)
                .WithParts(new[] { typeof(IConstructorAppService), typeof(DefaultConstructorAppService) })
                .CreateContainer();

            var export = container.GetExport<IConstructorAppService>();

            Assert.IsInstanceOfType(export, typeof(DefaultConstructorAppService));
        }

        [TestMethod]
        public void GetExport_AppService_single_constructor()
        {
            var builder = this.CreateCompositionContainerBuilder();
            var container = builder
                .WithAssembly(typeof(ICompositionContainer).Assembly)
                .WithParts(new[] { typeof(IConstructorAppService), typeof(SingleConstructorAppService) })
                .CreateContainer();

            var export = container.GetExport<IConstructorAppService>();

            Assert.IsInstanceOfType(export, typeof(SingleConstructorAppService));
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
        public class ComposedTestLogConsumer : ILogConsumer
        {
            public ILogger Logger { get; set; }
        }

        public class NonComposedTestLogConsumer
        {
            [Import]
            public ILogManager LogManager { get; set; }

            public ILogger Logger
            {
                get
                {
                    return this.LogManager.GetLogger(this.GetType());
                }
            }
        }

        [AppServiceContract(MetadataAttributes = new[] { typeof(OverridePriorityAttribute) })]
        public interface ITestAppService { }

        [AppServiceContract(AllowMultiple = true)]
        public interface ITestMultiAppService { }

        public class TestAppService : ITestAppService { }

        [OverridePriority(Priority.High)]
        public class TestOverrideAppService : ITestAppService { }

        public class TestMultiAppService1 : ITestMultiAppService { }

        public class TestMultiAppService2 : ITestMultiAppService { }

        public class TestMetadataConsumer
        {
            [ImportMany]
            public ICollection<ExportFactory<ITestAppService, AppServiceMetadata>> TestServices { get; set; }
        }

        public interface IConverter { }

        [AppServiceContract(MetadataAttributes = new[] { typeof(ProcessingPriorityAttribute) },
            ContractType = typeof(IConverter))]
        public interface IConverter<TSource, TTarget> : IConverter { }

        [ProcessingPriority(100)]
        public class StringToIntConverter : IConverter<string, int> { }

        public class SecondStringToIntConverter : IConverter<string, int> { }

        public class TestConverterConsumer
        {
            [ImportMany]
            public ICollection<ExportFactory<IConverter, AppServiceMetadata>> Converters { get; set; }
        }

        [AppServiceContract]
        public interface ITestGenericExport<T> { }

        public class TestGenericExport<T> : ITestGenericExport<T> { }

        [AppServiceContract]
        public interface IConstructorAppService { }

        public class DefaultConstructorAppService : IConstructorAppService
        {

        }

        public class SingleConstructorAppService : IConstructorAppService
        {
            public SingleConstructorAppService(ICompositionContainer compositionContainer)
            {
            }
        }

        public class NoCompositionConstructorAppService : IConstructorAppService
        {
            public NoCompositionConstructorAppService()
            {
            }

            public NoCompositionConstructorAppService(ICompositionContainer compositionContainer)
            {
            }
        }

        public class MultipleCompositionConstructorAppService : IConstructorAppService
        {
            [CompositionConstructor]
            public MultipleCompositionConstructorAppService()
            {
            }

            [CompositionConstructor]
            public MultipleCompositionConstructorAppService(ICompositionContainer compositionContainer)
            {
            }
        }
    }
}
