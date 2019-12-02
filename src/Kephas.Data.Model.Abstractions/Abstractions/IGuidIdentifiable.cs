// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IGuidIdentifiable.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IGuidIdentifiable interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Model.Abstractions
{
    using System;

    using Kephas.Data.Model.AttributedModel;
    using Kephas.Model.AttributedModel;

    /// <summary>
    /// Interface for unique identifier identifiable.
    /// </summary>
    [Mixin]
    [Key(new[] { nameof(Guid) })]
    public interface IGuidIdentifiable
    {
        /// <summary>
        /// Gets or sets a unique identifier.
        /// </summary>
        /// <value>
        /// The identifier of the unique.
        /// </value>
        Guid Guid { get; set; }
    }
}