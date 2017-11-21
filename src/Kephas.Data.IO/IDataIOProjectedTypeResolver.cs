// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataIOProjectedTypeResolver.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the IDataImportProjectedTypeResolver interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.IO
{
    using System;

    using Kephas.Services;

    /// <summary>
    /// Interface for data I/O projected type resolver.
    /// </summary>
    [SharedAppServiceContract]
    public interface IDataIOProjectedTypeResolver
    {
        /// <summary>
        /// Resolves the projected type of the type provided as projection.
        /// </summary>
        /// <param name="projectionType">The projection type.</param>
        /// <param name="context">The data I/O context.</param>
        /// <param name="throwOnNotFound">Indicates whether to throw or not when the indicated type is not found (optional).</param>
        /// <returns>
        /// The projected type.
        /// </returns>
        Type ResolveProjectedType(Type projectionType, IDataIOContext context, bool throwOnNotFound = true);
    }
}