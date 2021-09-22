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
using Kephas.Injection.Conventions;
using Kephas.Injection.Hosting;

namespace Kephas.Core.Tests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using Kephas.Application;
    using Kephas.Diagnostics.Logging;
    using Kephas.Logging;
    using Kephas.Reflection;
    using NSubstitute;

    using NUnit.Framework;

    /// <summary>
    /// Test class for <see cref="AmbientServicesExtensions"/>.
    /// </summary>
    [TestFixture]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public class AmbientServicesExtensionsTest
    {
        [Test]
        public void Constructor_register_ambient_services()
        {
            var ambientServices = new AmbientServices();

            Assert.AreSame(ambientServices, ambientServices.GetService(typeof(IAmbientServices)));
        }

        [Test]
        public void WithLogManager_success()
        {
            IAmbientServices ambientServices = new AmbientServices();
            ambientServices.WithLogManager(new DebugLogManager());

            Assert.IsTrue(ambientServices.LogManager is DebugLogManager);
        }

        [Test]
        public void WithInjector_builder()
        {
            var ambientServices = new AmbientServices(registerDefaultServices: false)
                .Register(Substitute.For<ILogManager>())
                .Register(Substitute.For<ITypeLoader>())
                .Register(Substitute.For<IAppRuntime>());
            var injector = Substitute.For<IInjector>();
            ambientServices.WithInjector<TestInjectorBuilder>(
                b => b.WithAssembly(this.GetType().Assembly)
                    .WithInjector(injector));

            Assert.AreSame(injector, ambientServices.Injector);
        }

        [Test]
        public void WithInjector_builder_missing_required_constructor()
        {
            var ambientServices = new AmbientServices();
            Assert.Throws<InvalidOperationException>(() => ambientServices.WithInjector<BadTestInjectorBuilder>());
        }

        public class TestInjectorBuilder : InjectorBuilderBase<TestInjectorBuilder>
        {
            private IInjector injector;

            public TestInjectorBuilder(IInjectionBuildContext context)
                : base(context)
            {
            }

            public TestInjectorBuilder WithInjector(IInjector injector)
            {
                this.injector = injector;
                return this;
            }

            protected override IConventionsBuilder CreateConventionsBuilder()
            {
                return Substitute.For<IConventionsBuilder>();
            }

            protected override IInjector CreateInjectorCore(IConventionsBuilder conventions)
            {
                return this.injector ?? Substitute.For<IInjector>();
            }
        }

        /// <summary>
        /// Missing required constructor with parameter of type ICompositionContainerBuilderContext.
        /// </summary>
        public class BadTestInjectorBuilder : InjectorBuilderBase<AmbientServicesExtensionsTest.BadTestInjectorBuilder>
        {
            public BadTestInjectorBuilder()
                : base(Substitute.For<IInjectionBuildContext>())
            {
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
    }
}
