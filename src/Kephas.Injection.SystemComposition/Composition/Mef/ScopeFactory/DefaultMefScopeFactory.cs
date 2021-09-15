// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultMefScopeFactory.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the default MEF scope provider class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Mef.ScopeFactory
{
    using System.Composition;

    using Kephas.Diagnostics.Contracts;

    /// <summary>
    /// A MEF scope provider.
    /// </summary>
    [InjectionScope]
    public class DefaultMefScopeFactory : MefScopeFactoryBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultMefScopeFactory"/> class.
        /// </summary>
        /// <param name="scopedContextFactory">The scoped context factory.</param>
        [ImportingConstructor]
        public DefaultMefScopeFactory([SharingBoundary(InjectionScopeNames.Default)] ExportFactory<CompositionContext> scopedContextFactory)
            : base(scopedContextFactory)
        {
            Requires.NotNull(scopedContextFactory, nameof(scopedContextFactory));
        }
    }
}
