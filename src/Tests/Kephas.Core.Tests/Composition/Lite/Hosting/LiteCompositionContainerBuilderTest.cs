// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LiteCompositionContainerBuilderTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the lite composition container builder test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests.Composition.Lite.Hosting
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Kephas.Application;
    using Kephas.Collections;
    using Kephas.Composition;
    using Kephas.Composition.AttributedModel;
    using Kephas.Composition.Hosting;
    using Kephas.Composition.Internal;
    using Kephas.Composition.Lite.Hosting;
    using Kephas.Cryptography;
    using Kephas.Logging;
    using Kephas.Reflection;
    using Kephas.Services;
    using Kephas.Services.Composition;
    using Kephas.Services.Reflection;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class LiteCompositionContainerBuilderTest
    {
        [Test]
        public void BuildLiteCompositionContainer_app_manager()
        {
            var ambientServices = new AmbientServices()
                .WithStaticAppRuntime(an => !this.IsTestAssembly(an));

            ambientServices.BuildLiteCompositionContainer();

            Assert.IsInstanceOf<CompositionContextAdapter>(ambientServices.CompositionContainer);

            var appManager = ambientServices.CompositionContainer.GetExport<IEncryptionService>();
            Assert.IsInstanceOf<AesEncryptionService>(appManager);
        }

        [Test]
        public void BuildLiteCompositionContainer_closed_generic()
        {
            var ambientServices = new AmbientServices()
                .WithStaticAppRuntime(this.IsAppAssembly);

            ambientServices.BuildLiteCompositionContainer(b => b.WithParts(new[] { typeof(IGeneric<>), typeof(IntGeneric) }));

            var service = ambientServices.GetService(typeof(IGeneric<int>));
            Assert.IsInstanceOf<IntGeneric>(service);
        }

        [Test]
        public void BuildLiteCompositionContainer_closed_generic_dependency()
        {
            var ambientServices = new AmbientServices()
                .WithStaticAppRuntime(this.IsAppAssembly);

            ambientServices.BuildLiteCompositionContainer(b => b.WithParts(new[] { typeof(IGeneric<>), typeof(IntGeneric), typeof(IntGenericDepedent) }));

            var service = ambientServices.GetService<IntGenericDepedent>();
            Assert.IsNotNull(service);
        }

        [Test]
        public void BuildLiteCompositionContainer_closed_generic_with_non_generic_contract_metadata()
        {
            var ambientServices = new AmbientServices()
                .WithStaticAppRuntime(this.IsAppAssembly);

            ambientServices.BuildLiteCompositionContainer(b => b.WithParts(new[] { typeof(IGenericSvc<>), typeof(INonGenericSvc), typeof(IntGenericSvc) }));

            var serviceFactory = ambientServices.GetService<IExportFactory<INonGenericSvc, GenericSvcMetadata>>();
            Assert.AreSame(typeof(int), serviceFactory.Metadata.ServiceType);
        }

        [Test]
        public void BuildLiteCompositionContainer_disposable_closed_generic_with_non_generic_contract_metadata()
        {
            var ambientServices = new AmbientServices()
                .WithStaticAppRuntime(this.IsAppAssembly);

            ambientServices.BuildLiteCompositionContainer(b => b.WithParts(new[] { typeof(IGenericSvc<>), typeof(INonGenericSvc), typeof(DisposableIntGenericSvc) }));

            var serviceFactory = ambientServices.GetService<IExportFactory<INonGenericSvc, GenericSvcMetadata>>();
            Assert.AreSame(typeof(int), serviceFactory.Metadata.ServiceType);
        }

        [Test]
        public void BuildLiteCompositionContainer_multi_service_with_no_implementations()
        {
            var ambientServices = new AmbientServices()
                .WithStaticAppRuntime(this.IsAppAssembly);

            ambientServices.BuildLiteCompositionContainer(b => b.WithParts(new[] { typeof(IMultiService) }));

            var serviceFactoryList = ambientServices.GetService<IList<IExportFactory<IMultiService, AppServiceMetadata>>>();
            Assert.IsEmpty(serviceFactoryList);
        }

        [Test]
        public void BuildLiteCompositionContainer_internally_owned_disposed()
        {
            var disposable = Substitute.For<IDisposable>();
            var ambientServices = new AmbientServices()
                .WithStaticAppRuntime(this.IsAppAssembly)
                .Register<IDisposable>(b => b.WithInstance(disposable).ExternallyOwned(false));

            ambientServices.BuildLiteCompositionContainer();

            ambientServices.Dispose();
            disposable.Received(1).Dispose();
        }

        [Test]
        public void BuildLiteCompositionContainer_externally_owned_disposed()
        {
            var disposable = Substitute.For<IDisposable>();
            var ambientServices = new AmbientServices()
                .WithStaticAppRuntime(this.IsAppAssembly)
                .Register<IDisposable>(b => b.WithInstance(disposable).ExternallyOwned(true));

            ambientServices.BuildLiteCompositionContainer();

            ambientServices.Dispose();
            disposable.Received(0).Dispose();
        }

        [Test]
        public void GetExport_AppService_Singleton()
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
        public void GetExport_ExportFactory_NotFound()
        {
            var builder = this.CreateCompositionContainerBuilderWithStringLogger();
            var container = builder
                .WithAssembly(typeof(ICompositionContext).GetTypeInfo().Assembly)
                .CreateContainer();

            Assert.Throws<CompositionException>(() => container.GetExport<IExportFactory<ITestAppService>>());
        }

        [Test]
        public void GetExport_ExportFactoryWithMetadata_NotFound()
        {
            var builder = this.CreateCompositionContainerBuilderWithStringLogger();
            var container = builder
                .WithAssembly(typeof(ICompositionContext).GetTypeInfo().Assembly)
                .CreateContainer();

            Assert.Throws<CompositionException>(() => container.GetExport<IExportFactory<ITestAppService, AppServiceMetadata>>());
        }

        [Test]
        public void GetExport_Lazy_NotFound()
        {
            var builder = this.CreateCompositionContainerBuilderWithStringLogger();
            var container = builder
                .WithAssembly(typeof(ICompositionContext).GetTypeInfo().Assembly)
                .CreateContainer();

            Assert.Throws<CompositionException>(() => container.GetExport<Lazy<ITestAppService>>());
        }

        [Test]
        public void GetExport_LazyWithMetadata_NotFound()
        {
            var builder = this.CreateCompositionContainerBuilderWithStringLogger();
            var container = builder
                .WithAssembly(typeof(ICompositionContext).GetTypeInfo().Assembly)
                .CreateContainer();

            Assert.Throws<CompositionException>(() => container.GetExport<Lazy<ITestAppService, AppServiceMetadata>>());
        }

        [Test]
        public void GetExports_AppService_Multiple_Singleton()
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
        public void GetExport_AppService_generic_export_with_non_generic_contract()
        {
            var builder = this.CreateCompositionContainerBuilderWithStringLogger();
            var container = builder
                .WithAssembly(typeof(ICompositionContext).GetTypeInfo().Assembly)
                .WithParts(new[] { typeof(ITestGenericWithNonGenericExport), typeof(ITestGenericWithNonGenericExport<>), typeof(TestGenericWithNonGenericExport) })
                .CreateContainer();

            var export = container.GetExport<ITestGenericWithNonGenericExport>();
            Assert.IsInstanceOf<TestGenericWithNonGenericExport>(export);
        }

        [Test]
        [Ignore("Ignore until the CompositionConstructor attribute will be supported.")]
        public void GetExport_AppService_with_composition_constructor()
        {
            var builder = this.CreateCompositionContainerBuilderWithStringLogger();
            var container = builder
                .WithAssembly(typeof(ICompositionContext).GetTypeInfo().Assembly)
                .WithRegistration(
                    new AppServiceInfo(typeof(ExportedClass), typeof(ExportedClass)),
                    new AppServiceInfo(typeof(ExportedClassWithFakeDependency), typeof(ExportedClassWithFakeDependency)))
                .CreateContainer();
            var exported = container.GetExport<ExportedClassWithFakeDependency>();

            Assert.IsNotNull(exported);
            Assert.IsInstanceOf<ExportedClassWithFakeDependency>(exported);
            Assert.IsNull(exported.Dependency);
        }

        private LiteCompositionContainerBuilder CreateCompositionContainerBuilder(Action<ICompositionRegistrationContext> config = null)
        {
            var mockLoggerManager = Substitute.For<ILogManager>();
            var mockPlatformManager = Substitute.For<IAppRuntime>();

            var context = new CompositionRegistrationContext(new AmbientServices()
                                        .Register(mockLoggerManager)
                                        .Register(mockPlatformManager));
            config?.Invoke(context);
            var factory = new LiteCompositionContainerBuilder(context);
            return factory;
        }

        private LiteCompositionContainerBuilder CreateCompositionContainerBuilderWithStringLogger()
        {
            var builder = this.CreateCompositionContainerBuilder();

            var mockLoggerManager = builder.LogManager;
            mockLoggerManager.GetLogger(Arg.Any<string>()).Returns(Substitute.For<ILogger>());

            return builder;
        }

        private bool IsTestAssembly(AssemblyName assemblyName)
        {
            return assemblyName.Name.Contains("Test") || assemblyName.Name.Contains("NUnit") || assemblyName.Name.Contains("Mono") || assemblyName.Name.Contains("Proxy");
        }

        private bool IsAppAssembly(AssemblyName assemblyName)
        {
            return !this.IsTestAssembly(assemblyName) && !ReflectionHelper.IsSystemAssembly(assemblyName);
        }

        [SingletonAppServiceContract(MetadataAttributes = new[] { typeof(OverridePriorityAttribute) })]
        public interface ITestAppService { }

        [SingletonAppServiceContract(AllowMultiple = true)]
        public interface ITestMultiAppService { }

        [SingletonAppServiceContract]
        public interface ITestMultiAppServiceConsumer { }

        public class TestAppService : ITestAppService { }

        [OverridePriority(Priority.High)]
        public class TestOverrideAppService : ITestAppService { }

        public class TestMultiAppService1 : ITestMultiAppService { }

        public class TestMultiAppService2 : ITestMultiAppService { }

        public class TestMultiAppServiceConsumer : ITestMultiAppServiceConsumer
        {
            public TestMultiAppServiceConsumer(
                IEnumerable<IExportFactory<ITestMultiAppService>> factories,
                IEnumerable<IExportFactory<ITestMultiAppService, AppServiceMetadata>> metadataFactories)
            {
                this.Factories = factories;
                this.MetadataFactories = metadataFactories;
            }

            public IEnumerable<IExportFactory<ITestMultiAppService>> Factories { get; set; }

            public IEnumerable<IExportFactory<ITestMultiAppService, AppServiceMetadata>> MetadataFactories { get; set; }
        }

        public class TestMetadataConsumer
        {
            /// <summary>
            /// Gets or sets the test services.
            /// </summary>
            public ICollection<IExportFactory<ITestAppService, AppServiceMetadata>> TestServices { get; set; }
        }

        public interface IConverter { }

        [SingletonAppServiceContract(MetadataAttributes = new[] { typeof(ProcessingPriorityAttribute) },
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
            public ICollection<IExportFactory<IConverter, AppServiceMetadata>> Converters { get; set; }
        }

        [AppServiceContract(AsOpenGeneric = false)]
        public interface IGeneric<T> { }

        public class IntGeneric : IGeneric<int> { }

        [AppServiceContract]
        public class IntGenericDepedent
        {
            public IntGenericDepedent(IGeneric<int> intGeneric) { }
        }

        public interface INonGenericSvc { }

        [AppServiceContract(ContractType = typeof(INonGenericSvc))]
        public interface IGenericSvc<TService> : INonGenericSvc { }

        public class IntGenericSvc : IGenericSvc<int> { }

        // The order of the interfaces is important, as a test case
        // includes finding the first generic interface
        public class DisposableIntGenericSvc : IDisposable, IGenericSvc<int>
        {
            public void Dispose() { }
        }

        public class GenericSvcMetadata : AppServiceMetadata
        {
            public GenericSvcMetadata(IDictionary<string, object> metadata) : base(metadata)
            {
                if (metadata == null) { return; }
                this.ServiceType = (Type)metadata.TryGetValue(nameof(this.ServiceType));
            }

            public Type ServiceType { get; }
        }

        [AppServiceContract(AllowMultiple = true)]
        public interface IMultiService { }

        [SingletonAppServiceContract]
        public interface ITestGenericExport<T> { }

        public class TestGenericExport : ITestGenericExport<string> { }

        public interface ITestGenericWithNonGenericExport { }

        [AppServiceContract(ContractType = typeof(ITestGenericWithNonGenericExport))]
        public interface ITestGenericWithNonGenericExport<T> : ITestGenericWithNonGenericExport { }

        public class TestGenericWithNonGenericExport : ITestGenericWithNonGenericExport<string> { }

        public class ExportedClass
        {
        }

        public class ExportedClassWithFakeDependency : ExportedClass
        {
            [CompositionConstructor]
            public ExportedClassWithFakeDependency()
            {
            }

            public ExportedClassWithFakeDependency(ExportedClass dependency)
            {
                this.Dependency = dependency;
            }

            public ExportedClass Dependency { get; }
        }
    }
}
