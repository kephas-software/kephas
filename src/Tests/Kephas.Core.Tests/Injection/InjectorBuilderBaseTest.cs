﻿// --------------------------------------------------------------------------------------------------------------------
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
    using Kephas.Injection.Builder;
    using Kephas.Logging;
    using Kephas.Reflection;
    using Kephas.Runtime;
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
        public void Build_success()
        {
            var ambientServices = new AmbientServices(registerDefaultServices: false)
                .Add(Substitute.For<ILogManager>())
                .Add(Substitute.For<ITypeLoader>())
                .Add(Substitute.For<IAppRuntime>());
            var builder = new TestInjectorBuilder(ambientServices)
                .WithAssemblies(new[] { this.GetType().Assembly });

            var container = builder.Build();
            Assert.IsNotNull(container);
        }

        public class TestInjectorBuilder : InjectorBuilderBase<TestInjectorBuilder>
        {
            public TestInjectorBuilder(IAmbientServices? ambientServices = null)
                : base(new InjectionBuildContext((ambientServices ?? CreateAmbientServices().WithStaticAppRuntime()).GetAppRuntime().GetAppAssemblies()))
            {
            }

            public TestInjectorBuilder(ILogManager logManager, IAppRuntime appRuntime)
                : base(new InjectionBuildContext(CreateAmbientServices().Add(logManager).Add(appRuntime).GetAppRuntime().GetAppAssemblies()))
            {
            }

            public override IRegistrationBuilder ForType(Type type) => Substitute.For<IRegistrationBuilder>();

            public override IRegistrationBuilder ForInstance(object instance) => Substitute.For<IRegistrationBuilder>();

            public override IRegistrationBuilder ForFactory(Type type, Func<IServiceProvider, object> factory) => Substitute.For<IRegistrationBuilder>();

            protected override IServiceProvider CreateInjectorCore()
            {
                return Substitute.For<IServiceProvider>();
            }

            private static IAmbientServices CreateAmbientServices() =>
                new AmbientServices().Add<IRuntimeTypeRegistry>(new RuntimeTypeRegistry(), b => b.ExternallyOwned());
        }

        public class TestRegistrationInjectorBuilder : InjectorBuilderBase<TestRegistrationInjectorBuilder>
        {
            public TestRegistrationInjectorBuilder(IInjectionBuildContext? buildContext = null)
                : base(buildContext ?? new InjectionBuildContext(CreateAmbientServices().GetAppRuntime().GetAppAssemblies()))
            {
            }

            public IDictionary<Type, TestTypeBuilder> TypeBuilders { get; } =
                new Dictionary<Type, TestTypeBuilder>();

            public override IRegistrationBuilder ForType(Type type)
            {
                var partBuilder = this.CreateBuilder(type);
                this.TypeBuilders.Add(type, partBuilder);
                return partBuilder;
            }

            /// <summary>
            /// Defines a registration for the specified type and its singleton instance.
            /// </summary>
            /// <param name="instance">The instance.</param>
            public override IRegistrationBuilder ForInstance(object instance) => Substitute.For<IRegistrationBuilder>();

            /// <summary>
            /// Defines a registration for the specified type and its instance factory.
            /// </summary>
            /// <param name="type">The registered service type.</param>
            /// <param name="factory">The service factory.</param>
            /// <returns>A <see cref="IRegistrationBuilder"/> to further configure the rule.</returns>
            public override IRegistrationBuilder ForFactory(Type type, Func<IServiceProvider, object> factory) => Substitute.For<IRegistrationBuilder>();

            protected override IServiceProvider CreateInjectorCore() => Substitute.For<IServiceProvider>();

            private TestTypeBuilder CreateBuilder(Type serviceType) => new (serviceType);

            private static IAmbientServices CreateAmbientServices() =>
                new AmbientServices().Add<IRuntimeTypeRegistry>(new RuntimeTypeRegistry(), b => b.ExternallyOwned());
        }

        public class TestTypeBuilder : IRegistrationBuilder
        {
            public TestTypeBuilder(Type serviceType)
            {
                this.ServiceType = serviceType;
            }

            public Type ServiceType { get; }

            public Type ContractType { get; private set; }

            public bool IsSingleton { get; private set; }

            public bool IsScoped { get; set; }

            public bool AllowMultiple { get; set; }

            public IRegistrationBuilder As(Type contractType)
            {
                this.ContractType = contractType;
                return this;
            }

            public IDictionary<string, object?>? Metadata { get; set; }

            public IRegistrationBuilder Singleton()
            {
                this.IsSingleton = true;
                return this;
            }

            /// <summary>
            /// Mark the part as being shared within the scope.
            /// </summary>
            /// <returns>A part builder allowing further configuration of the part.</returns>
            public IRegistrationBuilder Scoped()
            {
                this.IsScoped = true;
                return this;
            }

            public Func<IEnumerable<ConstructorInfo>, ConstructorInfo?> ConstructorSelector { get; private set; }

            public bool IsExternallyOwned { get; set; }

            public IRegistrationBuilder SelectConstructor(
                Func<IEnumerable<ConstructorInfo>, ConstructorInfo?> constructorSelector,
                Action<ParameterInfo, IParameterBuilder>? parameterBuilder = null)
            {
                this.ConstructorSelector = constructorSelector;
                return this;
            }

            public IRegistrationBuilder AddMetadata(string name, object? value)
            {
                (this.Metadata ??= new Dictionary<string, object?>())[name] = value;
                return this;
            }

            public IRegistrationBuilder ExternallyOwned()
            {
                this.IsExternallyOwned = true;
                return this;
            }

            IRegistrationBuilder IRegistrationBuilder.AllowMultiple(bool value)
            {
                this.AllowMultiple = value;
                return this;
            }
        }
    }
}