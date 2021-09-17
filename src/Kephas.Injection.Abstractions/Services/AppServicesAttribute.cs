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
        private readonly Type[] serviceTypes;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppServicesAttribute"/> class.
        /// </summary>
        /// <param name="contractDeclarationTypes">The contract declaration types.</param>
        /// <param name="serviceTypes">The service types (contract implementations).</param>
        public AppServicesAttribute(Type[]? contractDeclarationTypes = null, Type[]? serviceTypes = null)
        {
            this.contractDeclarationTypes = contractDeclarationTypes ?? Type.EmptyTypes;
            this.serviceTypes = serviceTypes ?? Type.EmptyTypes;
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
        /// Gets an enumeration of types implementing application service contracts.
        /// </summary>
        /// <returns>
        /// An enumeration of types implementing application service contracts.
        /// </returns>
        public IEnumerable<Type> GetAppServiceTypes()
        {
            return this.serviceTypes;
        }

        private IAppServiceInfo? TryGetAppServiceInfo(Type type)
        {
            return type.GetCustomAttributes(inherit: false).OfType<IAppServiceInfo>().FirstOrDefault();
        }
    }
}