// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultScopeFactory.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the default MEF scope provider class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Injection.SystemComposition.ScopeFactory
{
    using System;
    using System.Composition;

    /// <summary>
    /// A MEF scope provider.
    /// </summary>
    [InjectionScope]
    public class DefaultScopeFactory : ScopeFactoryBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultScopeFactory"/> class.
        /// </summary>
        /// <param name="scopedContextFactory">The scoped context factory.</param>
        [ImportingConstructor]
        public DefaultScopeFactory([SharingBoundary(InjectionScopeNames.Default)] ExportFactory<CompositionContext> scopedContextFactory)
            : base(scopedContextFactory ?? throw new ArgumentNullException(nameof(scopedContextFactory)))
        {
        }
    }
}
