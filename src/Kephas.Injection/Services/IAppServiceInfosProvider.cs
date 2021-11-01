// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAppServiceInfosProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IAppServiceInfosProvider interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    using Kephas.Services.Reflection;

    /// <summary>
    /// Service contract declaration record.
    /// </summary>
    /// <param name="ContractDeclarationType">The contract declaration type.</param>
    /// <param name="AppServiceInfo">The <see cref="IAppServiceInfo"/> attached to the contract declaration type.</param>
    public record ContractDeclaration(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type ContractDeclarationType,
        IAppServiceInfo AppServiceInfo);

    /// <summary>
    /// Service implementation declaration record.
    /// </summary>
    /// <param name="ServiceType">The service type.</param>
    /// <param name="ContractDeclarationType">The contract declaration type.</param>
    public record ServiceDeclaration(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type ServiceType,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type ContractDeclarationType);

    /// <summary>
    /// Interface providing the <see cref="GetAppServiceInfos"/> method,
    /// which collects <see cref="IAppServiceInfo"/> data together with the contract declaration type.
    /// </summary>
#if NET6_0_OR_GREATER
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
#endif
    public interface IAppServiceInfosProvider
    {
        /// <summary>
        /// Gets an enumeration of application service information objects and their contract declaration type.
        /// The contract declaration type is the type declaring the contract: if the <see cref="AppServiceContractAttribute.ContractType"/>
        /// is not provided, the contract declaration type is also the contract type.
        /// </summary>
        /// <param name="context">Optional. The context in which the service types are requested.</param>
        /// <returns>
        /// An enumeration of application service information objects and their contract declaration type.
        /// </returns>
        public IEnumerable<ContractDeclaration> GetAppServiceInfos(dynamic? context = null)
            => GetAppServiceInfosCore(this, context);

        /// <summary>
        /// Gets an enumeration of tuples containing the service type and the contract declaration type which it implements.
        /// </summary>
        /// <param name="context">Optional. The context in which the service types are requested.</param>
        /// <returns>
        /// An enumeration of tuples containing the service type and the contract declaration type which it implements.
        /// </returns>
        public IEnumerable<ServiceDeclaration> GetAppServiceTypes(dynamic? context = null)
            => Enumerable.Empty<ServiceDeclaration>();

        /// <summary>
        /// Tries the get the application service information from the custom attributes.
        /// </summary>
        /// <param name="type">The contract declaration type.</param>
        /// <returns>The <see cref="IAppServiceInfo"/> instance or <c>null</c>.</returns>
        protected internal IAppServiceInfo? TryGetAppServiceInfo(Type type)
        {
            return type.GetCustomAttributes(inherit: false).OfType<IAppServiceInfo>().FirstOrDefault();
        }

        /// <summary>
        /// Gets an enumeration of application service information objects and their contract declaration type.
        /// The contract declaration type is the type declaring the contract: if the <see cref="AppServiceContractAttribute.ContractType"/>
        /// is not provided, the contract declaration type is also the contract type.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="context">Optional. The context in which the service types are requested.</param>
        /// <returns>
        /// An enumeration of application service information objects and their contract declaration type.
        /// </returns>
        protected internal static IEnumerable<ContractDeclaration> GetAppServiceInfosCore(IAppServiceInfosProvider provider, dynamic? context = null)
        {
            var contractDeclarationTypes = provider.GetContractDeclarationTypes(context);
            if (contractDeclarationTypes == null)
            {
                yield break;
            }

            foreach (var contractType in contractDeclarationTypes)
            {
                var appServiceInfo = provider.TryGetAppServiceInfo(contractType);
                if (appServiceInfo != null)
                {
                    yield return new ContractDeclaration(contractType, appServiceInfo);
                }
            }
        }

        /// <summary>
        /// Gets the contract declaration types.
        /// </summary>
        /// <param name="context">Optional. The context in which the service types are requested.</param>
        /// <returns>
        /// The contract declaration types.
        /// </returns>
        protected internal IEnumerable<Type>? GetContractDeclarationTypes(dynamic? context = null) => null;
    }
}