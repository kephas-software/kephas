// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAppServiceInfoProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IAppServiceInfoProvider interface.
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
    public interface IAppServiceInfoProvider
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
            => GetAppServiceInfos(this, context);

        /// <summary>
        /// Tries the get the application service information from the custom attributes.
        /// </summary>
        /// <param name="type">The contract declaration type.</param>
        /// <returns>The <see cref="IAppServiceInfo"/> instance or <c>null</c>.</returns>
        protected IAppServiceInfo? TryGetAppServiceInfo(Type type)
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
        protected internal static IEnumerable<(Type contractDeclarationType, IAppServiceInfo appServiceInfo)> GetAppServiceInfos(IAppServiceInfoProvider provider, dynamic? context = null)
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