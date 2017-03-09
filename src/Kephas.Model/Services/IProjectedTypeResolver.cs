// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IProjectedTypeResolver.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the IProjectedTypeResolver interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Services
{
    using System;

    using Kephas.Services;

    /// <summary>
    /// Interface for projected type resolver.
    /// </summary>
    [SharedAppServiceContract]
    public interface IProjectedTypeResolver
    {
        /// <summary>
        /// Resolves the projected type.
        /// </summary>
        /// <param name="projectionType">The projection type.</param>
        /// <param name="throwOnNotFound">Indicates whether to throw or not when the indicated type is not found.</param>
        /// <returns>
        /// A Type.
        /// </returns>
        Type ResolveProjectedType(Type projectionType, bool throwOnNotFound = true);
    }
}