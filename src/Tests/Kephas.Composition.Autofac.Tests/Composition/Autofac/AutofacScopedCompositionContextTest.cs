﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AutofacScopedCompositionContextTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the MEF scoped composition context test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Tests.Composition.Autofac
{
    using System.Diagnostics.CodeAnalysis;

    using Kephas.Composition;
    using Kephas.Composition.Autofac.Hosting;
    using Kephas.Services;
    using Kephas.Services.Reflection;
    using Kephas.Testing.Composition;

    using NUnit.Framework;

    /// <summary>
    /// Tests for <see cref="AutofacScopedCompositionContext"/>.
    /// </summary>
    [TestFixture]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public class AutofacScopedCompositionContextTest : AutofacCompositionTestBase
    {
        [Test]
        public void CreateScopedContext_NestedScopes()
        {
            var container = this.CreateContainerWithBuilder(
                b => b.WithRegistration(
                    new AppServiceInfo(
                        typeof(AutofacCompositionContainerTest.ScopeExportedClass),
                        typeof(AutofacCompositionContainerTest.ScopeExportedClass),
                        AppServiceLifetime.Scoped)));
            using (var scopedContext = container.CreateScopedContext())
            {
                Assert.IsInstanceOf<AutofacScopedCompositionContext>(scopedContext);
                var scopedInstance1 = scopedContext.GetExport<AutofacCompositionContainerTest.ScopeExportedClass>();

                using (var nestedContext = scopedContext.CreateScopedContext())
                {
                    Assert.AreNotSame(scopedContext, nestedContext);

                    var scopedInstance2 = nestedContext.GetExport<AutofacCompositionContainerTest.ScopeExportedClass>();
                    Assert.AreNotSame(scopedInstance1, scopedInstance2);
                }
            }
        }

        [Test]
        public void CreateScopedContext_CompositionContext_registration_root()
        {
            var container = this.CreateContainerWithBuilder();
            var selfContainer = container.GetExport<ICompositionContext>();
            Assert.AreSame(container, selfContainer);
        }

        [Test]
        public void CreateScopedContext_CompositionContext_registration_scope()
        {
            var container = this.CreateContainerWithBuilder();
            using (var scopedContext = container.CreateScopedContext())
            {
                var selfScopedContext = scopedContext.GetExport<ICompositionContext>();
                Assert.AreSame(selfScopedContext, scopedContext);
            }
        }

        [Test]
        public void CreateScopedContext_CompositionContext_registration_scope_consumers()
        {
            var container = this.CreateContainerWithBuilder(
                b => b.WithRegistration(
                    new AppServiceInfo(
                        typeof(AutofacCompositionContainerTest.ScopeExportedClass),
                        typeof(AutofacCompositionContainerTest.ScopeExportedClass),
                        AppServiceLifetime.Scoped)));
            using (var scopedContext = container.CreateScopedContext())
            {
                var scopedInstance = scopedContext.GetExport<AutofacCompositionContainerTest.ScopeExportedClass>();
                Assert.AreSame(scopedContext, scopedInstance.CompositionContext);
            }
        }
    }
}