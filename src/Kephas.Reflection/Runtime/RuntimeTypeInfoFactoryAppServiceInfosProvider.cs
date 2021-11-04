// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRuntimeTypeInfoFactory.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IRuntimeTypeInfoFactory interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Runtime;
using Kephas.Services;

[assembly: AppServices(providerType: typeof(RuntimeTypeInfoFactoryAppServiceInfosProvider))]

namespace Kephas.Runtime
{
    using System.Collections.Generic;

    using Kephas.Runtime.Factories;
    using Kephas.Services;
    using Kephas.Services.Reflection;

    /// <summary>
    /// An <see cref="IAppServiceInfosProvider"/> for the runtime type infos factories.
    /// </summary>
    public class RuntimeTypeInfoFactoryAppServiceInfosProvider : IAppServiceInfosProvider
    {
        /// <summary>
        /// Gets the contract declaration types.
        /// </summary>
        /// <param name="context">Optional. The context in which the service types are requested.</param>
        /// <returns>
        /// An enumeration of application service information objects and their associated contract type.
        /// </returns>
        public IEnumerable<ContractDeclaration> GetAppServiceContracts(dynamic? context = null)
        {
            yield return new ContractDeclaration(
                typeof(IRuntimeTypeInfoFactory),
                new AppServiceInfo(
                    typeof(IRuntimeTypeInfoFactory),
                    AppServiceLifetime.Singleton)
                {
                    AllowMultiple = true,
                });
        }
    }
}
