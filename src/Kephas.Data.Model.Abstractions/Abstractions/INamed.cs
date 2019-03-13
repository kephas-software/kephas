// --------------------------------------------------------------------------------------------------------------------
// <copyright file="INamed.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the INamed interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Model.Abstractions
{
    using System.ComponentModel.DataAnnotations;

    using Kephas.Data.Model.Abstractions.Resources;
    using Kephas.Model.AttributedModel;

    /// <summary>
    /// Mixin providing the <see cref="Name"/> property.
    /// </summary>
    [Mixin]
    public interface INamed
    {
        /// <summary>
        /// Gets or sets the entity name.
        /// </summary>
        /// <value>
        /// The entity name.
        /// </value>
        [Required(AllowEmptyStrings = false)]
        [Display(ResourceType = typeof(ModelStrings), Name = "Named_Name_Name")]
        string Name { get; set; }
    }
}