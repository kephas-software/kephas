// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompositionContainerBuilderBaseTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Test class for <see cref="CompositionContainerBuilderBase{TBuilder}" />.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests.Composition
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;
    using System.Threading.Tasks;

    using Kephas.Composition;
    using Kephas.Composition.Conventions;
    using Kephas.Configuration;
    using Kephas.Logging;
    using Kephas.Runtime;
    using Kephas.Services;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Telerik.JustMock;

    /// <summary>
    /// Test class for <see cref="CompositionContainerBuilderBase{TBuilder}"/>.
    /// </summary>
    [TestClass]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public class CompositionContainerBuilderBaseTest
    {
        [TestMethod]
        public void Constructor_success()
        {
            var logManager = Mock.Create<ILogManager>();
            var configManager = Mock.Create<IConfigurationManager>();
            var platformManager = Mock.Create<IPlatformManager>();
            var builder = new TestCompositionContainerBuilder(logManager, configManager, platformManager);

            Assert.AreEqual(logManager, builder.LogManager);
            Assert.AreEqual(configManager, builder.ConfigurationManager);
            Assert.AreEqual(platformManager, builder.PlatformManager);
            Assert.AreEqual(3, builder.InternalExportProviders.Count);
            Assert.IsTrue(builder.InternalExportProviders.ContainsKey(typeof(ILogManager)));
            Assert.IsTrue(builder.InternalExportProviders.ContainsKey(typeof(IConfigurationManager)));
            Assert.IsTrue(builder.InternalExportProviders.ContainsKey(typeof(IPlatformManager)));
        }

        [TestMethod]
        public void WithConventionsBuilder()
        {
            var conventions = Mock.Create<IConventionsBuilder>();
            var builder = new TestCompositionContainerBuilder()
                .WithConventions(conventions);
            
            Assert.AreSame(conventions, builder.InternalConventionsBuilder);
        }

        [TestMethod]
        public void RegisterApplicationServices_Multiple()
        {
            var conventions = new TestConventionsBuilder();
            var builder = new TestCompositionContainerBuilder()
                .WithConventions(conventions);

            builder.InternalRegisterAppServices(
                conventions,
                new[]
                    {
                        typeof(IMultipleTestAppService).GetTypeInfo(), 
                        typeof(MultipleTestService).GetTypeInfo(),
                        typeof(NewMultipleTestService).GetTypeInfo(),
                    });

            Assert.AreEqual(1, conventions.DerivedConventionsBuilders.Count);
            Assert.IsTrue(conventions.DerivedConventionsBuilders.ContainsKey(typeof(IMultipleTestAppService)));
        }

        [TestMethod]
        public void RegisterApplicationServices_Single_FactoryProviderOverride()
        {
            var conventions = new TestConventionsBuilder();
            var builder = new TestCompositionContainerBuilder()
                .WithConventions(conventions)
                .WithFactoryProvider<ISingleTestAppService>(() => new SingleTestService());

            builder.InternalRegisterAppServices(
                conventions,
                new[]
                    {
                        typeof(ISingleTestAppService).GetTypeInfo(), 
                        typeof(SingleTestService).GetTypeInfo()
                    });

            Assert.AreEqual(0, conventions.TypeConventionsBuilders.Count);
        }

        [TestMethod]
        public void RegisterApplicationServices_Single_one_service()
        {
            var conventions = new TestConventionsBuilder();
            var builder = new TestCompositionContainerBuilder()
                .WithConventions(conventions);

            builder.InternalRegisterAppServices(
                conventions,
                new []
                    {
                        typeof(ISingleTestAppService).GetTypeInfo(), 
                        typeof(SingleTestService).GetTypeInfo()
                    });

            Assert.AreEqual(1, conventions.TypeConventionsBuilders.Count);
            Assert.IsTrue(conventions.TypeConventionsBuilders.ContainsKey(typeof(SingleTestService)));
        }

        [TestMethod]
        public void RegisterApplicationServices_Single_override_service_success()
        {
            var conventions = new TestConventionsBuilder();
            var builder = new TestCompositionContainerBuilder()
                .WithConventions(conventions);

            builder.InternalRegisterAppServices(
                conventions,
                new []
                    {
                        typeof(ISingleTestAppService).GetTypeInfo(), 
                        typeof(SingleTestService).GetTypeInfo(),
                        typeof(SingleOverrideTestService).GetTypeInfo(),
                    });

            Assert.AreEqual(1, conventions.TypeConventionsBuilders.Count);
            Assert.IsTrue(conventions.TypeConventionsBuilders.ContainsKey(typeof(SingleOverrideTestService)));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void RegisterApplicationServices_Single_override_service_failure()
        {
            var conventions = new TestConventionsBuilder();
            var builder = new TestCompositionContainerBuilder()
                .WithConventions(conventions);

            builder.InternalRegisterAppServices(
                conventions,
                new[]
                    {
                        typeof(ISingleTestAppService).GetTypeInfo(), 
                        typeof(SingleTestService).GetTypeInfo(),
                        typeof(SingleSameOverrideTestService).GetTypeInfo(),
                    });

            Assert.AreEqual(1, conventions.TypeConventionsBuilders.Count);
            Assert.IsTrue(conventions.TypeConventionsBuilders.ContainsKey(typeof(SingleOverrideTestService)));
        }

        [TestMethod]
        public void CreateContainer()
        {
            var builder = new TestCompositionContainerBuilder()
                .WithAssemblies(new[] { this.GetType().Assembly });

            var container = builder.CreateContainer();
            Assert.IsNotNull(container);
        }

        [TestMethod]
        public async Task CreateContainerAsync()
        {
            var builder = new TestCompositionContainerBuilder()
                .WithAssemblies(new[] { this.GetType().Assembly });

            var container = await builder.CreateContainerAsync();
            Assert.IsNotNull(container);
        }

        public class TestCompositionContainerBuilder : CompositionContainerBuilderBase<TestCompositionContainerBuilder>
        {
            public TestCompositionContainerBuilder()
                : this(Mock.Create<ILogManager>(), Mock.Create<IConfigurationManager>(), Mock.Create<IPlatformManager>())
            {
            }

            public TestCompositionContainerBuilder(ILogManager logManager, IConfigurationManager configurationManager, IPlatformManager platformManager)
                : base(logManager, configurationManager, platformManager)
            {
            }

            public IConventionsBuilder InternalConventionsBuilder
            {
                get { return this.ConventionsBuilder; }
            }

            public IDictionary<Type, IExportProvider> InternalExportProviders
            {
                get { return this.ExportProviders; }
            }

            public void InternalRegisterAppServices(IConventionsBuilder conventions, IEnumerable<TypeInfo> typeInfos)
            {
                this.RegisterAppServices(conventions, typeInfos);
            }

            protected override IExportProvider CreateFactoryProvider<TContract>(Func<TContract> factory, bool isShared = false)
            {
                return Mock.Create<IExportProvider>();
            }

            protected override IConventionsBuilder CreateConventionsBuilder()
            {
                return Mock.Create<IConventionsBuilder>();
            }

            protected override ICompositionContainer CreateContainerCore(IConventionsBuilder conventions, IEnumerable<Assembly> assemblies)
            {
                return Mock.Create<ICompositionContainer>();
            }
        }

        public class TestConventionsBuilder : IConventionsBuilder
        {
            public TestConventionsBuilder()
            {
                this.DerivedConventionsBuilders = new Dictionary<Type, IPartConventionsBuilder>();
                this.TypeConventionsBuilders = new Dictionary<Type, IPartConventionsBuilder>();
            }

            public IDictionary<Type, IPartConventionsBuilder> DerivedConventionsBuilders { get; private set; }

            public IDictionary<Type, IPartConventionsBuilder> TypeConventionsBuilders { get; private set; }

            public IPartConventionsBuilder ForTypesDerivedFrom(Type type)
            {
                var partBuilder = this.CreateBuilder(type);
                this.DerivedConventionsBuilders.Add(type, partBuilder);
                return partBuilder;
            }

            public IPartConventionsBuilder ForType(Type type)
            {
                var partBuilder = this.CreateBuilder(type);
                this.TypeConventionsBuilders.Add(type, partBuilder);
                return partBuilder;
            }

            private IPartConventionsBuilder CreateBuilder(Type type)
            {
                return Mock.Create<IPartConventionsBuilder>();
            }
        }

        [AppServiceContract(AllowMultiple = false)]
        public interface ISingleTestAppService { }

        [AppServiceContract(AllowMultiple = true)]
        public interface IMultipleTestAppService { }

        public class SingleTestService : ISingleTestAppService { }

        [OverridePriority(Priority.High)]
        public class SingleOverrideTestService : ISingleTestAppService { }

        public class SingleSameOverrideTestService : ISingleTestAppService { }

        public class MultipleTestService : IMultipleTestAppService { }

        public class NewMultipleTestService : IMultipleTestAppService { }
    }
}