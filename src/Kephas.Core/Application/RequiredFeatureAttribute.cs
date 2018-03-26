// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RequiredFeatureAttribute.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the require feature attribute class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application
{
    using System;

    using Kephas.Composition.Metadata;

    /// <summary>
    /// Attribute declaring a required feature for the annotated entity.
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class RequiredFeatureAttribute : Attribute, IMetadataValue<string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequiredFeatureAttribute"/> class.
        /// </summary>
        /// <param name="value">The name of the required feature.</param>
        public RequiredFeatureAttribute(string value)
        {
            this.Value = value;
        }

        /// <summary>
        /// Gets the metadata value for Kephas.
        /// </summary>
        object IMetadataValue.Value => this.Value;

        /// <summary>
        /// Gets the name of the required feature.
        /// </summary>
        public string Value { get; }
    }
}