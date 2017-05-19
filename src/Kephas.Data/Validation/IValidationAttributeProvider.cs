// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IValidationAttributeProvider.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the IValidationAttributeProvider interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Validation
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Interface for validation attribute provider.
    /// </summary>
    public interface IValidationAttributeProvider
    {
        /// <summary>
        /// Gets validation attribute.
        /// </summary>
        /// <returns>
        /// The validation attribute.
        /// </returns>
        ValidationAttribute GetValidationAttribute();
    }
}