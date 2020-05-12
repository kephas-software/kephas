// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITypeResolver.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the ITypeResolver interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Reflection
{
    using System;

    using Kephas.Services;

    /// <summary>
    /// Service for resolving types from type names.
    /// </summary>
    [SingletonAppServiceContract]
    public interface ITypeResolver
    {
        /// <summary>
        /// Resolves a type based on the provided type name.
        /// </summary>
        /// <param name="typeName">Name of the type.</param>
        /// <param name="throwOnNotFound">Indicates whether to throw or not when the indicated type is not found.</param>
        /// <returns>
        /// A Type.
        /// </returns>
        Type? ResolveType(string typeName, bool throwOnNotFound = true);
    }
}