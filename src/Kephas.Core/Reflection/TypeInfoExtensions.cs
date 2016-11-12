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
    }
}