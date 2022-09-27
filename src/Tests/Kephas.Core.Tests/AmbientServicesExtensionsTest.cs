// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AmbientServicesExtensionsTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Test class for <see cref="AmbientServicesBuilder" />.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Injection;

namespace Kephas.Core.Tests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using Kephas.Application;
    using Kephas.Diagnostics.Logging;
    using Kephas.Injection.Builder;
    using Kephas.Logging;
    using Kephas.Reflection;
    using Kephas.Testing;
    using NSubstitute;

    using NUnit.Framework;

    /// <summary>
    /// Test class for <see cref="AmbientServicesExtensions"/>.
    /// </summary>
    [TestFixture]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public class AmbientServicesExtensionsTest : TestBase
    {
        [Test]
        public void Constructor_register_ambient_services()
        {
            var ambientServices = this.CreateAmbientServices();

            Assert.AreSame(ambientServices, ambientServices.GetServiceInstance(typeof(IAmbientServices)));
        }

        [Test]
        public void WithLogManager_success()
        {
            IAmbientServices ambientServices = this.CreateAmbientServices();
            ambientServices.WithLogManager(new DebugLogManager());

            Assert.IsTrue(ambientServices.GetServiceInstance<ILogManager>() is DebugLogManager);
        }

        [Test]
        public void WithInjector_builder()
        {
            var ambientServices = new AmbientServices(registerDefaultServices: false)
                .Add(Substitute.For<ILogManager>())
                .Add(Substitute.For<ITypeLoader>())
                .Add(Substitute.For<IAppRuntime>());
            var injector = Substitute.For<IServiceProvider>();
            var builderInjector = ambientServices.BuildWith<TestInjectorBuilder>(
                b => b.WithAssemblies(this.GetType().Assembly)
                    .WithInjector(injector));

            Assert.AreSame(injector, builderInjector);
        }

        [Test]
        public void WithInjector_builder_missing_required_constructor()
        {
            var ambientServices = this.CreateAmbientServices();
            Assert.Throws<MissingMethodException>(() => ambientServices.BuildWith<BadTestInjectorBuilder>());
        }

        public class TestInjectorBuilder : InjectorBuilderBase<TestInjectorBuilder>
        {
            private IServiceProvider serviceProvider;

            public TestInjectorBuilder(IInjectionBuildContext context)
                : base(context)
            {
            }

            public TestInjectorBuilder WithInjector(IServiceProvider serviceProvider)
            {
                this.serviceProvider = serviceProvider;
                return this;
            }

            public override IRegistrationBuilder ForType(Type type) => Substitute.For<IRegistrationBuilder>();

            public override IRegistrationBuilder ForInstance(object instance) => Substitute.For<IRegistrationBuilder>();

            public override IRegistrationBuilder ForFactory(Type type, Func<IServiceProvider, object> factory) => Substitute.For<IRegistrationBuilder>();

            protected override IServiceProvider CreateInjectorCore()
            {
                return this.serviceProvider ?? Substitute.For<IServiceProvider>();
            }
        }

        /// <summary>
        /// Missing required constructor with parameter of type ICompositionContainerBuilderContext.
        /// </summary>
        public class BadTestInjectorBuilder : InjectorBuilderBase<BadTestInjectorBuilder>
        {
            public BadTestInjectorBuilder()
                : base(Substitute.For<IInjectionBuildContext>())
            {
            }

            public override IRegistrationBuilder ForType(Type type) => Substitute.For<IRegistrationBuilder>();

            public override IRegistrationBuilder ForInstance(object instance) => Substitute.For<IRegistrationBuilder>();

            public override IRegistrationBuilder ForFactory(Type type, Func<IServiceProvider, object> factory) => Substitute.For<IRegistrationBuilder>();

            protected override IServiceProvider CreateInjectorCore()
            {
                return Substitute.For<IServiceProvider>();
            }
        }
    }
}
