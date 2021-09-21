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
    using System.Linq;

    /// <summary>
    /// Assembly attribute decorating an assembly and collecting the application services, both contract types and implementation types.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public sealed class AppServicesAttribute : Attribute, IAppServiceInfoProvider, IAppServiceTypesProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppServicesAttribute"/> class.
        /// </summary>
        /// <param name="contractDeclarationTypes">Optional. The contract declaration types.</param>
        /// <param name="serviceProviderTypes">Optional. The app service provider types implementing <see cref="IAppServiceTypesProvider"/>.</param>
        public AppServicesAttribute(Type[]? contractDeclarationTypes = null, Type[]? serviceProviderTypes = null)
        {
            this.ContractDeclarationTypes = contractDeclarationTypes ?? Type.EmptyTypes;
            this.ServiceProviderTypes = serviceProviderTypes ?? Type.EmptyTypes;
        }

        /// <summary>
        /// Gets the contract declaration types.
        /// </summary>
        public Type[] ContractDeclarationTypes { get; }

        /// <summary>
        /// Gets the service provider types.
        /// </summary>
        public Type[] ServiceProviderTypes { get; }

        /// <summary>
        /// Gets the contract declaration types.
        /// </summary>
        /// <param name="context">Optional. The context in which the service types are requested.</param>
        /// <returns>
        ///     The contract declaration types.
        /// </returns>
        IEnumerable<Type>? IAppServiceInfoProvider.GetContractDeclarationTypes(dynamic? context) => this.ContractDeclarationTypes;

        /// <summary>
        /// Gets an enumeration of tuples containing the service type and the contract declaration type which it implements.
        /// </summary>
        /// <param name="context">Optional. The context in which the service types are requested.</param>
        /// <returns>
        /// An enumeration of tuples containing the service type and the contract declaration type which it implements.
        /// </returns>
        public IEnumerable<(Type serviceType, Type contractDeclarationType)> GetAppServiceTypes(dynamic? context = null)
        {
            return this.ServiceProviderTypes
                .Select(providerType => providerType == null ? null : Activator.CreateInstance(providerType) as IAppServiceTypesProvider)
                .Where(provider => provider != null)
                .SelectMany<IAppServiceTypesProvider?, (Type serviceType, Type contractDeclarationType)>(provider => provider!.GetAppServiceTypes(context));
        }

    }
}