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
    using System.Linq;

    using Kephas.Services.Reflection;

    /// <summary>
    /// Interface providing the <see cref="GetAppServiceInfos"/> method,
    /// which collects <see cref="IAppServiceInfo"/> data together with the contract declaration type.
    /// </summary>
    public interface IAppServiceInfosProvider
    {
        /// <summary>
        /// Gets the contract declaration types.
        /// </summary>
        /// <param name="context">Optional. The context in which the service types are requested.</param>
        /// <returns>
        /// The contract declaration types.
        /// </returns>
        public IEnumerable<Type>? GetContractDeclarationTypes(dynamic? context = null) => null;

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
            => GetAppServiceInfosCore(this, context);

        /// <summary>
        /// Gets an enumeration of tuples containing the service type and the contract declaration type which it implements.
        /// </summary>
        /// <param name="context">Optional. The context in which the service types are requested.</param>
        /// <returns>
        /// An enumeration of tuples containing the service type and the contract declaration type which it implements.
        /// </returns>
        public IEnumerable<(Type serviceType, Type contractDeclarationType)> GetAppServiceTypes(dynamic? context = null)
            => Enumerable.Empty<(Type serviceType, Type contractDeclarationType)>();

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
        protected internal static IEnumerable<(Type contractDeclarationType, IAppServiceInfo appServiceInfo)> GetAppServiceInfosCore(IAppServiceInfosProvider provider, dynamic? context = null)
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
                    yield return (contractType, appServiceInfo);
                }
            }
        }
    }
}