// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AmbientServicesBuilderTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Test class for <see cref="AmbientServicesBuilder" />.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;

    using Kephas.Composition;
    using Kephas.Composition.Conventions;
    using Kephas.Composition.Hosting;
    using Kephas.Diagnostics.Logging;
    using Kephas.Services;

    using NSubstitute;

    using NUnit.Framework;

    /// <summary>
    /// Test class for <see cref="AmbientServicesBuilder"/>.
    /// </summary>
    [TestFixture]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public class AmbientServicesBuilderTest
    {
        [Test]
        public void Constructor_register_ambient_services()
        {
            var ambientServices = new AmbientServices();
            var builder = new AmbientServicesBuilder(ambientServices);

            Assert.AreSame(ambientServices, ambientServices.GetService(typeof(IAmbientServices)));
        }

        [Test]
        public void WithLogManager_success()
        {
            var ambientServices = new AmbientServices();
            var builder = new AmbientServicesBuilder(ambientServices);
            builder.WithLogManager(new DebugLogManager());

            Assert.IsTrue(ambientServices.LogManager is DebugLogManager);
        }

        [Test]
        public void WithCompositionContainer_builder()
        {
            var ambientServices = new AmbientServices();
            var builder = new AmbientServicesBuilder(ambientServices);
            builder.WithCompositionContainer<TestCompositionContainerBuilder>(b => b.WithAssembly(this.GetType().Assembly));

            Assert.IsFalse(ambientServices.CompositionContainer is NullCompositionContainer);
        }

        [Test]
        public void WithCompositionContainer_builder_missing_required_constructor()
        {
            var ambientServices = new AmbientServices();
            var builder = new AmbientServicesBuilder(ambientServices);
            Assert.Throws<InvalidOperationException>(() => builder.WithCompositionContainer<BadTestCompositionContainerBuilder>());
        }

        [Test]
        public async Task WithCompositionContainerAsync_builder()
        {
            var ambientServices = new AmbientServices();
            var builder = new AmbientServicesBuilder(ambientServices);
            await builder.WithCompositionContainerAsync<TestCompositionContainerBuilder>(b => b.WithAssembly(this.GetType().Assembly));

            Assert.IsFalse(ambientServices.CompositionContainer is NullCompositionContainer);
        }

        [Test]
        public void WithCompositionContainerAsync_builder_missing_required_constructor()
        {
            var ambientServices = new AmbientServices();
            var builder = new AmbientServicesBuilder(ambientServices);
            Assert.That(() => builder.WithCompositionContainerAsync<BadTestCompositionContainerBuilder>(), Throws.TypeOf<InvalidOperationException>());
        }

        public class TestCompositionContainerBuilder : CompositionContainerBuilderBase<TestCompositionContainerBuilder>
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="CompositionContainerBuilderBase{TBuilder}"/> class.
            /// </summary>
            /// <param name="context">The context.</param>
            public TestCompositionContainerBuilder(ICompositionRegistrationContext context)
                : base(context)
            {
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

        /// <summary>
        /// Missing required constructor with parameter of type ICompositionContainerBuilderContext.
        /// </summary>
        public class BadTestCompositionContainerBuilder : CompositionContainerBuilderBase<AmbientServicesBuilderTest.BadTestCompositionContainerBuilder>
        {
            public BadTestCompositionContainerBuilder()
                : base(Substitute.For<ICompositionRegistrationContext>())
            {
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
    }
}
