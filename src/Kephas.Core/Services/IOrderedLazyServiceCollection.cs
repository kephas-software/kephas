// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IOrderedLazyServiceCollection.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IOrderedLazyServiceCollection interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services
{
    using System;
    using System.Collections.Generic;

    using Kephas.Services.Composition;

    /// <summary>
    /// Interface for ordered lazy service collection.
    /// </summary>
    /// <typeparam name="TService">Type of the service.</typeparam>
    /// <typeparam name="TServiceMetadata">Type of the service metadata.</typeparam>
    [AppServiceContract(AsOpenGeneric = true)]
    public interface IOrderedLazyServiceCollection<TService, TServiceMetadata> : IEnumerable<Lazy<TService, TServiceMetadata>>
        where TServiceMetadata : AppServiceMetadata
    {
        /// <summary>
        /// Gets the service factories in the appropriate order.
        /// </summary>
        /// <param name="filter">Optional. Specifies a filter.</param>
        /// <returns>
        /// The ordered service factories.
        /// </returns>
        IEnumerable<Lazy<TService, TServiceMetadata>> GetServiceFactories(
            Func<Lazy<TService, TServiceMetadata>, bool> filter = null);

        /// <summary>
        /// Gets the services in the appropriate order.
        /// </summary>
        /// <param name="filter">Optional. Specifies a filter.</param>
        /// <returns>
        /// The ordered services.
        /// </returns>
        IEnumerable<TService> GetServices(
            Func<Lazy<TService, TServiceMetadata>, bool> filter = null);
    }
}