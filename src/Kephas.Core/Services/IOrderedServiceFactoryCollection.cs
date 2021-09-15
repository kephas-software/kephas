// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IOrderedServiceFactoryCollection.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IOrderedServiceFactoryCollection interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Injection;

namespace Kephas.Services
{
    using System;
    using System.Collections.Generic;
    using Kephas.Services.Composition;

    /// <summary>
    /// Interface for ordered service factory collection.
    /// </summary>
    /// <typeparam name="TService">Type of the service.</typeparam>
    /// <typeparam name="TServiceMetadata">Type of the service metadata.</typeparam>
    [AppServiceContract(AsOpenGeneric = true)]
    public interface IOrderedServiceFactoryCollection<out TService, out TServiceMetadata> : IEnumerable<IExportFactory<TService, TServiceMetadata>>
        where TServiceMetadata : AppServiceMetadata
    {
        /// <summary>
        /// Gets the service factories in the appropriate order.
        /// </summary>
        /// <param name="filter">Optional. Specifies a filter.</param>
        /// <returns>
        /// The ordered service factories.
        /// </returns>
        IEnumerable<IExportFactory<TService, TServiceMetadata>> GetServiceFactories(
            Func<IExportFactory<TService, TServiceMetadata>, bool>? filter = null);

        /// <summary>
        /// Gets the services in the appropriate order.
        /// </summary>
        /// <param name="filter">Optional. Specifies a filter.</param>
        /// <returns>
        /// The ordered services.
        /// </returns>
        IEnumerable<TService> GetServices(
            Func<IExportFactory<TService, TServiceMetadata>, bool>? filter = null);
    }
}