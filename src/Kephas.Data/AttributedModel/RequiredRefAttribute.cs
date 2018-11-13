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

        /// <summary>Determines whether the specified value of the object is valid. </summary>
        /// <returns>true if the specified value is valid; otherwise, false.</returns>
        /// <param name="value">The value of the object to validate. </param>
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