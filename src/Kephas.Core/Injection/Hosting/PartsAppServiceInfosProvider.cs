// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PartsAppServiceInfosProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Injection.Hosting
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Kephas.Injection.AttributedModel;
    using Kephas.Services;

    /// <summary>
    /// Implementation of <see cref="IAppServiceInfosProvider"/> for an enumeration of types.
    /// </summary>
    internal class PartsAppServiceInfosProvider : IAppServiceInfosProvider, IAppServiceTypesProvider
    {
        private readonly IList<Type> parts;

        /// <summary>
        /// Initializes a new instance of the <see cref="PartsAppServiceInfosProvider"/> class.
        /// </summary>
        /// <param name="parts">The parts.</param>
        public PartsAppServiceInfosProvider(IEnumerable<Type> parts)
        {
            this.parts = parts.ToList();
        }

        /// <summary>
        /// Gets the contract declaration types.
        /// </summary>
        /// <param name="context">Optional. The context in which the service types are requested.</param>
        /// <returns>
        /// The contract declaration types.
        /// </returns>
        public IEnumerable<Type>? GetContractDeclarationTypes(dynamic? context = null) => this.parts;

        /// <summary>
        /// Gets an enumeration of tuples containing the service type and the contract declaration type which it implements.
        /// </summary>
        /// <param name="context">Optional. The context in which the service types are requested.</param>
        /// <returns>
        /// An enumeration of tuples containing the service type and the contract declaration type which it implements.
        /// </returns>
        public IEnumerable<(Type serviceType, Type contractDeclarationType)> GetAppServiceTypes(dynamic? context = null)
        {
            return
                from part in this.parts
                where part.IsClass && !part.IsAbstract && part.GetCustomAttribute<ExcludeFromInjectionAttribute>() == null
                let contractType = this.TryGetAppServiceContract(part)
                where contractType != null
                select (part, contractType!);
        }

        private Type? TryGetAppServiceContract(Type part)
        {
            if (this.IsAppServiceContract(part))
            {
                return part;
            }

            var contract = part.GetInterfaces().FirstOrDefault(this.IsAppServiceContract);
            if (contract != null)
            {
                return contract;
            }

            var baseType = part.BaseType;
            while (baseType != null)
            {
                if (this.IsAppServiceContract(baseType))
                {
                    return baseType;
                }

                baseType = baseType.BaseType;
            }

            return null;
        }

        private bool IsAppServiceContract(Type type) =>
            ((IAppServiceInfosProvider)this).TryGetAppServiceInfo(type) != null;
    }
}