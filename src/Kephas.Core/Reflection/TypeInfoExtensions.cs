// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeInfoExtensions.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Extension methods for type information.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Reflection
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Reflection;

    using Kephas.Runtime;

    /// <summary>
    /// Extension methods for type information.
    /// </summary>
    public static class TypeInfoExtensions
    {
        /// <summary>
        /// Gets the <see cref="IRuntimeTypeInfo"/> for the provided <see cref="TypeInfo"/> instance.
        /// </summary>
        /// <param name="typeInfo">The type information instance.</param>
        /// <returns>
        /// The provided <see cref="TypeInfo"/>'s associated <see cref="IRuntimeTypeInfo"/>.
        /// </returns>
        public static IRuntimeTypeInfo AsRuntimeTypeInfo(this TypeInfo typeInfo)
        {
            Contract.Requires(typeInfo != null);

            return RuntimeTypeInfo.GetRuntimeType(typeInfo);
        }

        /// <summary>
        /// Gets the type wrapped by the <see cref="Nullable{T}"/> or,
        /// if the type is not a nullable type, the type itself.
        /// </summary>
        /// <param name="typeInfo">The type to be checked.</param>
        /// <returns>A <see cref="Type"/> instance.</returns>
        public static TypeInfo GetNonNullableType(this TypeInfo typeInfo)
        {
            Contract.Requires(typeInfo != null);

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
            Contract.Requires(typeInfo != null);

            return typeInfo.IsGenericType && typeInfo.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        /// <summary>
        /// Indicates whether the <see cref="ITypeInfo"/> is a generic type.
        /// </summary>
        /// <param name="typeInfo">The type information instance.</param>
        /// <returns>
        /// <c>true</c> if the type is generic, either closed or open; <c>false</c> if not.
        /// </returns>
        public static bool IsGenericType(this ITypeInfo typeInfo)
        {
            Contract.Requires(typeInfo != null);

            return typeInfo.GenericTypeArguments.Count > 0 || typeInfo.GenericTypeParameters.Count > 0;
        }

        /// <summary>
        /// Indicates whether the <see cref="ITypeInfo"/> is a generic type definition (aka open generic).
        /// </summary>
        /// <param name="typeInfo">The type information instance.</param>
        /// <returns>
        /// <c>true</c> if the type is a generic type definition; <c>false</c> if not.
        /// </returns>
        public static bool IsGenericTypeDefinition(this ITypeInfo typeInfo)
        {
            Contract.Requires(typeInfo != null);

            return typeInfo.GenericTypeParameters.Count > 0 && typeInfo.GenericTypeDefinition == null;
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
            Contract.Requires(typeInfo != null);

            var qualifiedFullName = typeInfo.AssemblyQualifiedName;
            if (string.IsNullOrEmpty(qualifiedFullName))
            {
                return qualifiedFullName;
            }

            if (stripVersionInfo)
            {
                // find the second occurance of , and cut there.
                var index = qualifiedFullName.IndexOf(',');
                if (index > 0)
                {
                    index = qualifiedFullName.IndexOf(',', index + 1);
                }

                if (index > 0)
                {
                    qualifiedFullName = qualifiedFullName.Substring(0, index);
                }
            }

            return qualifiedFullName;
        }
    }
}