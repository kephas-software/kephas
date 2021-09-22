// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScopedSystemCompositionInjector.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Injection.SystemComposition.Hosting
{
    using System.Composition;
    using Kephas.Diagnostics.Contracts;

    /// <summary>
    /// The MEF injector.
    /// </summary>
    public class ScopedSystemCompositionInjector : SystemCompositionInjectorBase
    {
        /// <summary>
        /// The injector export.
        /// </summary>
        private Export<CompositionContext> export;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScopedSystemCompositionInjector"/> class.
        /// </summary>
        /// <param name="export">The export.</param>
        public ScopedSystemCompositionInjector(Export<CompositionContext> export)
        {
            Requires.NotNull(export, nameof(export));

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