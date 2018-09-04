// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MefScopedCompositionContextTest.cs" company="Kephas Software SRL">
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

    using Kephas.Composition.Mef.Hosting;
    using Kephas.Testing.Composition.Mef;

    using NUnit.Framework;

    /// <summary>
    /// Tests for <see cref="MefScopedCompositionContext"/>.
    /// </summary>
    [TestFixture]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public class MefScopedCompositionContextTest : CompositionTestBase
    {
        [Test]
        public void CreateScopedContext_NestedScopes()
        {
            var container = this.CreateContainerWithBuilder(typeof(MefCompositionContainerTest.ScopeExportedClass));
            using (var scopedContext = container.CreateScopedContext())
            {
                Assert.IsInstanceOf<MefScopedCompositionContext>(scopedContext);
                var scopedInstance1 = scopedContext.GetExport<MefCompositionContainerTest.ScopeExportedClass>();

                using (var nestedContext = scopedContext.CreateScopedContext())
                {
                    Assert.AreNotSame(scopedContext, nestedContext);

                    var scopedInstance2 = nestedContext.GetExport<MefCompositionContainerTest.ScopeExportedClass>();
                    Assert.AreNotSame(scopedInstance1, scopedInstance2);
                }
            }
        }
    }
}