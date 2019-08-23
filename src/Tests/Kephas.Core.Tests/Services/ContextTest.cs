// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContextTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Test class for <see cref="Context" />.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests.Services
{
    using System;
    using System.Security;
    using System.Security.Principal;

    using Kephas.Composition;
    using Kephas.Services;

    using NSubstitute;

    using NUnit.Framework;

    /// <summary>
    /// Test class for <see cref="Context"/>.
    /// </summary>
    [TestFixture]
    public class ContextTest
    {
        [Test]
        public void Dynamic_Context()
        {
            dynamic context = new Context();
            context.Value = 12;
            Assert.AreEqual(12, context.Value);

            var mockUser = Substitute.For<IIdentity>();
            context.Identity = mockUser;
            Assert.AreSame(mockUser, context.Identity);

            var contextBase = (Context)context;
            Assert.AreSame(mockUser, contextBase.Identity);

            Assert.AreSame(mockUser, contextBase["Identity"]);
            Assert.AreEqual(12, contextBase["Value"]);
        }

        [Test]
        public void Constructor_sync_composition_context_and_ambient_services()
        {
            var compositionContext = Substitute.For<ICompositionContext>();
            var ambientServices = Substitute.For<IAmbientServices>();
            compositionContext.GetExport<IAmbientServices>(Arg.Any<string>()).Returns(ambientServices);
            var context = new Context(compositionContext);
            Assert.AreSame(ambientServices, context.AmbientServices);
        }

        [Test]
        public void Constructor_sync_ambient_services_and_composition_context()
        {
            var compositionContext = Substitute.For<ICompositionContext>();
            var ambientServices = Substitute.For<IAmbientServices>();
            ambientServices.CompositionContainer.Returns(compositionContext);
            var context = new Context(ambientServices);
            Assert.AreSame(compositionContext, context.CompositionContext);
        }

        [Test]
        public void Identity_can_be_set_once()
        {
            var context = new Context();
            context.Identity = Substitute.For<IIdentity>();

            Assert.Throws<SecurityException>(() => context.Identity = Substitute.For<IIdentity>());
        }

        [Test]
        public void Identity_can_be_set_multiple_when_overridable()
        {
            var context = new OverrideIdentityContext();
            context.Identity = Substitute.For<IIdentity>();

            var newIdentity = Substitute.For<IIdentity>();
            context.Identity = newIdentity;

            Assert.AreSame(newIdentity, context.Identity);
        }

        [Test]
        public void WithDisposableResource()
        {
            var disposable = Substitute.For<IDisposable>();
            using (var context = new Context())
            {
                context.WithDisposableResource(disposable);
            }

            disposable.Received(1).Dispose();
        }

        [Test]
        public void DisposeResources_called_once()
        {
            var disposable = Substitute.For<IDisposable>();
            var context = new Context();
            context.WithDisposableResource(disposable);

            context.DisposeResources();
            context.DisposeResources();

            disposable.Received(1).Dispose();
        }

        public class OverrideIdentityContext : Context
        {
            /// <summary>
            /// Validates the identity before changing it.
            /// </summary>
            /// <param name="currentValue">The current value.</param>
            /// <param name="newValue">The new value.</param>
            /// <returns>
            /// True if validation succeeds, false if it fails.
            /// </returns>
            protected override bool ValidateIdentity(IIdentity currentValue, IIdentity newValue)
            {
                return true;
            }
        }
    }
}
