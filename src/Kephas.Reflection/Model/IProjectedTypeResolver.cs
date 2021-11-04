// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IProjectedTypeResolver.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IProjectedTypeResolver interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model
{
    using System;

    using Kephas.Services;

    /// <summary>
    /// Interface for projected type resolver.
    /// </summary>
    [SingletonAppServiceContract]
    public interface IProjectedTypeResolver
    {
        /// <summary>
        /// Resolves the projected type.
        /// </summary>
        /// <param name="projectionType">The projection type.</param>
        /// <param name="context">Optional. A context for the resolution.</param>
        /// <param name="throwOnNotFound">Optional. Indicates whether to throw or not when the indicated type is not found.</param>
        /// <returns>
        /// The resolved type or <c>null</c>, if <paramref name="throwOnNotFound"/> is set to false and a projected type could not be found.
        /// </returns>
        Type? ResolveProjectedType(Type projectionType, IContext? context = null, bool throwOnNotFound = true);
    }
}