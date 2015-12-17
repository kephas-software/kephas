// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeExtensions.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Extension methods for types.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Reflection
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
        /// Gets the <see cref="IDynamicTypeInfo"/> for the provided <see cref="Type"/> instance.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        /// The provided <see cref="Type"/>'s associated <see cref="IDynamicTypeInfo"/>.
        /// </returns>
        public static IDynamicTypeInfo GetDynamicTypeInfo(this Type type)
        {
            Contract.Requires(type != null);

            return DynamicTypeInfo.GetDynamicType(type);
        }
    }
}
