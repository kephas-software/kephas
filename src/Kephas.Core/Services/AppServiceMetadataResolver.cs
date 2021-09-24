﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppServiceMetadataResolver.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the application service metadata resolver class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services
{
    using System;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Default implementation of <see cref="IAppServiceMetadataResolver"/>.
    /// </summary>
    internal class AppServiceMetadataResolver : IAppServiceMetadataResolver
    {
        /// <summary>
        /// The 'T' prefix in generic type arguments.
        /// </summary>
        private const string TypePrefix = "T";

        /// <summary>
        /// The 'Type' suffix in generic type arguments.
        /// </summary>
        private const string TypeSuffix = "Type";

        /// <summary>
        /// Gets the metadata name from generic type parameter.
        /// </summary>
        /// <param name="genericTypeParameter">The generic type parameter.</param>
        /// <returns>The metadata name.</returns>
        public string GetMetadataNameFromGenericTypeParameter(Type genericTypeParameter)
        {
            var name = genericTypeParameter.Name;
            if (name.StartsWith(TypePrefix) && name.Length > 1 && name[1] == char.ToUpperInvariant(name[1]))
            {
                name = name[1..];
            }

            if (!name.EndsWith(TypeSuffix))
            {
                name += TypeSuffix;
            }

            return name;
        }

        /// <summary>
        /// Gets the metadata value from generic parameter.
        /// </summary>
        /// <param name="implementationType">The service implementation type.</param>
        /// <param name="position">The position.</param>
        /// <param name="serviceType">Type of the service contract.</param>
        /// <returns>The metadata value.</returns>
        public object GetMetadataValueFromGenericParameter(Type implementationType, int position, Type serviceType)
        {
            var typeInfo = implementationType.GetTypeInfo();
            var closedGeneric = typeInfo.ImplementedInterfaces
                .Select(i => i.GetTypeInfo())
                .FirstOrDefault(
                    i =>
                        i.IsGenericType && !i.IsGenericTypeDefinition
                                        && i.GetGenericTypeDefinition() == serviceType);

            if (closedGeneric == null && implementationType.IsConstructedGenericType && implementationType.GetGenericTypeDefinition() == serviceType)
            {
                closedGeneric = typeInfo;
            }

            var genericArg = closedGeneric?.GenericTypeArguments[position];
            if (genericArg?.IsGenericParameter ?? false)
            {
                genericArg = genericArg.GetTypeInfo().BaseType;
            }

            return genericArg;
        }
    }
}