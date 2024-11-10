// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Extension methods for types.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Reflection
{
    using System;

    using Kephas.Runtime;

    /// <summary>
    /// Extension methods for types.
    /// </summary>
    public static class ReflectionTypeExtensions
    {
        /// <summary>
        /// Gets the <see cref="IRuntimeTypeInfo"/> for the provided <see cref="Type"/> instance.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        /// The provided <see cref="Type"/>'s associated <see cref="IRuntimeTypeInfo"/>.
        /// </returns>
        public static IRuntimeTypeInfo AsRuntimeTypeInfo(this Type type)
        {
            type = type ?? throw new ArgumentNullException(nameof(type));

            return RuntimeTypeRegistry.Instance.GetTypeInfo(type);
        }
    }
}