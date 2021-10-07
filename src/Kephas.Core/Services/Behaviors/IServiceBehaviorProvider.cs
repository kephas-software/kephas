// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IServiceBehaviorProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IServiceBehaviorProvider interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Injection;

namespace Kephas.Services.Behaviors
{
    using System.Collections.Generic;

    /// <summary>
    /// Interface for service behavior provider.
    /// </summary>
    [SingletonAppServiceContract]
    public interface IServiceBehaviorProvider
    {
        /// <summary>
        /// Filters the enabled services from the provided services collection.
        /// </summary>
        /// <typeparam name="TContract">Type of the service contract.</typeparam>
        /// <param name="services">The services.</param>
        /// <param name="context">Context for the enabled check (optional).</param>
        /// <returns>
        /// An enumeration of enabled services.
        /// </returns>
        IEnumerable<TContract> WhereEnabled<TContract>(
            IEnumerable<TContract> services,
            IContext? context = null)
            where TContract : class;

        /// <summary>
        /// Filters the enabled services from the provided services collection.
        /// </summary>
        /// <typeparam name="TContract">Type of the service contract.</typeparam>
        /// <param name="serviceFactories">The service export factories.</param>
        /// <param name="context">Context for the enabled check (optional).</param>
        /// <returns>
        /// An enumeration of enabled services.
        /// </returns>
        IEnumerable<IExportFactory<TContract>> WhereEnabled<TContract>(
            IEnumerable<IExportFactory<TContract>> serviceFactories,
            IContext? context = null)
            where TContract : class;

        /// <summary>
        /// Enumerates the enabled services from the provided services collection.
        /// </summary>
        /// <typeparam name="TContract">Type of the service contract.</typeparam>
        /// <typeparam name="TMetadata">Type of the service metadata.</typeparam>
        /// <param name="serviceFactories">The service export factories.</param>
        /// <param name="context">Context for the enabled check (optional).</param>
        /// <returns>
        /// An enumerator that allows foreach to be used to process where enabled in this collection.
        /// </returns>
        IEnumerable<IExportFactory<TContract, TMetadata>> WhereEnabled<TContract, TMetadata>(
            IEnumerable<IExportFactory<TContract, TMetadata>> serviceFactories,
            IContext? context = null)
            where TContract : class;
    }
}