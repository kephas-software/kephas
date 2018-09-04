// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICompositionContainerBuilder.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the ICompositionContainerBuilder interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Hosting
{
    /// <summary>
    /// Contract for composition container builders.
    /// </summary>
    public interface ICompositionContainerBuilder
    {
        /// <summary>
        /// Creates the container with the provided configuration asynchronously.
        /// </summary>
        /// <returns>A new container with the provided configuration.</returns>
        ICompositionContext CreateContainer();
    }
}