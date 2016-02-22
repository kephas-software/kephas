// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MefScopeProvider.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the MEF scope provider class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Mef.Internals
{
    using System.Composition;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// A MEF scope provider.
    /// </summary>
    public class MefScopeProvider
    {
        private readonly ExportFactory<CompositionContext> scopedContextFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="MefScopeProvider"/> class.
        /// </summary>
        /// <param name="scopedContextFactory">The scoped context factory.</param>
        [ImportingConstructor]
        public MefScopeProvider([SharingBoundary(SharingBoundaries.Scope)] ExportFactory<CompositionContext> scopedContextFactory)
        {
            Contract.Requires(scopedContextFactory != null);

            this.scopedContextFactory = scopedContextFactory;
        }

        /// <summary>
        /// Creates the scoped context export.
        /// </summary>
        /// <returns>
        /// The new scoped context export.
        /// </returns>
        internal Export<CompositionContext> CreateScopedContextExport()
        {
            return this.scopedContextFactory.CreateExport();
        }
    }
}
