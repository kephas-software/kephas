// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScopedSystemCompositionInjectorTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the MEF scoped injector test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Tests.Injection.SystemComposition
{
    using System.Diagnostics.CodeAnalysis;

    using Kephas.Injection;
    using Kephas.Injection.SystemComposition;
    using Kephas.Testing.Injection;
    using NUnit.Framework;

    /// <summary>
    /// Tests for <see cref="ScopedSystemCompositionInjector"/>.
    /// </summary>
    [TestFixture]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public class ScopedSystemCompositionInjectorTest : SystemCompositionInjectionTestBase
    {
        [Test]
        public void CreateScopedInjector_NestedScopes()
        {
            var container = this.CreateInjectorWithBuilder(typeof(SystemCompositionInjectionContainerTest.ScopeExportedClass));
            using var scopedContext = container.CreateScopedInjector();
            Assert.IsInstanceOf<ScopedSystemCompositionInjector>(scopedContext);
            var scopedInstance1 = scopedContext.Resolve<SystemCompositionInjectionContainerTest.ScopeExportedClass>();

            using var nestedContext = scopedContext.CreateScopedInjector();
            Assert.AreNotSame(scopedContext, nestedContext);

            var scopedInstance2 = nestedContext.Resolve<SystemCompositionInjectionContainerTest.ScopeExportedClass>();
            Assert.AreNotSame(scopedInstance1, scopedInstance2);
        }

        [Test]
        public void CreateScopedInjector_Injector_registration_root()
        {
            var container = this.CreateInjectorWithBuilder();
            var selfContainer = container.Resolve<IInjector>();
            Assert.AreSame(container, selfContainer);
        }

        [Test]
        public void CreateScopedInjector_Injector_registration_scope()
        {
            var container = this.CreateInjectorWithBuilder();
            using (var scopedContext = container.CreateScopedInjector())
            {
                var selfScopedContext = scopedContext.Resolve<IInjector>();
                Assert.AreSame(selfScopedContext, scopedContext);
            }
        }

        [Test]
        public void CreateScopedInjector_Injector_registration_scope_consumers()
        {
            var container = this.CreateInjectorWithBuilder(typeof(SystemCompositionInjectionContainerTest.ScopeExportedClass));
            using (var scopedContext = container.CreateScopedInjector())
            {
                var scopedInstance = scopedContext.Resolve<SystemCompositionInjectionContainerTest.ScopeExportedClass>();
                Assert.AreSame(scopedContext, scopedInstance.Injector);
            }
        }
    }
}