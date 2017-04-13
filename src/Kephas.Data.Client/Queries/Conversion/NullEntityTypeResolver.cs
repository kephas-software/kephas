// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NullEntityTypeResolver.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the null entity type resolver class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Client.Queries.Conversion
{
    using System;

    using Kephas.Reflection;
    using Kephas.Services;

    /// <summary>
    /// A null entity type resolver.
    /// </summary>
    [OverridePriority(Priority.Lowest)]
    public class NullEntityTypeResolver : IEntityTypeResolver
    {
        /// <summary>
        /// Resolves an entity type based on the provided client entity type.
        /// </summary>
        /// <param name="clientEntityType">The client entity type.</param>
        /// <param name="throwOnNotFound">Indicates whether to throw or not when the indicated type is not found.</param>
        /// <returns>
        /// A <see cref="ITypeInfo"/>.
        /// </returns>
        public Type ResolveEntityType(Type clientEntityType, bool throwOnNotFound = true)
        {
            return clientEntityType;
        }
    }
}