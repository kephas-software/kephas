// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValidationAnnotation.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the validation annotation class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Model.Elements.Annotations
{
    using System.ComponentModel.DataAnnotations;

    using Kephas.Data.Validation;
    using Kephas.Model.Construction;
    using Kephas.Model.Elements.Annotations;

    /// <summary>
    /// A validation annotation.
    /// </summary>
    public class ValidationAnnotation : AttributeAnnotation<ValidationAttribute>, IValidationAttributeProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationAnnotation"/> class.
        /// </summary>
        /// <param name="constructionContext">Context for the construction.</param>
        /// <param name="name">The model element name.</param>
        /// <param name="validationAttribute">The validation attribute.</param>
        public ValidationAnnotation(IModelConstructionContext constructionContext, string name, ValidationAttribute validationAttribute)
            : base(constructionContext, name, validationAttribute)
        {
        }

        /// <summary>
        /// Gets validation attribute.
        /// </summary>
        /// <returns>
        /// The validation attribute.
        /// </returns>
        public ValidationAttribute GetValidationAttribute()
        {
            return this.Attribute;
        }
    }
}