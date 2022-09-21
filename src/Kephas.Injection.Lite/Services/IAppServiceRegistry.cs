// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAppServiceRegistry.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services
{
    using System;

    using Kephas.Services.Reflection;

    /// <summary>
    /// Registry for <see cref="IAppServiceInfo"/> instances.
    /// </summary>
    public interface IAppServiceRegistry : IAppServiceInfosProvider, IServiceProvider, IDisposable
    {
        /// <summary>
        /// Attempts to get the <see cref="IAppServiceSource"/> for the given service contract.
        /// </summary>
        /// <param name="contractType">Type of the service contract.</param>
        /// <param name="appServiceSource">The <see cref="IAppServiceSource"/> instance.</param>
        /// <returns>
        /// True if a <see cref="IAppServiceSource"/> is found, otherwise false.
        /// </returns>
        bool TryGetSource(Type contractType, out IAppServiceSource? appServiceSource);

        /// <summary>
        /// Gets a value indicating whether the service with the provided contract is registered.
        /// </summary>
        /// <param name="contractType">Type of the service contract.</param>
        /// <returns>
        /// <c>true</c> if the service is registered, <c>false</c> if not.
        /// </returns>
        bool IsRegistered(Type contractType) => this.TryGetSource(contractType, out _);

        /// <summary>
        /// Registers the source described by <paramref name="appServiceSource"/>.
        /// </summary>
        /// <param name="appServiceSource">The service source.</param>
        /// <returns>
        /// This service registry.
        /// </returns>
        IAppServiceRegistry RegisterSource(IAppServiceSource appServiceSource);

        /// <summary>
        /// Gets the service object of the specified type.
        /// </summary>
        /// <returns>
        /// A service object of type <paramref name="contractType"/>.-or- null if there is no service object of type <paramref name="contractType"/>.
        /// </returns>
        /// <param name="contractType">The contract type of service to get. </param>
        object? IServiceProvider.GetService(Type contractType)
        {
            this.TryGetSource(contractType, out var serviceSource);
            return serviceSource?.GetService(this, contractType);
        }
    }
}