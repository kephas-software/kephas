// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IServiceCollectionProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IServiceCollectionProvider interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.DependencyInjection.Conventions
{
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Interface for Microsoft.Extensions.DependencyInjection service collection provider.
    /// </summary>
    public interface IServiceCollectionProvider
    {
        /// <summary>
        /// Gets service collection.
        /// </summary>
        /// <returns>
        /// The service collection.
        /// </returns>
        IServiceCollection GetServiceCollection();
    }
}