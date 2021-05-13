// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAssociationContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Model.Associations
{
    using System;

    using Kephas.Reflection;
    using Kephas.Services;

    /// <summary>
    /// Provides context for association information.
    /// </summary>
    public interface IAssociationContext : IContext
    {
        /// <summary>
        /// Gets or sets the type filter.
        /// </summary>
        Func<ITypeInfo, IContext?, bool>? TypeFilter { get; set; }
    }
}