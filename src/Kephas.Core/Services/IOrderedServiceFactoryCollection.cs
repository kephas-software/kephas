// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IOrderedServiceFactoryCollection.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IOrderedServiceFactoryCollection interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services
{
    using System;
    using System.Collections.Generic;

    using Kephas.Injection;

    /// <summary>
    /// Interface for ordered service factory collection.
    /// </summary>
    /// <typeparam name="TContract">Type of the service contract.</typeparam>
    /// <typeparam name="TMetadata">Type of the service metadata.</typeparam>
    [AppServiceContract(AsOpenGeneric = true)]
    public interface IOrderedServiceFactoryCollection<out TContract, out TMetadata> : IEnumerable<IExportFactory<TContract, TMetadata>>
        where TMetadata : AppServiceMetadata
    {
        /// <summary>
        /// Gets the service factories in the appropriate order.
        /// </summary>
        /// <param name="filter">Optional. Specifies a filter.</param>
        /// <returns>
        /// The ordered service factories.
        /// </returns>
        IEnumerable<IExportFactory<TContract, TMetadata>> GetServiceFactories(
            Func<IExportFactory<TContract, TMetadata>, bool>? filter = null);

        /// <summary>
        /// Gets the services in the appropriate order.
        /// </summary>
        /// <param name="filter">Optional. Specifies a filter.</param>
        /// <returns>
        /// The ordered services.
        /// </returns>
        IEnumerable<TContract> GetServices(
            Func<IExportFactory<TContract, TMetadata>, bool>? filter = null);
    }
}