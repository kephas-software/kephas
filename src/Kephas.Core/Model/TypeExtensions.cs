// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the type extensions class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model
{
    using System;
    using System.Reflection;

    using Kephas.Activation;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Reflection;
    using Kephas.Runtime;

    /// <summary>
    /// Extension methods for <see cref="Type"/>, <see cref="TypeInfo"/>, and <see cref="ITypeInfo"/>.
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// The name of the extended model type info property.
        /// </summary>
        private static readonly string AbstractTypeInfoName = "__" + nameof(AbstractTypeInfoName);

        /// <summary>
        /// Gets the abstract type for an implementation type.
        /// </summary>
        /// <param name="type">The type to act on.</param>
        /// <returns>
        /// The abstract type, or the type itself, if the type is not an implementation type.
        /// </returns>
        public static ITypeInfo GetAbstractTypeInfo(this ITypeInfo type)
        {
            type = type ?? throw new ArgumentNullException(nameof(type));

            if (!(type[AbstractTypeInfoName] is ITypeInfo abstractTypeInfo))
            {
                var implementationFor = type.GetAttribute<ImplementationForAttribute>();
                abstractTypeInfo = implementationFor?.AbstractType != null
                    ? type.TypeRegistry.GetTypeInfo(implementationFor.AbstractType)!
                    : type;
                type[AbstractTypeInfoName] = abstractTypeInfo;
            }

            return abstractTypeInfo;
        }

        /// <summary>
        /// Gets the abstract type for an implementation type.
        /// </summary>
        /// <param name="type">The type to act on.</param>
        /// <returns>
        /// The abstract type, or the type itself, if the type is not an implementation type.
        /// </returns>
        public static Type GetAbstractType(this Type type)
        {
            type = type ?? throw new ArgumentNullException(nameof(type));

            var implementationFor = type.GetCustomAttribute<ImplementationForAttribute>();
            return (implementationFor != null ? implementationFor.AbstractType : type) ?? type;
        }

        /// <summary>
        /// Gets the abstract type for an implementation type.
        /// </summary>
        /// <param name="type">The type to act on.</param>
        /// <returns>
        /// The abstract type, or the type itself, if the type is not an implementation type.
        /// </returns>
        public static Type GetAbstractType(this ITypeInfo type)
        {
            type = type ?? throw new ArgumentNullException(nameof(type));

            return ((IRuntimeTypeInfo)GetAbstractTypeInfo(type)).Type;
        }

        /// <summary>
        /// Gets the abstract type for which this instance is an implementation.
        /// </summary>
        /// <param name="obj">The object to act on.</param>
        /// <returns>
        /// The abstract type.
        /// </returns>
        public static Type GetAbstractType(this object obj)
        {
            obj = obj ?? throw new ArgumentNullException(nameof(obj));

            return obj switch
            {
                Type typeObj => typeObj.GetAbstractType(),
                ITypeInfo typeInfoInterface => GetAbstractType(typeInfoInterface),
                _ => obj.GetType().GetAbstractType()
            };
        }
    }
}