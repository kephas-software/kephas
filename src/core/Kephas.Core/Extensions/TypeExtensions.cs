// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeExtensions.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Extension methods for types.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Extensions
{
    using System;
    using System.Diagnostics.Contracts;

    using Kephas.Dynamic;

    /// <summary>
    /// Extension methods for types.
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// Gets the dynamic type for the provided type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The provided type's associated dynamic type.</returns>
        public static IDynamicType GetDynamicType(this Type type)
        {
            Contract.Requires(type != null);

            return RuntimeDynamicType.GetDynamicType(type);
        }
    }
}
