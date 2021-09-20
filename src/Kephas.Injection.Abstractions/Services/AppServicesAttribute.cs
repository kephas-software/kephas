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

    using Kephas.Services.Reflection;

    /// <summary>
    /// Assembly attribute decorating an assembly and collecting the application services, both contract types and implementation types.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public sealed class AppServicesAttribute : Attribute, IAppServiceInfoProvider, IAppServiceTypesProvider
    {
        private readonly Type[] contractDeclarationTypes;
        private readonly (Type serviceType, Type contractDeclarationType)[] serviceTypes;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppServicesAttribute"/> class.
        /// </summary>
        /// <param name="contractDeclarationTypes">The contract declaration types.</param>
        /// <param name="serviceTypes">The service types (contract implementations) with their respective contract.</param>
        public AppServicesAttribute(Type[]? contractDeclarationTypes = null, (Type serviceType, Type contractDeclarationType)[]? serviceTypes = null)
        {
            this.contractDeclarationTypes = contractDeclarationTypes ?? Type.EmptyTypes;
            this.serviceTypes = serviceTypes ?? Array.Empty<(Type serviceType, Type contractDeclarationType)>();
        }

        /// <summary>
        /// Gets an enumeration of application service information objects.
        /// </summary>
        /// <param name="candidateTypes">The candidate types which can take part in the composition.</param>
        /// <returns>
        /// An enumeration of application service information objects and their associated contract type.
        /// </returns>
        public IEnumerable<(Type contractDeclarationType, IAppServiceInfo appServiceInfo)> GetAppServiceInfos(IList<Type>? candidateTypes = null)
        {
            foreach (var contractType in this.contractDeclarationTypes)
            {
                var appServiceInfo = this.TryGetAppServiceInfo(contractType);
                if (appServiceInfo != null)
                {
                    yield return (contractType, appServiceInfo);
                }
            }
        }

        /// <summary>
        /// Gets an enumeration of tuples containing the service type and the contract declaration type which it implements.
        /// </summary>
        /// <returns>
        /// An enumeration of tuples containing the service type and the contract declaration type which it implements.
        /// </returns>
        public IEnumerable<(Type serviceType, Type contractDeclarationType)> GetAppServiceTypes()
        {
            return this.serviceTypes;
        }

        private IAppServiceInfo? TryGetAppServiceInfo(Type type)
        {
            return type.GetCustomAttributes(inherit: false).OfType<IAppServiceInfo>().FirstOrDefault();
        }
    }
}