// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimeTypeInfoExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the runtime type information extensions class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Runtime
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Extension methods for <see cref="IRuntimeTypeInfo"/>.
    /// </summary>
    public static class RuntimeTypeInfoExtensions
    {
        /// <summary>
        /// Creates an instance with the provided arguments (if any).
        /// </summary>
        /// <param name="typeInfo">The type information.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>
        /// The new instance.
        /// </returns>
        public static object CreateInstance(this IRuntimeTypeInfo typeInfo, params object[] args)
        {
            typeInfo = typeInfo ?? throw new ArgumentNullException(nameof(typeInfo));

            return typeInfo.CreateInstance((IEnumerable<object>)args);
        }
    }
}