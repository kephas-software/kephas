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
    using Kephas.Diagnostics.Contracts;
    using Kephas.Model.Construction;
    using Kephas.Model.Elements;

    /// <summary>
    /// A validation annotation.
    /// </summary>
    public class ValidationAnnotation : Annotation, IValidationAttributeProvider
    {
        /// <summary>
        /// The validation attribute.
        /// </summary>
        private ValidationAttribute validationAttribute;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationAnnotation"/> class.
        /// </summary>
        /// <param name="constructionContext">Context for the construction.</param>
        /// <param name="name">The model element name.</param>
        /// <param name="validationAttribute">The validation attribute.</param>
        public ValidationAnnotation(IModelConstructionContext constructionContext, string name, ValidationAttribute validationAttribute)
            : base(constructionContext, name)
        {
            Requires.NotNull(validationAttribute, nameof(validationAttribute));

            this.validationAttribute = validationAttribute;
        }

        /// <summary>
        /// Gets validation attribute.
        /// </summary>
        /// <returns>
        /// The validation attribute.
        /// </returns>
        public ValidationAttribute GetValidationAttribute()
        {
            return this.validationAttribute;
        }
    }
}