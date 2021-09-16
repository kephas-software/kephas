// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppServiceInfoProviderAttribute.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Kephas.Services.Reflection;

    /// <summary>
    /// Assembly attribute collecting the app service contract types.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public sealed class AppServiceInfoProviderAttribute : Attribute, IAppServiceInfoProvider
    {
        private readonly Type[] contractTypes;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppServiceInfoProviderAttribute"/> class.
        /// </summary>
        /// <param name="contractTypes">The contract types.</param>
        public AppServiceInfoProviderAttribute(params Type[] contractTypes)
        {
            this.contractTypes = contractTypes;
        }

        /// <summary>
        /// Gets an enumeration of application service information objects.
        /// </summary>
        /// <param name="candidateTypes">The candidate types which can take part in the composition.</param>
        /// <returns>
        /// An enumeration of application service information objects and their associated contract type.
        /// </returns>
        public IEnumerable<(Type contractType, IAppServiceInfo appServiceInfo)> GetAppServiceInfos(IList<Type>? candidateTypes = null)
        {
            foreach (var contractType in this.contractTypes)
            {
                var appServiceInfo = this.TryGetAppServiceInfo(contractType);
                if (appServiceInfo != null)
                {
                    yield return (contractType, appServiceInfo);
                }
            }
        }

        private IAppServiceInfo? TryGetAppServiceInfo(Type type)
        {
            return type.GetCustomAttributes(inherit: false).OfType<IAppServiceInfo>().FirstOrDefault();
        }
    }
}