// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LiteInjectorBuilderTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the lite injector builder test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests.Injection.Lite.Hosting
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Kephas.Application;
    using Kephas.Collections;
    using Kephas.Cryptography;
    using Kephas.Injection;
    using Kephas.Injection.AttributedModel;
    using Kephas.Injection.Builder;
    using Kephas.Injection.Internal;
    using Kephas.Injection.Lite.Builder;
    using Kephas.Logging;
    using Kephas.Reflection;
    using Kephas.Services;
    using Kephas.Services.Reflection;
    using Kephas.Testing.Injection;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class LiteInjectorBuilderTest
    {
        [Test]
        public void BuildWithLite_app_manager()
        {
            var ambientServices = new AmbientServices()
                .WithStaticAppRuntime(an => !this.IsTestAssembly(an));

            ambientServices.BuildWithLite();

            Assert.IsInstanceOf<InjectorAdapter>(ambientServices.Injector);

            var appManager = ambientServices.Injector.Resolve<IEncryptionService>();
            Assert.IsInstanceOf<NullEncryptionService>(appManager);
        }

        [Test]
        public void BuildWithLite_closed_generic()
        {
            var ambientServices = new AmbientServices();

            ambientServices.BuildWithLite(b => b.WithParts(new[] { typeof(IGeneric<>), typeof(IntGeneric) }));

            var service = ((IServiceProvider)ambientServices).GetService(typeof(IGeneric<int>));
            Assert.IsInstanceOf<IntGeneric>(service);
        }

        [Test]
        public void BuildWithLite_closed_generic_dependency()
        {
            var ambientServices = new AmbientServices()
                .WithStaticAppRuntime(this.IsAppAssembly);

            ambientServices.BuildWithLite(b => b.WithParts(new[] { typeof(IGeneric<>), typeof(IntGeneric), typeof(IntGenericDepedent) }));

            var service = ambientServices.GetService<IntGenericDepedent>();
            Assert.IsNotNull(service);
        }

        [Test]
        public void BuildWithLite_closed_generic_with_non_generic_contract_metadata()
        {
            var ambientServices = new AmbientServices()
                .WithStaticAppRuntime(this.IsAppAssembly);

            ambientServices.BuildWithLite(b => b.WithParts(new[] { typeof(IGenericSvc<>), typeof(INonGenericSvc), typeof(IntGenericSvc) }));

            var serviceFactory = ambientServices.GetService<IExportFactory<INonGenericSvc, GenericSvcMetadata>>();
            Assert.AreSame(typeof(int), serviceFactory.Metadata.TargetServiceType);
        }

        [Test]
        public void BuildWithLite_disposable_closed_generic_with_non_generic_contract_metadata()
        {
            var ambientServices = new AmbientServices()
                .WithStaticAppRuntime(this.IsAppAssembly);

            ambientServices.BuildWithLite(b => b.WithParts(new[] { typeof(IGenericSvc<>), typeof(INonGenericSvc), typeof(DisposableIntGenericSvc) }));

            var serviceFactory = ambientServices.GetService<IExportFactory<INonGenericSvc, GenericSvcMetadata>>();
            Assert.AreSame(typeof(int), serviceFactory.Metadata.TargetServiceType);
        }

        [Test]
        public void BuildWithLite_multi_service_with_no_implementations()
        {
            var ambientServices = new AmbientServices()
                .WithStaticAppRuntime(this.IsAppAssembly);

            ambientServices.BuildWithLite(b => b.WithParts(new[] { typeof(IMultiService) }));

            var serviceFactoryList = ambientServices.GetService<IList<IExportFactory<IMultiService, AppServiceMetadata>>>();
            Assert.IsEmpty(serviceFactoryList);
        }

        [Test]
        public void BuildWithLite_internally_owned_disposed()
        {
            var disposable = Substitute.For<IDisposable>();
            var ambientServices = new AmbientServices()
                .WithStaticAppRuntime(this.IsAppAssembly)
                .Register<IDisposable>(b => b.ForInstance(disposable));

            ambientServices.BuildWithLite();

            ambientServices.Dispose();
            disposable.Received(1).Dispose();
        }

        [Test]
        public void BuildWithLite_externally_owned_disposed()
        {
            var disposable = Substitute.For<IDisposable>();
            var ambientServices = new AmbientServices()
                .WithStaticAppRuntime(this.IsAppAssembly)
                .Register<IDisposable>(b => b.ForInstance(disposable).ExternallyOwned());

            ambientServices.BuildWithLite();

            ambientServices.Dispose();
            disposable.Received(0).Dispose();
        }

        [Test]
        public void Resolve_AppService_Singleton()
        {
            var builder = this.CreateInjectorBuilderWithStringLogger();
            var container = builder
                .WithAssemblies(typeof(IInjector).Assembly, typeof(IContextFactory).Assembly)
                .WithParts(new[] { typeof(ITestAppService), typeof(TestAppService) })
                .Build();

            var exported = container.Resolve<ITestAppService>();
            var secondExported = container.Resolve<ITestAppService>();

            Assert.AreSame(exported, secondExported);
        }

        [Test]
        public void Resolve_AppService_Single_Success()
        {
            var builder = this.CreateInjectorBuilderWithStringLogger();
            var container = builder
                .WithAssemblies(typeof(IInjector).Assembly, typeof(IContextFactory).Assembly)
                .WithParts(new[] { typeof(ITestAppService), typeof(TestAppService) })
                .Build();

            var exported = container.Resolve<ITestAppService>();

            Assert.IsInstanceOf<TestAppService>(exported);
        }

        [Test]
        public void Resolve_AppService_Single_Override_Success()
        {
            var builder = this.CreateInjectorBuilderWithStringLogger();
            var container = builder
                .WithAssemblies(typeof(IInjector).Assembly, typeof(IContextFactory).Assembly)
                .WithParts(new[] { typeof(ITestAppService), typeof(TestAppService), typeof(TestOverrideAppService) })
                .Build();

            var exported = container.Resolve<ITestAppService>();

            Assert.IsInstanceOf<TestOverrideAppService>(exported);
        }

        [Test]
        public void Resolve_ExportFactory_NotFound()
        {
            var builder = this.CreateInjectorBuilderWithStringLogger();
            var container = builder
                .WithAssemblies(typeof(IInjector).Assembly, typeof(IContextFactory).Assembly)
                .Build();

            Assert.Throws<InjectionException>(() => container.Resolve<IExportFactory<ITestAppService>>());
        }

        [Test]
        public void Resolve_ExportFactoryWithMetadata_NotFound()
        {
            var builder = this.CreateInjectorBuilderWithStringLogger();
            var container = builder
                .WithAssemblies(typeof(IInjector).Assembly, typeof(IContextFactory).Assembly)
                .Build();

            Assert.Throws<InjectionException>(() => container.Resolve<IExportFactory<ITestAppService, AppServiceMetadata>>());
        }

        [Test]
        public void Resolve_Lazy_NotFound()
        {
            var builder = this.CreateInjectorBuilderWithStringLogger();
            var container = builder
                .WithAssemblies(typeof(IInjector).Assembly, typeof(IContextFactory).Assembly)
                .Build();

            Assert.Throws<InjectionException>(() => container.Resolve<Lazy<ITestAppService>>());
        }

        [Test]
        public void Resolve_LazyWithMetadata_NotFound()
        {
            var builder = this.CreateInjectorBuilderWithStringLogger();
            var container = builder
                .WithAssemblies(typeof(IInjector).Assembly, typeof(IContextFactory).Assembly)
                .Build();

            Assert.Throws<InjectionException>(() => container.Resolve<Lazy<ITestAppService, AppServiceMetadata>>());
        }

        [Test]
        public void ResolveMany_AppService_Multiple_Singleton()
        {
            var builder = this.CreateInjectorBuilderWithStringLogger();
            var container = builder
                .WithAssemblies(typeof(IInjector).Assembly, typeof(IContextFactory).Assembly)
                .WithParts(new[] { typeof(ITestMultiAppService), typeof(TestMultiAppService1), typeof(TestMultiAppService2) })
                .Build();

            var exports = container.ResolveMany<ITestMultiAppService>().ToList();
            var exports2 = container.ResolveMany<ITestMultiAppService>().ToList();

            Assert.AreSame(exports[0], exports2[0]);
            Assert.AreSame(exports[1], exports2[1]);
        }

        [Test]
        public void ResolveMany_AppService_Multiple_Success()
        {
            var builder = this.CreateInjectorBuilderWithStringLogger();
            var container = builder
                .WithAssemblies(typeof(IInjector).Assembly, typeof(IContextFactory).Assembly)
                .WithParts(new[] { typeof(ITestMultiAppService), typeof(TestMultiAppService1), typeof(TestMultiAppService2) })
                .Build();

            var exports = container.ResolveMany<ITestMultiAppService>().ToList();

            Assert.AreEqual(2, exports.Count);
        }

        [Test]
        public void ResolveMany_AppService_IExportFactory_Success()
        {
            var builder = this.CreateInjectorBuilderWithStringLogger();
            var container = builder
                .WithAssemblies(typeof(IInjector).Assembly, typeof(IContextFactory).Assembly)
                .WithParts(new[] { typeof(ITestMultiAppService), typeof(ITestMultiAppServiceConsumer), typeof(TestMultiAppService1), typeof(TestMultiAppService2), typeof(TestMultiAppServiceConsumer) })
                .Build();

            var export = (TestMultiAppServiceConsumer)container.Resolve<ITestMultiAppServiceConsumer>();

            Assert.AreEqual(2, export.Factories.Count());
            Assert.AreEqual(2, export.MetadataFactories.Count());
        }

        [Test]
        public void Resolve_AppService_generic_export()
        {
            var builder = this.CreateInjectorBuilderWithStringLogger();
            var container = builder
                .WithAssemblies(typeof(IInjector).Assembly, typeof(IContextFactory).Assembly)
                .WithParts(new[] { typeof(ITestGenericExport<>), typeof(TestGenericExport) })
                .Build();

            var export = container.Resolve<ITestGenericExport<string>>();
            Assert.IsInstanceOf<TestGenericExport>(export);
        }

        [Test]
        public void Resolve_AppService_generic_export_with_non_generic_contract()
        {
            var builder = this.CreateInjectorBuilderWithStringLogger();
            var container = builder
                .WithAssemblies(typeof(IInjector).Assembly, typeof(IContextFactory).Assembly)
                .WithParts(new[] { typeof(ITestGenericWithNonGenericExport), typeof(ITestGenericWithNonGenericExport<>), typeof(TestGenericWithNonGenericExport) })
                .Build();

            var export = container.Resolve<ITestGenericWithNonGenericExport>();
            Assert.IsInstanceOf<TestGenericWithNonGenericExport>(export);
        }

        [Test]
        [Ignore("Ignore until the CompositionConstructor attribute will be supported.")]
        public void Resolve_AppService_with_injection_constructor()
        {
            var builder = this.CreateInjectorBuilderWithStringLogger();
            var container = builder
                .WithAssemblies(typeof(IInjector).Assembly, typeof(IContextFactory).Assembly)
                .WithRegistration(
                    new AppServiceInfo(typeof(ExportedClass), typeof(ExportedClass)),
                    new AppServiceInfo(typeof(ExportedClassWithFakeDependency), typeof(ExportedClassWithFakeDependency)))
                .Build();
            var exported = container.Resolve<ExportedClassWithFakeDependency>();

            Assert.IsNotNull(exported);
            Assert.IsInstanceOf<ExportedClassWithFakeDependency>(exported);
            Assert.IsNull(exported.Dependency);
        }

        private LiteInjectorBuilder CreateInjectorBuilder(Action<IInjectionBuildContext>? config = null, IAmbientServices? ambientServices = null)
        {
            var mockLoggerManager = Substitute.For<ILogManager>();
            var mockPlatformManager = Substitute.For<IAppRuntime>();

            var context = new InjectionBuildContext((ambientServices ?? new AmbientServices())
                                        .Register(mockLoggerManager)
                                        .Register(mockPlatformManager));
            config?.Invoke(context);
            var factory = new LiteInjectorBuilder(context);
            return factory;
        }

        private LiteInjectorBuilder CreateInjectorBuilderWithStringLogger()
        {
            IAmbientServices ambientServices = new AmbientServices();
            var builder = this.CreateInjectorBuilder(ambientServices: ambientServices);

            var mockLoggerManager = ambientServices.LogManager;
            mockLoggerManager.GetLogger(Arg.Any<string>()).Returns(Substitute.For<ILogger>());

            return builder;
        }

        private bool IsTestAssembly(AssemblyName assemblyName)
        {
            return assemblyName.Name.Contains("Test") || assemblyName.Name.Contains("NUnit") || assemblyName.Name.Contains("Mono") || assemblyName.Name.Contains("Proxy");
        }

        private bool IsAppAssembly(AssemblyName assemblyName)
        {
            return !this.IsTestAssembly(assemblyName) && !assemblyName.IsSystemAssembly();
        }

        [SingletonAppServiceContract]
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

        [SingletonAppServiceContract(ContractType = typeof(IConverter))]
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
        public interface IGenericSvc<TTargetService> : INonGenericSvc { }

        public class IntGenericSvc : IGenericSvc<int> { }

        // The order of the interfaces is important, as a test case
        // includes finding the first generic interface
        public class DisposableIntGenericSvc : IDisposable, IGenericSvc<int>
        {
            public void Dispose() { }
        }

        public class GenericSvcMetadata : AppServiceMetadata
        {
            public GenericSvcMetadata(IDictionary<string, object?>? metadata) : base(metadata)
            {
                if (metadata == null) { return; }
                this.TargetServiceType = (Type?)metadata.TryGetValue(nameof(this.TargetServiceType));
            }

            public Type? TargetServiceType { get; set; }
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
            [InjectConstructor]
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
