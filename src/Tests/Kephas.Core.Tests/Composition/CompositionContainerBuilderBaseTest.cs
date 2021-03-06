﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompositionContainerBuilderBaseTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
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

    using Kephas.Application;
    using Kephas.Composition;
    using Kephas.Composition.Conventions;
    using Kephas.Composition.Hosting;
    using Kephas.Logging;
    using Kephas.Reflection;
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
            var appRuntime = Substitute.For<IAppRuntime>();
            var builder = new TestCompositionContainerBuilder(logManager, appRuntime);

            Assert.AreEqual(logManager, builder.LogManager);
            Assert.AreEqual(appRuntime, builder.AppRuntime);
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
            var ambientServices = new AmbientServices(registerDefaultServices: false)
                .Register(Substitute.For<ILogManager>())
                .Register(Substitute.For<ITypeLoader>())
                .Register(Substitute.For<IAppRuntime>());
            var builder = new TestCompositionContainerBuilder(ambientServices)
                .WithAssemblies(new[] { this.GetType().Assembly });

            var container = builder.CreateContainer();
            Assert.IsNotNull(container);
        }

        public class TestCompositionContainerBuilder : CompositionContainerBuilderBase<TestCompositionContainerBuilder>
        {
            public TestCompositionContainerBuilder(IAmbientServices ambientServices = null)
                : base(new CompositionRegistrationContext(ambientServices ?? new AmbientServices().WithStaticAppRuntime()))
            {
            }

            public TestCompositionContainerBuilder(ILogManager logManager, IAppRuntime appRuntime)
                : base(new CompositionRegistrationContext(new AmbientServices().Register(logManager).Register(appRuntime)))
            {
            }

            public IConventionsBuilder InternalConventionsBuilder
            {
                get { return this.ConventionsBuilder; }
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
            public IPartBuilder ForInstance(Type type, object instance)
            {
                return Substitute.For<IPartBuilder>();
            }

            /// <summary>
            /// Defines a registration for the specified type and its instance factory.
            /// </summary>
            /// <param name="type">The registered service type.</param>
            /// <param name="factory">The service factory.</param>
            /// <returns>A <see cref="IPartBuilder"/> to further configure the rule.</returns>
            public IPartBuilder ForInstanceFactory(Type type, Func<ICompositionContext, object> factory)
            {
                // throw new NotImplementedException();
                return Substitute.For<IPartBuilder>();
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
            }

            public TestPartConventionsBuilder(Predicate<Type> typePredicate)
            {
                this.TypePredicate = typePredicate;
                this.ExportBuilder = new TestExportConventionsBuilder();
            }

            public TestExportConventionsBuilder ExportBuilder { get; set; }

            public Type Type { get; set; }

            public Type ServiceType { get; set; }

            public Predicate<Type> TypePredicate { get; set; }

            public bool IsSingleton { get; set; }

            public bool IsScoped { get; set; }

            public bool AllowMultiple { get; set; }

            public IPartConventionsBuilder AsServiceType(Type serviceType)
            {
                this.ServiceType = serviceType;
                return this;
            }

            public IPartConventionsBuilder Singleton()
            {
                this.IsSingleton = true;
                return this;
            }

            /// <summary>
            /// Mark the part as being shared within the scope.
            /// </summary>
            /// <returns>A part builder allowing further configuration of the part.</returns>
            public IPartConventionsBuilder Scoped()
            {
                this.IsScoped = true;
                return this;
            }

            public Func<IEnumerable<ConstructorInfo>, ConstructorInfo?> ConstructorSelector { get; private set; }

            public IPartConventionsBuilder Export(Action<IExportConventionsBuilder>? conventionsBuilder = null)
            {
                conventionsBuilder?.Invoke(this.ExportBuilder);

                return this;
            }

            public IPartConventionsBuilder ExportInterface(
                Type exportInterface,
                Action<Type, IExportConventionsBuilder>? exportConfiguration = null)
            {
                return this;
            }

            public IPartConventionsBuilder SelectConstructor(Func<IEnumerable<ConstructorInfo>, ConstructorInfo?> constructorSelector, Action<ParameterInfo, IImportConventionsBuilder>? importConfiguration = null)
            {
                this.ConstructorSelector = constructorSelector;
                return this;
            }

            IPartConventionsBuilder IPartConventionsBuilder.AllowMultiple(bool value)
            {
                this.AllowMultiple = value;
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