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

    /// <summary>
    /// Attribute marking required references.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public class RequiredRefAttribute : RequiredAttribute
    {
        /// <summary>Checks that the value of the required data field is not empty.</summary>
        /// <returns>true if validation is successful; otherwise, false.</returns>
        /// <param name="value">The data field value to validate.</param>
        /// <exception cref="T:System.ComponentModel.DataAnnotations.ValidationException">The data field value was null.</exception>
        public override bool IsValid(object value)
        {
            if (value is IRef refValue)
            {
                return !refValue.IsEmpty;
            }

            return !Id.IsEmpty(value);
        }
    }
}