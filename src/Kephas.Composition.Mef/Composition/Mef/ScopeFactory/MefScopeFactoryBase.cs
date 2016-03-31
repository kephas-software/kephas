// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MefScopeFactoryBase.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the MEF scope provider class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Mef.ScopeFactory
{
    using System.Composition;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// A MEF scope provider.
    /// </summary>
    public abstract class MefScopeFactoryBase : IMefScopeFactory
    {
        private readonly ExportFactory<CompositionContext> scopedContextFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="MefScopeFactoryBase"/> class.
        /// </summary>
        /// <param name="scopedContextFactory">The scoped context factory.</param>
        protected MefScopeFactoryBase(ExportFactory<CompositionContext> scopedContextFactory)
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
        public virtual Export<CompositionContext> CreateScopedContextExport()
        {
            return this.scopedContextFactory.CreateExport();
        }
    }
}
