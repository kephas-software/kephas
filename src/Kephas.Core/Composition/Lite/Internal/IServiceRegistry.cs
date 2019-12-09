// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IServiceRegistry.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IServiceRegistry interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Lite.Internal
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Interface for service registry.
    /// </summary>
    internal interface IServiceRegistry : IEnumerable<IServiceInfo>, IDisposable
    {
        /// <summary>
        /// Gets the registration sources.
        /// </summary>
        /// <value>
        /// The  registration sources.
        /// </value>
        IEnumerable<IServiceSource> Sources { get; }

        /// <summary>
        /// Indexer to get items within this collection using array index syntax.
        /// </summary>
        /// <param name="contractType">Type of the contract.</param>
        /// <returns>
        /// The indexed item.
        /// </returns>
        IServiceInfo this[Type contractType] { get; }

        /// <summary>
        /// Attempts to get value from the given data.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="serviceInfo">[out] Information describing the service.</param>
        /// <returns>
        /// True if it succeeds, false if it fails.
        /// </returns>
        bool TryGet(Type serviceType, out IServiceInfo serviceInfo);

        /// <summary>
        /// Gets or registers a service.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="serviceInfoGetter">The service info getter.</param>
        /// <returns>
        /// The existing or the registered service info.
        /// </returns>
        IServiceInfo GetOrRegister(Type serviceType, Func<Type, IServiceInfo> serviceInfoGetter);

        /// <summary>
        /// Gets a value indicating whether the service with the provided contract is registered.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <returns>
        /// <c>true</c> if the service is registered, <c>false</c> if not.
        /// </returns>
        bool IsRegistered(Type serviceType);

        /// <summary>
        /// Registers the service described by serviceInfo.
        /// </summary>
        /// <param name="serviceInfo">Information describing the service.</param>
        /// <returns>
        /// This service registry.
        /// </returns>
        ServiceRegistry RegisterService(IServiceInfo serviceInfo);

        /// <summary>
        /// Registers the source described by serviceSource.
        /// </summary>
        /// <param name="serviceSource">The service source.</param>
        /// <returns>
        /// This service registry.
        /// </returns>
        ServiceRegistry RegisterSource(IServiceSource serviceSource);
    }
}