// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITagged.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the ITagged interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Model.Abstractions
{
    using System.ComponentModel.DataAnnotations;

    using Kephas.Data.Model.Abstractions.Resources;
    using Kephas.Model.AttributedModel;

    /// <summary>
    /// Mixin providing the <see cref="Tags"/> property.
    /// </summary>
    [Mixin]
    public interface ITagged
    {
        /// <summary>
        /// Gets or sets the tags.
        /// </summary>
        /// <value>
        /// The tags.
        /// </value>
        [Display(ResourceType = typeof(ModelStrings), Name = "Tagged_Tags_Name")]
        string[] Tags { get; set; }
    }
}