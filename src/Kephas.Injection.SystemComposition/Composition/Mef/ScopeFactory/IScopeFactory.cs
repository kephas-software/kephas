// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IScopeFactory.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IScopeFactory interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Mef.ScopeFactory
{
    using System.Composition;

    /// <summary>
    /// Interface for MEF scope factory.
    /// </summary>
    public interface IScopeFactory
    {
        /// <summary>
        /// Creates the scoped context export.
        /// </summary>
        /// <returns>
        /// The new scoped context export.
        /// </returns>
        Export<CompositionContext> CreateScopedContextExport();
    }
}