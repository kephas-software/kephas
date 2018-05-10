// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICompositionContextAware.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the ICompositionContextAware interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition
{
    /// <summary>
    /// Interface for components being aware of the composition context within they live.
    /// </summary>
    public interface ICompositionContextAware
    {
        /// <summary>
        /// Gets a context for the dependency injection/composition.
        /// </summary>
        /// <value>
        /// The composition context.
        /// </value>
        ICompositionContext CompositionContext { get; }
    }
}