// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMefScopeFactory.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the IMefScopeFactory interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Mef.ScopeFactory
{
    using System.Composition;

    /// <summary>
    /// Interface for MEF scope factory.
    /// </summary>
    public interface IMefScopeFactory
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