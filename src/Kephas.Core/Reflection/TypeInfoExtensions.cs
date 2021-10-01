// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeInfoExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Extension methods for type information.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Reflection
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Runtime;

    /// <summary>
    /// Extension methods for type information.
    /// </summary>
    public static class TypeInfoExtensions
    {
        /// <summary>
        /// Information describing the object type.
        /// </summary>
        internal static readonly TypeInfo ObjectTypeInfo = IntrospectionExtensions.GetTypeInfo(typeof(object));

        /// <summary>
        /// Gets the type wrapped by the <see cref="Nullable{T}"/> or,
        /// if the type is not a nullable type, the type itself.
        /// </summary>
        /// <param name="typeInfo">The type to be checked.</param>
        /// <returns>A <see cref="Type"/> instance.</returns>
        public static TypeInfo GetNonNullableType(this TypeInfo typeInfo)
        {
            Requires.NotNull(typeInfo, nameof(typeInfo));

            return IsNullableType(typeInfo) ? IntrospectionExtensions.GetTypeInfo(typeInfo.GenericTypeArguments[0]) : typeInfo;
        }

        /// <summary>
        /// Indicates whether the type is an instance of the generic <see cref="Nullable{T}"/> type.
        /// </summary>
        /// <param name="typeInfo">The type to check.</param>
        /// <returns>
        ///   <c>true</c> if the type is nullable; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNullableType(this TypeInfo typeInfo)
        {
            Requires.NotNull(typeInfo, nameof(typeInfo));

            return typeInfo.IsGenericType && typeInfo.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        /// <summary>
        /// Indicates whether the type is an instance of the generic <see cref="Nullable{T}"/> type.
        /// </summary>
        /// <param name="typeInfo">The type to check.</param>
        /// <returns>
        ///   <c>true</c> if the type is nullable; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNullableType(this ITypeInfo typeInfo)
        {
            Requires.NotNull(typeInfo, nameof(typeInfo));

            if (typeInfo is IRuntimeTypeInfo runtimeTypeInfo)
            {
                return IsNullableType(runtimeTypeInfo.TypeInfo);
            }

            return false;
        }

        /// <summary>
        /// A TypeInfo extension method that gets the base constructed generic of a provided type.
        /// The base can be either an interface or a class.
        /// </summary>
        /// <param name="typeInfo">The type to act on.</param>
        /// <param name="openGenericTypeInfo">The open generic type of which constructed generic type is requested.</param>
        /// <returns>
        /// The base constructed generic.
        /// </returns>
        /// <example>
        /// <code>
        /// var type = typeof(string).GetTypeInfo().GetBaseConstructedGenericOf(typeof(IEnumerable&lt;&gt;).GetTypeInfo());
        /// Assert.AreSame(type, typeof(IEnumerable&lt;char&gt;).GetTypeInfo());
        /// </code>
        /// </example>
        public static TypeInfo? GetBaseConstructedGenericOf(this TypeInfo typeInfo, TypeInfo openGenericTypeInfo)
        {
            Requires.NotNull(typeInfo, nameof(typeInfo));
            Requires.NotNull(openGenericTypeInfo, nameof(openGenericTypeInfo));

            var openGenericType = openGenericTypeInfo.AsType();
            var type = typeInfo.AsType();
            if (openGenericTypeInfo.IsClass)
            {
                while (!typeInfo.Equals(ObjectTypeInfo))
                {
                    if (type.IsConstructedGenericType && typeInfo.GetGenericTypeDefinition() == openGenericType)
                    {
                        return typeInfo;
                    }

                    type = typeInfo.BaseType;
                    typeInfo = IntrospectionExtensions.GetTypeInfo(type);
                }
            }
            else if (openGenericTypeInfo.IsInterface)
            {
                var implementedInterfaces = typeInfo.ImplementedInterfaces;
                var constructedInterface = implementedInterfaces.FirstOrDefault(
                    t => t.IsConstructedGenericType && t.GetGenericTypeDefinition() == openGenericType);
                return IntrospectionExtensions.GetTypeInfo(constructedInterface);
            }

            return null;
        }

        /// <summary>
        /// Gets the qualified full name of a <see cref="TypeInfo"/>. Optionally, strips away the version information.
        /// </summary>
        /// <param name="typeInfo">The type information instance.</param>
        /// <param name="stripVersionInfo"><c>true</c> to strip away the version information (optional).</param>
        /// <returns>
        /// The qualified full name.
        /// </returns>
        public static string GetQualifiedFullName(this TypeInfo typeInfo, bool stripVersionInfo = true)
        {
            Requires.NotNull(typeInfo, nameof(typeInfo));

            return TypeExtensions.GetQualifiedFullName(typeInfo.AsType(), stripVersionInfo);
        }
    }
}