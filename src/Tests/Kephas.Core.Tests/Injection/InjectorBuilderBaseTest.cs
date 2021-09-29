// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InjectorBuilderBaseTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Test class for <see cref="InjectorBuilderBase{TBuilder}" />.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests.Injection
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;

    using Kephas.Application;
    using Kephas.Injection;
    using Kephas.Injection.Conventions;
    using Kephas.Injection.Hosting;
    using Kephas.Logging;
    using Kephas.Reflection;
    using NSubstitute;
    using NUnit.Framework;

    /// <summary>
    /// Test class for <see cref="InjectorBuilderBase{TBuilder}"/>.
    /// </summary>
    [TestFixture]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public class InjectorBuilderBaseTest
    {
        [Test]
        public void Constructor_success()
        {
            var logManager = Substitute.For<ILogManager>();
            var appRuntime = Substitute.For<IAppRuntime>();
            var builder = new TestInjectorBuilder(logManager, appRuntime);

            Assert.AreEqual(logManager, builder.LogManager);
            Assert.AreEqual(appRuntime, builder.AppRuntime);
        }

        [Test]
        public void WithConventionsBuilder()
        {
            var conventions = Substitute.For<IConventionsBuilder>();
            var builder = new TestInjectorBuilder()
                .WithConventions(conventions);

            Assert.AreSame(conventions, builder.InternalConventionsBuilder);
        }

        [Test]
        public void Build_success()
        {
            var ambientServices = new AmbientServices(registerDefaultServices: false)
                .Register(Substitute.For<ILogManager>())
                .Register(Substitute.For<ITypeLoader>())
                .Register(Substitute.For<IAppRuntime>());
            var builder = new TestInjectorBuilder(ambientServices)
                .WithAssemblies(new[] { this.GetType().Assembly });

            var container = builder.Build();
            Assert.IsNotNull(container);
        }

        public class TestInjectorBuilder : InjectorBuilderBase<TestInjectorBuilder>
        {
            public TestInjectorBuilder(IAmbientServices? ambientServices = null)
                : base(new InjectionBuildContext(ambientServices ?? new AmbientServices().WithStaticAppRuntime()))
            {
            }

            public TestInjectorBuilder(ILogManager logManager, IAppRuntime appRuntime)
                : base(new InjectionBuildContext(new AmbientServices().Register(logManager).Register(appRuntime)))
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

            protected override IInjector CreateInjectorCore(IConventionsBuilder conventions)
            {
                return Substitute.For<IInjector>();
            }
        }

        public class TestConventionsBuilder : IConventionsBuilder
        {
            public TestConventionsBuilder()
            {
                this.DerivedConventionsBuilders = new Dictionary<Type, IPartBuilder>();
                this.TypeConventionsBuilders = new Dictionary<Type, IPartBuilder>();
                this.MatchingConventionsBuilders = new Dictionary<Predicate<Type>, IPartBuilder>();
            }

            public IDictionary<Type, IPartBuilder> DerivedConventionsBuilders { get; private set; }

            public IDictionary<Type, IPartBuilder> TypeConventionsBuilders { get; private set; }

            public IDictionary<Predicate<Type>, IPartBuilder> MatchingConventionsBuilders { get; private set; }

            public IPartBuilder ForType(Type type)
            {
                var partBuilder = this.CreateBuilder(type);
                this.TypeConventionsBuilders.Add(type, partBuilder);
                return partBuilder;
            }

            /// <summary>
            /// Defines a registration for the specified type and its singleton instance.
            /// </summary>
            /// <param name="instance">The instance.</param>
            public IPartBuilder ForInstance(object instance)
            {
                return Substitute.For<IPartBuilder>();
            }

            /// <summary>
            /// Defines a registration for the specified type and its instance factory.
            /// </summary>
            /// <param name="type">The registered service type.</param>
            /// <param name="factory">The service factory.</param>
            /// <returns>A <see cref="IPartBuilder"/> to further configure the rule.</returns>
            public IPartBuilder ForFactory(Type type, Func<IInjector, object> factory)
            {
                // throw new NotImplementedException();
                return Substitute.For<IPartBuilder>();
            }

            private IPartBuilder CreateBuilder(Type type)
            {
                return new TestPartConventionsBuilder(type);
            }
        }

        public class TestPartConventionsBuilder : IPartBuilder
        {
            public TestPartConventionsBuilder(Type type)
            {
                this.Type = type;
            }

            public Type Type { get; set; }

            public Type ContractType { get; set; }

            public bool IsSingleton { get; set; }

            public bool IsScoped { get; set; }

            public bool AllowMultiple { get; set; }

            public IPartBuilder As(Type contractType)
            {
                this.ContractType = contractType;
                return this;
            }

            public IDictionary<string, object?>? Metadata { get; set; }

            public IPartBuilder Singleton()
            {
                this.IsSingleton = true;
                return this;
            }

            /// <summary>
            /// Mark the part as being shared within the scope.
            /// </summary>
            /// <returns>A part builder allowing further configuration of the part.</returns>
            public IPartBuilder Scoped()
            {
                this.IsScoped = true;
                return this;
            }

            public Func<IEnumerable<ConstructorInfo>, ConstructorInfo?> ConstructorSelector { get; private set; }

            public IPartBuilder SelectConstructor(Func<IEnumerable<ConstructorInfo>, ConstructorInfo?> constructorSelector, Action<ParameterInfo, IImportConventionsBuilder>? importConfiguration = null)
            {
                this.ConstructorSelector = constructorSelector;
                return this;
            }

            public IPartBuilder AddMetadata(string name, object? value)
            {
                (this.Metadata ??= new Dictionary<string, object?>())[name] = value;
                return this;
            }

            IPartBuilder IPartBuilder.AllowMultiple(bool value)
            {
                this.AllowMultiple = value;
                return this;
            }
        }
    }
}