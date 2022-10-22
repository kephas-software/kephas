// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RequiresFeatureAttribute.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the require feature attribute class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application
{
    using System;

    using Kephas.Services;

    /// <summary>
    /// Attribute declaring that the annotated entity requires a specific feature.
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class RequiresFeatureAttribute : Attribute, IMetadataValue<string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequiresFeatureAttribute"/> class.
        /// </summary>
        /// <param name="value">The name of the required feature.</param>
        public RequiresFeatureAttribute(string value)
        {
            this.Value = value;
        }

        /// <summary>
        /// Gets the name of the required feature.
        /// </summary>
        public string Value { get; }
    }
}