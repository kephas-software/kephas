// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMediServiceCollectionProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IMediServiceCollectionProvider interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Medi.Conventions
{
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Interface for Microsoft.Extensions.DependencyInjection service collection provider.
    /// </summary>
    public interface IMediServiceCollectionProvider
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