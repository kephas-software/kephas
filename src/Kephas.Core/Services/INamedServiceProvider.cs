// --------------------------------------------------------------------------------------------------------------------
// <copyright file="INamedServiceProvider.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
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