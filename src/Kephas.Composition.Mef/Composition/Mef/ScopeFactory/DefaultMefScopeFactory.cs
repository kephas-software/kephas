// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultMefScopeFactory.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the default MEF scope provider class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Mef.ScopeFactory
{
    using System.Composition;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// A MEF scope provider.
    /// </summary>
    [SharingBoundaryScope(ScopeNames.Default)]
    public class DefaultMefScopeFactory : MefScopeFactoryBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultMefScopeFactory"/> class.
        /// </summary>
        /// <param name="scopedContextFactory">The scoped context factory.</param>
        [ImportingConstructor]
        public DefaultMefScopeFactory([SharingBoundary(ScopeNames.Default)] ExportFactory<CompositionContext> scopedContextFactory)
            : base(scopedContextFactory)
        {
            Contract.Requires(scopedContextFactory != null);
        }
    }
}
