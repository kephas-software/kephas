// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MetadataValueAttribute.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the metadata value attribute class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services
{
    using System;

    /// <summary>
    /// Marks properties which should be included in the service metadata.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class MetadataValueAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MetadataValueAttribute"/> class.
        /// </summary>
        /// <param name="valueName">Name of the value. If not provided, the metadata value will have the name {attribute-name}{property-name}.</param>
        public MetadataValueAttribute(string valueName = null)
        {
            this.ValueName = valueName;
        }

        /// <summary>
        /// Gets the name of the metadata value.
        /// </summary>
        /// <value>
        /// The name of the metadata value.
        /// </value>
        public string ValueName { get; }
    }
}