﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PartsAppServiceInfoProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Testing.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Castle.Core.Internal;
    using Kephas.Services.AttributedModel;
    using Kephas.Services;

    /// <summary>
    /// Implementation of <see cref="IAppServiceInfoProvider"/> for an enumeration of types.
    /// </summary>
    public class PartsAppServiceInfoProvider : IAppServiceInfoProvider
    {
        private readonly IList<Type> parts;

        /// <summary>
        /// Initializes a new instance of the <see cref="PartsAppServiceInfoProvider"/> class.
        /// </summary>
        /// <param name="parts">The parts.</param>
        public PartsAppServiceInfoProvider(IEnumerable<Type> parts)
        {
            this.parts = parts.ToList();
        }

        /// <summary>
        /// Gets the contract declaration types.
        /// </summary>
        /// <returns>
        /// The contract declaration types.
        /// </returns>
        IEnumerable<ContractDeclarationInfo>? IAppServiceInfoProvider.GetContractDeclarationTypes() => this.parts.Select(p => new ContractDeclarationInfo(p, null));

        /// <summary>
        /// Gets an enumeration of tuples containing the service type and the contract declaration type which it implements.
        /// </summary>
        /// <returns>
        /// An enumeration of tuples containing the service type and the contract declaration type which it implements.
        /// </returns>
        public IEnumerable<ServiceDeclaration> GetAppServices()
        {
            return
                from serviceType in this.parts
                where serviceType.IsClass && !serviceType.IsAbstract && !serviceType.IsNestedPrivate && serviceType.GetCustomAttribute<ExcludeFromServicesAttribute>() == null
                let contractDeclarationType = this.TryGetAppServiceContract(serviceType)
                where contractDeclarationType != null
                select new ServiceDeclaration(serviceType, contractDeclarationType!);
        }

        private Type? TryGetAppServiceContract(Type part)
        {
            if (this.IsAppServiceContract(part))
            {
                return this.GetOriginalAppServiceContract(part);
            }

            var contract = part.GetInterfaces().FirstOrDefault(this.IsAppServiceContract);
            if (contract != null)
            {
                return this.GetOriginalAppServiceContract(contract);
            }

            var baseType = part.BaseType;
            while (baseType != null)
            {
                if (this.IsAppServiceContract(baseType))
                {
                    return this.GetOriginalAppServiceContract(baseType);
                }

                baseType = baseType.BaseType;
            }

            return null;
        }

        private Type GetOriginalAppServiceContract(Type type)
        {
            if (!type.IsGenericType)
            {
                return type;
            }

            var appServiceContractAttr = type.GetAttribute<AppServiceContractAttribute>();
            if (appServiceContractAttr == null)
            {
                return type;
            }

            return appServiceContractAttr.ContractType is { IsGenericType: false }
                ? type.GetGenericTypeDefinition()
                : type;
        }

        private bool IsAppServiceContract(Type type) =>
            ((IAppServiceInfoProvider)this).TryGetAppServiceInfo(type) != null;
    }
}