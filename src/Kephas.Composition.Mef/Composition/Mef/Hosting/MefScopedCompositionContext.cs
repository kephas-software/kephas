// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MefScopedCompositionContext.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   The MEF composition container.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Mef.Hosting
{
    using System.Composition;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// The MEF composition container.
    /// </summary>
    public class MefScopedCompositionContext : MefCompositionContextBase
    {
        /// <summary>
        /// The composition context export.
        /// </summary>
        private Export<CompositionContext> export;

        /// <summary>
        /// Initializes a new instance of the <see cref="MefScopedCompositionContext"/> class.
        /// </summary>
        /// <param name="export">The export.</param>
        public MefScopedCompositionContext(Export<CompositionContext> export)
        {
            Contract.Requires(export != null);

            this.export = export;
            this.Initialize(this.export.Value);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (this.export != null)
            {
                this.export.Dispose();
                this.export = null;
            }

            base.Dispose(disposing);
        }
    }
}