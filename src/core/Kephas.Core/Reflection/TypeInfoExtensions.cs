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

    using Kephas.Dynamic;

    /// <summary>
    /// Extension methods for type information.
    /// </summary>
    public static class TypeInfoExtensions
    {
        /// <summary>
        /// Gets the <see cref="IDynamicTypeInfo"/> for the provided <see cref="TypeInfo"/> instance.
        /// </summary>
        /// <param name="typeInfo">The type information instance.</param>
        /// <returns>
        /// The provided <see cref="TypeInfo"/>'s associated <see cref="IDynamicTypeInfo"/>.
        /// </returns>
        public static IDynamicTypeInfo GetDynamicTypeInfo(this TypeInfo typeInfo)
        {
            Contract.Requires(typeInfo != null);

            return DynamicTypeInfo.GetDynamicType(typeInfo);
        }
    }
}