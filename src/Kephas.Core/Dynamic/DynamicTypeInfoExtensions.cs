// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DynamicTypeInfoExtensions.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the dynamic type information extensions class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Dynamic
{
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Extension methods for <see cref="IDynamicTypeInfo"/>.
    /// </summary>
    public static class DynamicTypeInfoExtensions
    {
        /// <summary>
        /// Creates an instance with the provided arguments (if any).
        /// </summary>
        /// <param name="typeInfo">The type information.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>
        /// The new instance.
        /// </returns>
        public static object CreateInstance(this IDynamicTypeInfo typeInfo, params object[] args)
        {
            Contract.Requires(typeInfo != null);

            return typeInfo.CreateInstance((IEnumerable<object>)args);
        }
    }
}