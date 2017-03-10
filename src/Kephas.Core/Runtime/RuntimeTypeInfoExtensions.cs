// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimeTypeInfoExtensions.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the runtime type information extensions class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Runtime
{
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    using Kephas.Diagnostics.Contracts;

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
            Requires.NotNull(typeInfo, nameof(typeInfo));

            return typeInfo.CreateInstance((IEnumerable<object>)args);
        }
    }
}