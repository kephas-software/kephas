// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScopedSystemCompositionInjectorTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the MEF scoped composition context test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Tests.Composition.Mef
{
    using System.Diagnostics.CodeAnalysis;

    using Kephas.Composition;
    using Kephas.Composition.Mef.Hosting;
    using Kephas.Injection;
    using Kephas.Testing.Composition;
    using NUnit.Framework;

    /// <summary>
    /// Tests for <see cref="ScopedSystemCompositionInjector"/>.
    /// </summary>
    [TestFixture]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public class ScopedSystemCompositionInjectorTest : SystemCompositionTestBase
    {
        [Test]
        public void CreateScopedContext_NestedScopes()
        {
            var container = this.CreateContainerWithBuilder(typeof(SystemCompositionContainerTest.ScopeExportedClass));
            using var scopedContext = container.CreateScopedInjector();
            Assert.IsInstanceOf<ScopedSystemCompositionInjector>(scopedContext);
            var scopedInstance1 = scopedContext.Resolve<SystemCompositionContainerTest.ScopeExportedClass>();

            using var nestedContext = scopedContext.CreateScopedInjector();
            Assert.AreNotSame(scopedContext, nestedContext);

            var scopedInstance2 = nestedContext.Resolve<SystemCompositionContainerTest.ScopeExportedClass>();
            Assert.AreNotSame(scopedInstance1, scopedInstance2);
        }

        [Test]
        public void CreateScopedContext_CompositionContext_registration_root()
        {
            var container = this.CreateContainerWithBuilder();
            var selfContainer = container.Resolve<IInjector>();
            Assert.AreSame(container, selfContainer);
        }

        [Test]
        public void CreateScopedContext_CompositionContext_registration_scope()
        {
            var container = this.CreateContainerWithBuilder();
            using (var scopedContext = container.CreateScopedInjector())
            {
                var selfScopedContext = scopedContext.Resolve<IInjector>();
                Assert.AreSame(selfScopedContext, scopedContext);
            }
        }

        [Test]
        public void CreateScopedContext_CompositionContext_registration_scope_consumers()
        {
            var container = this.CreateContainerWithBuilder(typeof(SystemCompositionContainerTest.ScopeExportedClass));
            using (var scopedContext = container.CreateScopedInjector())
            {
                var scopedInstance = scopedContext.Resolve<SystemCompositionContainerTest.ScopeExportedClass>();
                Assert.AreSame(scopedContext, scopedInstance.Injector);
            }
        }
    }
}