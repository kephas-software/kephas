// --------------------------------------------------------------------------------------------------------------------
// <copyright file="INamedServiceProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the INamedServiceProvider interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services
{
    /// <summary>
    /// Contract for a shared application service providing named services.
    /// </summary>
    [SharedAppServiceContract]
    public interface INamedServiceProvider
    {
        /// <summary>
        /// Gets the service with the provided name.
        /// </summary>
        /// <typeparam name="TService">Type of the service.</typeparam>
        /// <param name="serviceName">Name of the service.</param>
        /// <returns>
        /// The named service.
        /// </returns>
        TService GetNamedService<TService>(string serviceName);
    }
}