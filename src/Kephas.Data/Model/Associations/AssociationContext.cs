// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssociationContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Model.Associations
{
    using System;
    using System.Collections.Generic;
    using Kephas.Services;
    using Kephas.Reflection;
    using Kephas.Services;

    /// <summary>
    /// Context for association operations.
    /// </summary>
    public class AssociationContext : Context, IAssociationContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AssociationContext"/> class.
        /// </summary>
        /// <param name="serviceProvider">The DI injector.</param>
        public AssociationContext(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
        }

        /// <summary>
        /// Gets or sets the type registry containing the associated types.
        /// </summary>
        public ITypeRegistry? TypeRegistry { get; set; }

        /// <summary>
        /// Gets or sets the type filter.
        /// </summary>
        public Func<ITypeInfo, IContext?, bool>? TypeFilter { get; set; }

        /// <summary>
        /// Gets or sets the reference type information (typically <see cref="IRef{T}"/>).
        /// </summary>
        public ITypeInfo? RefTypeInfo { get; set; }

        /// <summary>
        /// Gets or sets the collection type information (typically <see cref="ICollection{T}"/>).
        /// </summary>
        public ITypeInfo? CollectionTypeInfo { get; set; }
    }
}