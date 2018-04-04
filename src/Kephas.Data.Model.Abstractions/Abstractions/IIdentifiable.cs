// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IIdentifiable.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IIdentifiable interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Model.Abstractions
{
    using System.ComponentModel.DataAnnotations;

    using Kephas.Data.Model.Abstractions.Resources;
    using Kephas.Model.AttributedModel;

    /// <summary>
    /// Base mix-in for identifiable entities.
    /// </summary>
    /// <typeparam name="T">Type of the ID property.</typeparam>
    [Mixin]
    public interface IIdentifiable<T> : IIdentifiable
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [Display(ResourceType = typeof(ModelStrings), Name = "Identifiable_Id_Name")]
        new T Id { get; set; }
    }
}