// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HostingServiceCollectionExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Kephas.Services;

    /// <summary>
    /// Microsoft.Extensions.DependencyInjection related ambient services extensions.
    /// </summary>
    public static class HostingServiceCollectionExtensions
    {
        /// <summary>
        /// Gets the services configurators.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <returns>An enumeration of <see cref="IServicesConfigurator"/>.</returns>
        public static IEnumerable<IServicesConfigurator> GetServicesConfigurators(this IAmbientServices ambientServices)
        {
            var configuratorTypes = ambientServices
                .Where(t => t.ContractDeclarationType == typeof(IServicesConfigurator) || t.ContractType == typeof(IServicesConfigurator))
                .Select(t => t.InstanceType)
                .Where(t => t is not null)
                .ToList();
            var orderedConfiguratorTypes = configuratorTypes
                .Select(t => new Lazy<IServicesConfigurator, AppServiceMetadata>(
                    () => (IServicesConfigurator)Activator.CreateInstance(t),
                    new AppServiceMetadata(ServiceHelper.GetServiceMetadata(t, typeof(IServicesConfigurator)))))
                .Order();
            return orderedConfiguratorTypes.Select(f => f.Value);
        }
    }
}
