// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICompositionContainerBuilder.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the ICompositionContainerBuilder interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Hosting
{
    using System.Threading.Tasks;

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

        /// <summary>
        /// Creates the container with the provided configuration asynchronously.
        /// </summary>
        /// <returns>A new container with the provided configuration.</returns>
        Task<ICompositionContext> CreateContainerAsync();
    }
}