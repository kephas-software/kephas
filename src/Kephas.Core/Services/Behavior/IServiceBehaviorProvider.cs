// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IServiceBehaviorProvider.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the IServiceBehaviorProvider interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services.Behavior
{
    using System.Collections.Generic;

    using Kephas.Composition;

    /// <summary>
    /// Interface for service behavior provider.
    /// </summary>
    [SharedAppServiceContract]
    public interface IServiceBehaviorProvider
    {
        /// <summary>
        /// Filters the enabled services from the provided services collection.
        /// </summary>
        /// <typeparam name="TService">Type of the service.</typeparam>
        /// <param name="services">The services.</param>
        /// <param name="context">Context for the enabled check (optional).</param>
        /// <returns>
        /// An enumeration of enabled services.
        /// </returns>
        IEnumerable<TService> WhereEnabled<TService>(
            IEnumerable<TService> services,
            IContext context = null)
            where TService : class;

        /// <summary>
        /// Filters the enabled services from the provided services collection.
        /// </summary>
        /// <typeparam name="TService">Type of the service.</typeparam>
        /// <param name="serviceFactories">The service export factories.</param>
        /// <param name="context">Context for the enabled check (optional).</param>
        /// <returns>
        /// An enumeration of enabled services.
        /// </returns>
        IEnumerable<IExportFactory<TService>> WhereEnabled<TService>(
            IEnumerable<IExportFactory<TService>> serviceFactories,
            IContext context = null)
            where TService : class;

        /// <summary>
        /// Enumerates the enabled services from the provided services collection.
        /// </summary>
        /// <typeparam name="TService">Type of the service.</typeparam>
        /// <typeparam name="TMetadata">Type of the service metadata.</typeparam>
        /// <param name="serviceFactories">The service export factories.</param>
        /// <param name="context">Context for the enabled check (optional).</param>
        /// <returns>
        /// An enumerator that allows foreach to be used to process where enabled in this collection.
        /// </returns>
        IEnumerable<IExportFactory<TService, TMetadata>> WhereEnabled<TService, TMetadata>(
            IEnumerable<IExportFactory<TService, TMetadata>> serviceFactories,
            IContext context = null)
            where TService : class;
    }
}