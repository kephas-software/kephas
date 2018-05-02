// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompositionContainerBuilderBaseTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
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

    using Kephas.Application;
    using Kephas.Application.Configuration;
    using Kephas.Composition;
    using Kephas.Composition.Conventions;
    using Kephas.Composition.Hosting;
    using Kephas.Configuration;
    using Kephas.Logging;

    using NSubstitute;

    using NUnit.Framework;

    /// <summary>
    /// Test class for <see cref="CompositionContainerBuilderBase{TBuilder}"/>.
    /// </summary>
    [TestFixture]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public class CompositionContainerBuilderBaseTest
    {
        [Test]
        public void Constructor_success()
        {
            var logManager = Substitute.For<ILogManager>();
            var configuration = Substitute.For<IAppConfiguration>();
            var platformManager = Substitute.For<IAppRuntime>();
            var builder = new TestCompositionContainerBuilder(logManager, configuration, platformManager);

            Assert.AreEqual(logManager, builder.LogManager);
            Assert.AreEqual(configuration, builder.AppConfiguration);
            Assert.AreEqual(platformManager, builder.AppRuntime);

            // The IServiceProvider export providers.
            Assert.AreEqual(1, builder.InternalExportProviders.Count);
        }

        [Test]
        public void WithConventionsBuilder()
        {
            var conventions = Substitute.For<IConventionsBuilder>();
            var builder = new TestCompositionContainerBuilder()
                .WithConventions(conventions);

            Assert.AreSame(conventions, builder.InternalConventionsBuilder);
        }

        [Test]
        public void CreateContainer()
        {
            var builder = new TestCompositionContainerBuilder()
                .WithAssemblies(new[] { this.GetType().Assembly });

            var container = builder.CreateContainer();
            Assert.IsNotNull(container);
        }

        public class TestCompositionContainerBuilder : CompositionContainerBuilderBase<TestCompositionContainerBuilder>
        {
            public TestCompositionContainerBuilder()
                : base(new CompositionRegistrationContext(new AmbientServices()))
            {
            }

            public TestCompositionContainerBuilder(ILogManager logManager, IAppConfiguration appConfiguration, IAppRuntime appRuntime)
                : base(new CompositionRegistrationContext(new AmbientServices().RegisterService(logManager).RegisterService(appConfiguration).RegisterService(appRuntime)))
            {
            }

            public IConventionsBuilder InternalConventionsBuilder
            {
                get { return this.ConventionsBuilder; }
            }

            public IList<IExportProvider> InternalExportProviders
            {
                get { return this.ExportProviders; }
            }

            protected override IExportProvider CreateFactoryExportProvider<TContract>(Func<TContract> factory, bool isShared = false)
            {
                return Substitute.For<IExportProvider>();
            }

            protected override IExportProvider CreateServiceProviderExportProvider(IServiceProvider serviceProvider, Func<IServiceProvider, Type, bool> isServiceRegisteredFunc)
            {
                return Substitute.For<IExportProvider>();
            }

            protected override IConventionsBuilder CreateConventionsBuilder()
            {
                return Substitute.For<IConventionsBuilder>();
            }

            protected override ICompositionContext CreateContainerCore(IConventionsBuilder conventions, IEnumerable<Type> parts)
            {
                return Substitute.For<ICompositionContext>();
            }
        }

        public class TestConventionsBuilder : IConventionsBuilder
        {
            public TestConventionsBuilder()
            {
                this.DerivedConventionsBuilders = new Dictionary<Type, IPartConventionsBuilder>();
                this.TypeConventionsBuilders = new Dictionary<Type, IPartConventionsBuilder>();
                this.MatchingConventionsBuilders = new Dictionary<Predicate<Type>, IPartConventionsBuilder>();
            }

            public IDictionary<Type, IPartConventionsBuilder> DerivedConventionsBuilders { get; private set; }

            public IDictionary<Type, IPartConventionsBuilder> TypeConventionsBuilders { get; private set; }

            public IDictionary<Predicate<Type>, IPartConventionsBuilder> MatchingConventionsBuilders { get; private set; }

            public IPartConventionsBuilder ForTypesDerivedFrom(Type type)
            {
                var partBuilder = this.CreateBuilder(type);
                this.DerivedConventionsBuilders.Add(type, partBuilder);
                return partBuilder;
            }

            public IPartConventionsBuilder ForTypesMatching(Predicate<Type> typePredicate)
            {
                var partBuilder = this.CreateBuilder(typePredicate);
                this.MatchingConventionsBuilders.Add(typePredicate, partBuilder);
                return partBuilder;
            }

            public IPartConventionsBuilder ForType(Type type)
            {
                var partBuilder = this.CreateBuilder(type);
                this.TypeConventionsBuilders.Add(type, partBuilder);
                return partBuilder;
            }

            /// <summary>
            /// Defines a registration for the specified type and its singleton instance.
            /// </summary>
            /// <param name="type">The registered service type.</param>
            /// <param name="instance">The instance.</param>
            /// <returns>A <see cref="IPartBuilder"/> to further configure the rule.</returns>
            public IPartBuilder ForInstance(Type type, object instance)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// Defines a registration for the specified type and its instance factory.
            /// </summary>
            /// <param name="type">The registered service type.</param>
            /// <param name="factory">The service factory.</param>
            /// <returns>A <see cref="IPartBuilder"/> to further configure the rule.</returns>
            public IPartBuilder ForInstanceFactory(Type type, Func<ICompositionContext, object> factory)
            {
                throw new NotImplementedException();
            }

            private IPartConventionsBuilder CreateBuilder(Type type)
            {
                return new TestPartConventionsBuilder(type);
            }

            private IPartConventionsBuilder CreateBuilder(Predicate<Type> typePredicate)
            {
                return new TestPartConventionsBuilder(typePredicate);
            }
        }

        public class TestPartConventionsBuilder : IPartConventionsBuilder
        {
            public TestPartConventionsBuilder(Type type)
            {
                this.Type = type;
                this.ExportBuilder = new TestExportConventionsBuilder();
                this.ImportedProperties = new List<Tuple<Predicate<PropertyInfo>, Action<PropertyInfo, IImportConventionsBuilder>>>();
            }

            public TestPartConventionsBuilder(Predicate<Type> typePredicate)
            {
                this.TypePredicate = typePredicate;
                this.ExportBuilder = new TestExportConventionsBuilder();
                this.ImportedProperties = new List<Tuple<Predicate<PropertyInfo>, Action<PropertyInfo, IImportConventionsBuilder>>>();
            }

            public TestExportConventionsBuilder ExportBuilder { get; set; }

            public Type Type { get; set; }

            public Predicate<Type> TypePredicate { get; set; }

            public bool IsShared { get; set; }

            public List<Tuple<Predicate<PropertyInfo>, Action<PropertyInfo, IImportConventionsBuilder>>> ImportedProperties { get; }

            public IPartConventionsBuilder Shared()
            {
                this.IsShared = true;
                return this;
            }

            /// <summary>
            /// Mark the part as being shared within the scope.
            /// </summary>
            /// <param name="scopeName"></param>
            /// <returns>A part builder allowing further configuration of the part.</returns>
            public IPartConventionsBuilder ScopeShared(string scopeName = ScopeNames.Default)
            {
                // TODO test this
                this.IsShared = true;
                this.ScopeName = scopeName;

                return this;
            }

            public string ScopeName { get; private set; }

            public Func<IEnumerable<ConstructorInfo>, ConstructorInfo> ConstructorSelector { get; private set; }

            public IPartConventionsBuilder Export(Action<IExportConventionsBuilder> conventionsBuilder = null)
            {
                conventionsBuilder?.Invoke(this.ExportBuilder);

                return this;
            }

            public IPartConventionsBuilder ExportInterfaces(Predicate<Type> interfaceFilter = null, Action<Type, IExportConventionsBuilder> exportConfiguration = null)
            {
                return this;
            }

            public IPartConventionsBuilder SelectConstructor(Func<IEnumerable<ConstructorInfo>, ConstructorInfo> constructorSelector, Action<ParameterInfo, IImportConventionsBuilder> importConfiguration = null)
            {
                this.ConstructorSelector = constructorSelector;
                return this;
            }

            public IPartConventionsBuilder ImportProperties(Predicate<PropertyInfo> propertyFilter, Action<PropertyInfo, IImportConventionsBuilder> importConfiguration = null)
            {
                this.ImportedProperties.Add(Tuple.Create(propertyFilter, importConfiguration));

                return this;
            }
        }

        public class TestExportConventionsBuilder : IExportConventionsBuilder
        {
            public TestExportConventionsBuilder()
            {
                this.Metadata = new Dictionary<string, object>();
            }

            public Type ContractType { get; set; }

            public IDictionary<string, object> Metadata { get; set; }

            public IExportConventionsBuilder AsContractType(Type contractType)
            {
                this.ContractType = contractType;
                return this;
            }

            public IExportConventionsBuilder AddMetadata(string name, object value)
            {
                this.Metadata.Add(name, value);
                return this;
            }

            public IExportConventionsBuilder AddMetadata(string name, Func<Type, object> getValueFromPartType)
            {
                this.Metadata.Add(name, getValueFromPartType);
                return this;
            }
        }
    }
}