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
        private static readonly string AbstractTypeInfoName = "Kephas_" + nameof(AbstractTypeInfoName);

        /// <summary>
        /// Gets the abstract type for an implementation type.
        /// </summary>
        /// <param name="type">The type to act on.</param>
        /// <returns>
        /// The abstract type, or the type itself, if the type is not an implementation type.
        /// </returns>
        public static ITypeInfo GetAbstractTypeInfo(this Type type)
        {
            Requires.NotNull(type, nameof(type));

            return type.AsRuntimeTypeInfo().GetAbstractTypeInfo();
        }

        /// <summary>
        /// Gets the abstract type for an implementation type.
        /// </summary>
        /// <param name="type">The type to act on.</param>
        /// <returns>
        /// The abstract type, or the type itself, if the type is not an implementation type.
        /// </returns>
        public static ITypeInfo GetAbstractTypeInfo(this ITypeInfo type)
        {
            Requires.NotNull(type, nameof(type));

            if (!(type[AbstractTypeInfoName] is ITypeInfo abstractTypeInfo))
            {
                var implementationFor = type.GetAttribute<ImplementationForAttribute>();
                abstractTypeInfo = (implementationFor != null ? implementationFor.AbstractType?.AsRuntimeTypeInfo() : type) ?? type;
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
            Requires.NotNull(type, nameof(type));

            return ((IRuntimeTypeInfo)GetAbstractTypeInfo(type.AsRuntimeTypeInfo())).Type;
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
            Requires.NotNull(type, nameof(type));

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
            switch (obj)
            {
                case Type typeObj:
                    return GetAbstractType(typeObj);
                case ITypeInfo typeInfoInterface:
                    return GetAbstractType(typeInfoInterface);
                default:
                    return obj?.GetType().GetAbstractType();
            }
        }

        /// <summary>
        /// Gets the abstract type for which this instance is an implementation.
        /// </summary>
        /// <param name="obj">The object to act on.</param>
        /// <returns>
        /// The abstract type.
        /// </returns>
        public static ITypeInfo GetAbstractTypeInfo(this object obj)
        {
            switch (obj)
            {
                case Type typeObj:
                    return GetAbstractTypeInfo(typeObj);
                case ITypeInfo typeInfoInterface:
                    return GetAbstractTypeInfo(typeInfoInterface);
                default:
                    return obj?.GetType().GetAbstractTypeInfo();
            }
        }
    }
}