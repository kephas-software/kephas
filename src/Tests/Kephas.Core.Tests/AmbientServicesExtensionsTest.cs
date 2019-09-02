// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AmbientServicesExtensionsTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
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
            var ambientServices = new AmbientServices();
            ambientServices.WithLogManager(new DebugLogManager());

            Assert.IsTrue(ambientServices.LogManager is DebugLogManager);
        }

        [Test]
        public void WithCompositionContainer_builder()
        {
            var ambientServices = new AmbientServices();
            var compositionContext = Substitute.For<ICompositionContext>();
            ambientServices.WithCompositionContainer<TestCompositionContainerBuilder>(
                b => b.WithAssembly(this.GetType().Assembly)
                    .WithCompositionContext(compositionContext));

            Assert.AreSame(compositionContext, ambientServices.CompositionContainer);
        }

        [Test]
        public void WithCompositionContainer_builder_missing_required_constructor()
        {
            var ambientServices = new AmbientServices();
            Assert.Throws<InvalidOperationException>(() => ambientServices.WithCompositionContainer<BadTestCompositionContainerBuilder>());
        }

        public class TestCompositionContainerBuilder : CompositionContainerBuilderBase<TestCompositionContainerBuilder>
        {
            private ICompositionContext compositionContext;

            /// <summary>
            /// Initializes a new instance of the <see cref="CompositionContainerBuilderBase{TBuilder}"/> class.
            /// </summary>
            /// <param name="context">The context.</param>
            public TestCompositionContainerBuilder(ICompositionRegistrationContext context)
                : base(context)
            {
            }

            public TestCompositionContainerBuilder WithCompositionContext(ICompositionContext compositionContext)
            {
                this.compositionContext = compositionContext;
                return this;
            }

            protected override IConventionsBuilder CreateConventionsBuilder()
            {
                return Substitute.For<IConventionsBuilder>();
            }

            protected override ICompositionContext CreateContainerCore(IConventionsBuilder conventions, IEnumerable<Type> parts)
            {
                return this.compositionContext ?? Substitute.For<ICompositionContext>();
            }
        }

        /// <summary>
        /// Missing required constructor with parameter of type ICompositionContainerBuilderContext.
        /// </summary>
        public class BadTestCompositionContainerBuilder : CompositionContainerBuilderBase<AmbientServicesExtensionsTest.BadTestCompositionContainerBuilder>
        {
            public BadTestCompositionContainerBuilder()
                : base(Substitute.For<ICompositionRegistrationContext>())
            {
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
