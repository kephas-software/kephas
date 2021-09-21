// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AmbientServicesAppServiceInfoProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Services;

[assembly: AppServices(
    processingPriority: Priority.Highest,
    serviceProviderTypes: new[] { typeof(AmbientServicesAppServiceInfoProvider) })]

namespace Kephas.Services
{
    using System;
    using System.Collections.Generic;

    using Kephas.Injection.Hosting;
    using Kephas.Services.Reflection;

    /// <summary>
    /// Provider of <see cref="IAppServiceInfo"/> from the <see cref="IAmbientServices"/>.
    /// </summary>
    public class AmbientServicesAppServiceInfoProvider : IAppServiceInfoProvider
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
        public IEnumerable<(Type contractDeclarationType, IAppServiceInfo appServiceInfo)> GetAppServiceInfos(dynamic? context = null)
        {
            if (((IInjectionRegistrationContext?)context)?.AmbientServices is not IAppServiceInfoProvider ambientServicesProvider)
            {
                return Array.Empty<(Type contractDeclarationType, IAppServiceInfo appServiceInfo)>();
            }

            return ambientServicesProvider.GetAppServiceInfos(context);
        }
    }
}