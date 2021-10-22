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

    using Kephas.Runtime;

    /// <summary>
    /// Extension methods for type information.
    /// </summary>
    public static class TypeInfoExtensions
    {
        /// <summary>
        /// Indicates whether the type is an instance of the generic <see cref="Nullable{T}"/> type.
        /// </summary>
        /// <param name="typeInfo">The type to check.</param>
        /// <returns>
        ///   <c>true</c> if the type is nullable; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNullableType(this ITypeInfo typeInfo)
        {
            typeInfo = typeInfo ?? throw new ArgumentNullException(nameof(typeInfo));

            if (typeInfo is IRuntimeTypeInfo runtimeTypeInfo)
            {
                return runtimeTypeInfo.Type.IsNullableType();
            }

            return false;
        }
    }
}