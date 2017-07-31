// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultDataImportProjectedTypeResolver.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the default data import projected type resolver class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.IO.Import
{
    using System;

    using Kephas.Services;

    /// <summary>
    /// A default data import projected type resolver.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class DefaultDataImportProjectedTypeResolver : IDataImportProjectedTypeResolver
    {
        /// <summary>
        /// Resolves the projected type of the type provided as projection.
        /// </summary>
        /// <param name="projectionType">The projection type.</param>
        /// <param name="context">The data import context.</param>
        /// <param name="throwOnNotFound">Indicates whether to throw or not when the indicated type is not found (optional).</param>
        /// <returns>
        /// The projected type.
        /// </returns>
        public Type ResolveProjectedType(Type projectionType, IDataImportContext context, bool throwOnNotFound = true)
        {
            return projectionType;
        }
    }
}