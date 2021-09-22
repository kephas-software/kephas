// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AutofacInjectionContainerBuilderTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Autofac.Core.Activators.Reflection;
using Kephas.Injection;
using Kephas.Injection.AttributedModel;
using Kephas.Injection.Conventions;
using Kephas.Injection.Hosting;

namespace Kephas.Tests.Composition.Autofac
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    using global::Autofac.Core;

    using Kephas.Application;
    using Kephas.Composition;
    using Kephas.Composition.Autofac.Conventions;
    using Kephas.Composition.Autofac.Hosting;
    using Kephas.Logging;
    using Kephas.Services;
    using Kephas.Services.Reflection;
    using Kephas.Testing.Composition;

    using NSubstitute;

    using NUnit.Framework;

    /// <summary>
    /// Tests for <see cref="AutofacInjectorBuilder"/>.
    /// </summary>
    [TestFixture]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public class AutofacInjectionContainerBuilderTest : AutofacInjectionTestBase
    {
        [Test]
        public async Task CreateContainer_simple_ambient_services_exported()
        {
            var builder = this.CreateInjectorBuilder();
            var mockAppRuntime = builder.AppRuntime;

            mockAppRuntime.GetAppAssemblies(Arg.Any<Func<AssemblyName, bool>>())
                .Returns(new[] { typeof(ILogger).GetTypeInfo().Assembly, typeof(AutofacInjector).GetTypeInfo().Assembly });

            var container = builder.Build();

            var loggerManager = container.Resolve<ILogManager>();
            Assert.AreEqual(builder.LogManager, loggerManager);

            var platformManager = container.Resolve<IAppRuntime>();
            Assert.AreEqual(mockAppRuntime, platformManager);
        }

        [Test]
        public void CreateContainer_simple_ambient_services_exported_no_assemblies()
        {
            var builder = this.CreateInjectorBuilder();
            var container = builder
                .WithAssemblies(new Assembly[0])
                .WithPart(typeof(AppServiceInfoConventionsRegistrar))
                .Build();

            var loggerManager = container.Resolve<ILogManager>();
            Assert.AreEqual(builder.LogManager, loggerManager);

            var platformManager = container.Resolve<IAppRuntime>();
            Assert.AreEqual(builder.AppRuntime, platformManager);
        }

        [Test]
        public void CreateContainer_composed_loggers_exported()
        {
            var builder = this.CreateInjectorBuilderWithStringLogger();
            var container = builder
                .WithAssembly(typeof(IInjector).GetTypeInfo().Assembly)
                .WithAssembly(typeof(AutofacConventionsBuilder).GetTypeInfo().Assembly)
                .Build();

            var logger = container.Resolve<ILogger<AutofacInjectorTest.ExportedClass>>();
            Assert.IsInstanceOf<TypedLogger<AutofacInjectorTest.ExportedClass>>(logger);
        }

        [Test]
        public void Resolve_AppService_Singleton()
        {
            var builder = this.CreateInjectorBuilderWithStringLogger();
            var container = builder
                .WithAssembly(typeof(IInjector).GetTypeInfo().Assembly)
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
                .WithAssembly(typeof(IInjector).GetTypeInfo().Assembly)
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
                .WithAssembly(typeof(IInjector).GetTypeInfo().Assembly)
                .WithParts(new[] { typeof(ITestAppService), typeof(TestAppService), typeof(TestOverrideAppService) })
                .Build();

            var exported = container.Resolve<ITestAppService>();

            Assert.IsInstanceOf<TestOverrideAppService>(exported);
        }

        [Test]
        public void ResolveMany_AppService_Multiple_Singleton()
        {
            var builder = this.CreateInjectorBuilderWithStringLogger();
            var container = builder
                .WithAssembly(typeof(IInjector).GetTypeInfo().Assembly)
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
                .WithAssembly(typeof(IInjector).GetTypeInfo().Assembly)
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
                .WithAssembly(typeof(IInjector).GetTypeInfo().Assembly)
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
                .WithAssembly(typeof(IInjector).GetTypeInfo().Assembly)
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
                .WithAssembly(typeof(IInjector).GetTypeInfo().Assembly)
                .WithParts(new[] { typeof(ITestGenericWithNonGenericExport), typeof(ITestGenericWithNonGenericExport<>), typeof(TestGenericWithNonGenericExport) })
                .Build();

            var export = container.Resolve<ITestGenericWithNonGenericExport>();
            Assert.IsInstanceOf<TestGenericWithNonGenericExport>(export);
        }

        [Test]
        public void Resolve_AppService_with_injection_constructor()
        {
            var builder = this.CreateInjectorBuilderWithStringLogger();
            var container = builder
                .WithAssembly(typeof(IInjector).GetTypeInfo().Assembly)
                .WithRegistration(
                    new AppServiceInfo(typeof(ExportedClass), typeof(ExportedClass)),
                    new AppServiceInfo(typeof(ExportedClassWithFakeDependency), typeof(ExportedClassWithFakeDependency)))
                .Build();
            var exported = container.Resolve<ExportedClassWithFakeDependency>();

            Assert.IsNotNull(exported);
            Assert.IsInstanceOf<ExportedClassWithFakeDependency>(exported);
            Assert.IsNull(exported.Dependency);
        }

        [Test]
        public void Resolve_ScopedAppService_no_scope()
        {
            var builder = this.CreateInjectorBuilderWithStringLogger();
            var container = builder
                .WithAssembly(typeof(IInjector).GetTypeInfo().Assembly)
                .WithParts(new[] { typeof(ITestScopedExport), typeof(TestScopedExport) })
                .Build();

            Assert.IsInstanceOf<TestScopedExport>(container.Resolve<ITestScopedExport>());
        }

        [Test]
        public void Resolve_ScopedAppService_export()
        {
            var builder = this.CreateInjectorBuilderWithStringLogger();
            var container = builder
                .WithAssembly(typeof(IInjector).GetTypeInfo().Assembly)
                .WithParts(new[] { typeof(ITestScopedExport), typeof(TestScopedExport) })
                .Build();

            ITestScopedExport exportScope1;
            using (var scopedContext = container.CreateScopedInjector())
            {
                exportScope1 = scopedContext.Resolve<ITestScopedExport>();
                Assert.IsInstanceOf<TestScopedExport>(exportScope1);

                var export = scopedContext.Resolve<ITestScopedExport>();
                Assert.AreSame(exportScope1, export);
            }

            using (var scopedContext2 = container.CreateScopedInjector())
            {
                var export2 = scopedContext2.Resolve<ITestScopedExport>();
                Assert.AreNotSame(exportScope1, export2);
            }
        }

        [Test]
        public void Resolve_AppService_no_constructor()
        {
            var builder = this.CreateInjectorBuilderWithStringLogger()
                .WithAssembly(typeof(IInjector).GetTypeInfo().Assembly)
                .WithParts(new[] { typeof(IConstructorAppService), typeof(NoCompositionConstructorAppService) });
            Assert.Throws<NoConstructorsFoundException>(() => builder.Build());
        }

        [Test]
        public void Resolve_AppService_ambiguous_constructor()
        {
            var builder = this.CreateInjectorBuilderWithStringLogger();
            var container = builder
                .WithAssembly(typeof(IInjector).GetTypeInfo().Assembly)
                .WithParts(new[] { typeof(IConstructorAppService), typeof(AmbiguousCompositionConstructorAppService) })
                .Build();
            Assert.Throws<DependencyResolutionException>(() => container.Resolve<IConstructorAppService>());
        }

        [Test]
        public void Resolve_AppService_largest_constructor()
        {
            var builder = this.CreateInjectorBuilderWithStringLogger();
            var container = builder
                .WithAssembly(typeof(IInjector).GetTypeInfo().Assembly)
                .WithParts(new[] { typeof(IConstructorAppService), typeof(LargestCompositionConstructorAppService) })
                .Build();

            var component = container.Resolve<IConstructorAppService>();
            Assert.IsNotNull(component);
        }

        [Test]
        public void Resolve_AppService_multiple_constructor()
        {
            var builder = this.CreateInjectorBuilderWithStringLogger();
            Assert.Throws<InjectionException>(() => builder
                .WithAssembly(typeof(IInjector).GetTypeInfo().Assembly)
                .WithParts(new[] { typeof(IConstructorAppService), typeof(MultipleCompositionConstructorAppService) })
                .Build());
        }

        [Test]
        public void Resolve_AppService_default_constructor()
        {
            var builder = this.CreateInjectorBuilderWithStringLogger();
            var container = builder
                .WithAssembly(typeof(IInjector).GetTypeInfo().Assembly)
                .WithParts(new[] { typeof(IConstructorAppService), typeof(DefaultConstructorAppService) })
                .Build();

            var export = container.Resolve<IConstructorAppService>();

            Assert.IsInstanceOf<DefaultConstructorAppService>(export);
        }

        [Test]
        public void Resolve_AppService_single_constructor()
        {
            var builder = this.CreateInjectorBuilderWithStringLogger();
            var container = builder
                .WithAssembly(typeof(IInjector).GetTypeInfo().Assembly)
                .WithParts(new[] { typeof(IConstructorAppService), typeof(SingleConstructorAppService) })
                .Build();

            var export = container.Resolve<IConstructorAppService>();

            Assert.IsInstanceOf<SingleConstructorAppService>(export);
        }

        [Test]
        public async Task CreateContainer_instance_registration()
        {
            var registrar = Substitute.For<IConventionsRegistrar>();
            registrar
                .WhenForAnyArgs(r => r.RegisterConventions(Arg.Any<IConventionsBuilder>(), Arg.Any<IList<Type>>(), Arg.Any<IInjectionRegistrationContext>()))
                .Do(ci => { ci.Arg<IConventionsBuilder>().ForInstance(typeof(string), "123"); });

            var factory = this.CreateInjectorBuilder(ctx => ctx.Registrars = new[] { registrar });
            var mockAppRuntime = factory.AppRuntime;

            mockAppRuntime.GetAppAssemblies(Arg.Any<Func<AssemblyName, bool>>())
                .Returns(new[] { typeof(ILogger).GetTypeInfo().Assembly, typeof(AutofacInjector).GetTypeInfo().Assembly });

            var container = factory.Build();

            var instance = container.Resolve<string>();
            Assert.AreEqual("123", instance);
        }

        [Test]
        public async Task CreateContainer_instance_extension_registration()
        {
            var registrar = Substitute.For<IConventionsRegistrar>();
            registrar
                .WhenForAnyArgs(r => r.RegisterConventions(Arg.Any<IConventionsBuilder>(), Arg.Any<IList<Type>>(), Arg.Any<IInjectionRegistrationContext>()))
                .Do(ci => { ci.Arg<IConventionsBuilder>().ForInstance<string>("123"); });

            var factory = this.CreateInjectorBuilder(ctx => ctx.Registrars = new[] { registrar });
            var mockPlatformManager = factory.AppRuntime;

            mockPlatformManager.GetAppAssemblies(Arg.Any<Func<AssemblyName, bool>>())
                .Returns(new[] { typeof(ILogger).GetTypeInfo().Assembly, typeof(AutofacInjector).GetTypeInfo().Assembly });

            var container = factory.Build();

            var instance = container.Resolve<string>();
            Assert.AreEqual("123", instance);
        }

        [Test]
        public async Task CreateContainer_instance_factory_registration()
        {
            var registrar = Substitute.For<IConventionsRegistrar>();
            registrar
                .WhenForAnyArgs(r => r.RegisterConventions(Arg.Any<IConventionsBuilder>(), Arg.Any<IList<Type>>(), Arg.Any<IInjectionRegistrationContext>()))
                .Do(ci => { ci.Arg<IConventionsBuilder>().ForInstanceFactory(typeof(string), ctx => "123"); });

            var factory = this.CreateInjectorBuilder(ctx => ctx.Registrars = new[] { registrar });
            var mockPlatformManager = factory.AppRuntime;

            mockPlatformManager.GetAppAssemblies(Arg.Any<Func<AssemblyName, bool>>())
                .Returns(new[] { typeof(ILogger).GetTypeInfo().Assembly, typeof(AutofacInjector).GetTypeInfo().Assembly });

            var container = factory.Build();

            var instance = container.Resolve<string>();
            Assert.AreEqual("123", instance);
        }

        [Test]
        public async Task CreateContainer_instance_factory_extension_registration()
        {
            var registrar = Substitute.For<IConventionsRegistrar>();
            registrar
                .WhenForAnyArgs(r => r.RegisterConventions(Arg.Any<IConventionsBuilder>(), Arg.Any<IList<Type>>(), Arg.Any<IInjectionRegistrationContext>()))
                .Do(ci => { ci.Arg<IConventionsBuilder>().ForInstanceFactory<string>(ctx => "123"); });

            var factory = this.CreateInjectorBuilder(ctx => ctx.Registrars = new[] { registrar });
            var mockPlatformManager = factory.AppRuntime;

            mockPlatformManager.GetAppAssemblies(Arg.Any<Func<AssemblyName, bool>>())
                .Returns(new[] { typeof(ILogger).GetTypeInfo().Assembly, typeof(AutofacInjector).GetTypeInfo().Assembly });

            var container = factory.Build();

            var instance = container.Resolve<string>();
            Assert.AreEqual("123", instance);
        }

        private AutofacInjectorBuilder CreateInjectorBuilder(Action<IInjectionRegistrationContext> config = null)
        {
            var mockLoggerManager = Substitute.For<ILogManager>();
            var mockPlatformManager = Substitute.For<IAppRuntime>();

            var context = new InjectionRegistrationContext(new AmbientServices()
                                        .Register(mockLoggerManager)
                                        .Register(mockPlatformManager));
            config?.Invoke(context);
            var factory = new AutofacInjectorBuilder(context);
            return factory;
        }

        private AutofacInjectorBuilder CreateInjectorBuilderWithStringLogger()
        {
            var builder = this.CreateInjectorBuilder();

            var mockLoggerManager = builder.LogManager;
            mockLoggerManager.GetLogger(Arg.Any<string>()).Returns(Substitute.For<ILogger>());

            return builder;
        }

        // [Export]
        public class ComposedTestLogConsumer
        {
            public ComposedTestLogConsumer(ILogger<ComposedTestLogConsumer> logger)
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
            public ILogger<NonComposedTestLogConsumer> Logger { get; set; }
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

        //public class TestMetadataConsumer
        //{
        //    /// <summary>
        //    /// Gets or sets the test services.
        //    /// </summary>
        //    [Kephas.Composition.AttributedModel.ImportMany]
        //    public ICollection<ExportFactoryAdapter<ITestAppService, AppServiceMetadata>> TestServices { get; set; }
        //}

        public interface IConverter { }

        [SingletonAppServiceContract(MetadataAttributes = new[] { typeof(ProcessingPriorityAttribute) },
            ContractType = typeof(IConverter))]
        public interface IConverter<TSource, TTarget> : IConverter { }

        [ProcessingPriority(100)]
        public class StringToIntConverter : IConverter<string, int> { }

        public class SecondStringToIntConverter : IConverter<string, int> { }

        //public class TestConverterConsumer
        //{
        //    /// <summary>
        //    /// Gets or sets the converters.
        //    /// </summary>
        //    [ImportMany]
        //    public ICollection<ExportFactoryAdapter<IConverter, AppServiceMetadata>> Converters { get; set; }
        //}

        [ScopedAppServiceContract()]
        public interface ITestMyScopedExport { }

        public class TestMyScopedExport : ITestMyScopedExport { }

        //[CompositionScope]
        //public class MyScopeFactory : MefScopeFactoryBase
        //{
        //    /// <summary>
        //    /// Initializes a new instance of the <see cref="MefScopeFactoryBase"/> class.
        //    /// </summary>
        //    /// <param name="scopedContextFactory">The scoped context factory.</param>
        //    public MyScopeFactory([SharingBoundary(CompositionScopeNames.Default)] ExportFactory<CompositionContext> scopedContextFactory)
        //        : base(scopedContextFactory)
        //    {
        //    }
        //}

        [ScopedAppServiceContract]
        public interface ITestScopedExport { }

        public class TestScopedExport : ITestScopedExport { }

        [SingletonAppServiceContract]
        public interface ITestGenericExport<T> { }

        public class TestGenericExport : ITestGenericExport<string> { }

        public interface ITestGenericWithNonGenericExport { }

        [AppServiceContract(ContractType = typeof(ITestGenericWithNonGenericExport))]
        public interface ITestGenericWithNonGenericExport<T> : ITestGenericWithNonGenericExport { }

        public class TestGenericWithNonGenericExport : ITestGenericWithNonGenericExport<string> { }

        [SingletonAppServiceContract]
        public interface IConstructorAppService { }

        public class DefaultConstructorAppService : IConstructorAppService
        {
        }

        public class SingleConstructorAppService : IConstructorAppService
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="SingleConstructorAppService"/> class.
            /// </summary>
            /// <param name="injectionContainer">
            /// The composition container.
            /// </param>
            public SingleConstructorAppService(IInjector injectionContainer)
            {
            }
        }

        public class AmbiguousCompositionConstructorAppService : IConstructorAppService
        {
            public AmbiguousCompositionConstructorAppService(IAmbientServices ambientServices)
            {
            }

            public AmbiguousCompositionConstructorAppService(IInjector injectionContainer)
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
            /// <param name="injectionContainer">
            /// The composition container.
            /// </param>
            public LargestCompositionConstructorAppService(IInjector injectionContainer)
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
            [InjectConstructor]
            public MultipleCompositionConstructorAppService()
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="MultipleCompositionConstructorAppService"/> class.
            /// </summary>
            /// <param name="injectionContainer">
            /// The composition container.
            /// </param>
            [InjectConstructor]
            public MultipleCompositionConstructorAppService(IInjector injectionContainer)
            {
            }
        }

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
