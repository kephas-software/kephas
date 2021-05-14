// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAssociationContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Model.Associations
{
    using System;
    using System.Collections.Generic;

    using Kephas.Reflection;
    using Kephas.Services;

    /// <summary>
    /// Provides context for association information.
    /// </summary>
    public interface IAssociationContext : IContext
    {
        /// <summary>
        /// Gets or sets the type registry containing the associated types.
        /// </summary>
        ITypeRegistry? TypeRegistry { get; set; }

        /// <summary>
        /// Gets or sets the type filter.
        /// </summary>
        Func<ITypeInfo, IContext?, bool>? TypeFilter { get; set; }

        /// <summary>
        /// Gets or sets the reference type information (typically <see cref="IRef{T}"/>).
        /// </summary>
        ITypeInfo? RefTypeInfo { get; set; }

        /// <summary>
        /// Gets or sets the collection type information (typically <see cref="ICollection{T}"/>).
        /// </summary>
        ITypeInfo? CollectionTypeInfo { get; set; }
    }
}