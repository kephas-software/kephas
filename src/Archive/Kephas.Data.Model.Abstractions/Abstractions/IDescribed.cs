// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDescribed.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IDescribed interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Model.Abstractions
{
    using System.ComponentModel.DataAnnotations;

    using Kephas.Data.Model.Abstractions.Resources;
    using Kephas.Model.AttributedModel;

    /// <summary>
    /// Mixin providing the <see cref="Description"/> property.
    /// </summary>
    [Mixin]
    public interface IDescribed
    {
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        [Display(ResourceType = typeof(ModelStrings), Name = "Described_Description_Name", Description = "Described_Description_Description")]
        string Description { get; set; }
    }
}