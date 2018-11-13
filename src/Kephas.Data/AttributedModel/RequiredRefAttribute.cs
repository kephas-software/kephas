// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RequiredRefAttribute.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the required reference attribute class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.AttributedModel
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using Kephas.Data.Resources;

    /// <summary>
    /// Attribute marking required references.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public class RequiredRefAttribute : ValidationAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequiredRefAttribute"/> class.
        /// </summary>
        public RequiredRefAttribute()
            : base(() => Strings.RequiredRefAttribute_ValidationError)
        {
        }

        /// <summary>
        /// Validates the specified value with respect to the current validation attribute.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <param name="validationContext">The context information about the validation operation.</param>
        /// <returns>
        /// An instance of the <see cref="T:System.ComponentModel.DataAnnotations.ValidationResult" />
        /// class.
        /// </returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (!this.IsEmptyRef(value))
            {
                return ValidationResult.Success;
            }

            var memberNames = validationContext.MemberName != null ? new[] { validationContext.MemberName } : null;
            return new ValidationResult(this.FormatErrorMessage(validationContext.DisplayName), memberNames);
        }

        /// <summary>Checks that the value of the required data field is not empty.</summary>
        /// <returns>true if validation is successful; otherwise, false.</returns>
        /// <param name="value">The data field value to validate.</param>
        /// <exception cref="T:System.ComponentModel.DataAnnotations.ValidationException">The data field value was null.</exception>
        protected virtual bool IsEmptyRef(object value)
        {
            if (value is IRef refValue)
            {
                return !refValue.IsEmpty;
            }

            return !Id.IsEmpty(value);
        }
    }
}