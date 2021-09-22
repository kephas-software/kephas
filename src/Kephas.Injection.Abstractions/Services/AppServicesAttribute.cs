// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppServicesAttribute.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services
{
    using System;
    using System.Collections.Generic;

    using Kephas.Services.Reflection;

    /// <summary>
    /// Assembly attribute decorating an assembly and collecting the application services, both contract types and implementation types.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public sealed class AppServicesAttribute : Attribute, IAppServiceInfoProvider, IAppServiceTypesProvider, IHasProcessingPriority
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppServicesAttribute"/> class.
        /// </summary>
        /// <param name="providerType">
        /// The type providing the application services.
        /// Should implement at least one of <see cref="IAppServiceInfoProvider"/> and <see cref="IAppServiceTypesProvider"/> interfaces.
        /// </param>
        /// <param name="processingPriority">Adds the processing priority in which the attributes will be processed.</param>
        public AppServicesAttribute(Type providerType, Priority processingPriority = Priority.Normal)
        {
            this.ProviderType = providerType ?? throw new ArgumentNullException(nameof(providerType));
            this.ProcessingPriority = processingPriority;
        }

        /// <summary>
        /// Gets the provider type.
        /// </summary>
        public Type ProviderType { get; }

        /// <summary>
        /// Gets the processing priority.
        /// </summary>
        public Priority ProcessingPriority { get; }

        /// <summary>
        /// Gets an enumeration of application service information objects and their contract declaration type.
        /// The contract declaration type is the type declaring the contract: if the <see cref="AppServiceContractAttribute.ContractType"/>
        /// is not provided, the contract declaration type is also the contract type.
        /// </summary>
        /// <param name="context">Optional. The context in which the service types are requested.</param>
        /// <returns>
        /// An enumeration of application service information objects and their contract declaration type.
        /// </returns>
        public IEnumerable<(Type contractDeclarationType, IAppServiceInfo appServiceInfo)> GetAppServiceInfos(dynamic? context = null)
        {
            var provider = Activator.CreateInstance(this.ProviderType) as IAppServiceInfoProvider;
            return provider?.GetAppServiceInfos(context) ?? Array.Empty<(Type contractDeclarationType, IAppServiceInfo appServiceInfo)>();
        }

        /// <summary>
        /// Gets an enumeration of tuples containing the service type and the contract declaration type which it implements.
        /// </summary>
        /// <param name="context">Optional. The context in which the service types are requested.</param>
        /// <returns>
        /// An enumeration of tuples containing the service type and the contract declaration type which it implements.
        /// </returns>
        public IEnumerable<(Type serviceType, Type contractDeclarationType)> GetAppServiceTypes(dynamic? context = null)
        {
            var provider = Activator.CreateInstance(this.ProviderType) as IAppServiceTypesProvider;
            return provider?.GetAppServiceTypes(context) ?? Array.Empty<(Type serviceType, Type contractDeclarationType)>();
        }
    }
}