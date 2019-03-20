// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AttributedAppServiceInfoProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the attributed application service information provider class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Conventions
{
    using System.Collections.Generic;
    using System.Reflection;

    using Kephas.Composition.Hosting;
    using Kephas.Services;
    using Kephas.Services.Reflection;

    /// <summary>
    /// An attributed application service information provider.
    /// </summary>
    public class AttributedAppServiceInfoProvider : IAppServiceInfoProvider
    {
        /// <summary>
        /// Gets an enumeration of application service information objects.
        /// </summary>
        /// <param name="candidateTypes">The candidate types which can take part in the composition.</param>
        /// <param name="registrationContext">Context for the registration.</param>
        /// <returns>
        /// An enumeration of application service information objects and their associated contract type.
        /// </returns>
        public IEnumerable<(TypeInfo contractType, IAppServiceInfo appServiceInfo)> GetAppServiceInfos(IEnumerable<TypeInfo> candidateTypes, ICompositionRegistrationContext registrationContext)
        {
            foreach (var candidateType in candidateTypes)
            {
                var appServiceInfo = this.TryGetAppServiceInfo(candidateType);
                if (appServiceInfo != null)
                {
                    yield return (candidateType, appServiceInfo);
                }
            }
        }

        /// <summary>
        /// Tries to get the <see cref="IAppServiceInfo"/> for the provided type.
        /// </summary>
        /// <param name="typeInfo">Information describing the type.</param>
        /// <returns>
        /// An <see cref="IAppServiceInfo"/> or <c>null</c>, if the provided type is not a service contract.
        /// </returns>
        protected virtual IAppServiceInfo TryGetAppServiceInfo(TypeInfo typeInfo)
        {
            return typeInfo.GetCustomAttribute<AppServiceContractAttribute>();
        }
    }
}