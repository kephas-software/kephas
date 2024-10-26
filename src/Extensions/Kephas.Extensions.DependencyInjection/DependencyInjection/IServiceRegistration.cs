// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IServiceRegistration.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Kephas.Extensions.DependencyInjection;

/// <summary>
/// Contract for registering services in the service collection.
/// </summary>
/// <remarks>
/// The <see cref="IServiceRegistration"/> implementations must have a parameterless constructor. 
/// </remarks>
[SingletonService]
public interface IServiceRegistration
{
    /// <summary>
    /// Registers services in the provided collection, using the given configuration.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The application configuration.</param>
    void RegisterServices(IServiceCollection services, IConfiguration configuration);
}