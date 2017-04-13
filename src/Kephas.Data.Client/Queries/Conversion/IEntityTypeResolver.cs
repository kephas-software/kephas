// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEntityTypeResolver.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the IEntityTypeResolver interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Client.Queries.Conversion
{
    using System;

    using Kephas.Reflection;
    using Kephas.Services;

    /// <summary>
    /// Interface for entity type resolver.
    /// </summary>
    [SharedAppServiceContract]
    public interface IEntityTypeResolver
    {
        /// <summary>
        /// Resolves an entity type based on the provided client entity type.
        /// </summary>
        /// <param name="clientEntityType">The client entity type.</param>
        /// <param name="throwOnNotFound">Indicates whether to throw or not when the indicated type is not found.</param>
        /// <returns>
        /// A <see cref="ITypeInfo"/>.
        /// </returns>
        Type ResolveEntityType(Type clientEntityType, bool throwOnNotFound = true);
    }
}