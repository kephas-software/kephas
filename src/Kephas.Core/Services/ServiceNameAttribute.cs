// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceNameAttribute.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the service name attribute class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services
{
    using System;

    using Kephas.Composition.Metadata;
    using Kephas.Diagnostics.Contracts;

    /// <summary>
    /// Attribute for naming services.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ServiceNameAttribute : Attribute, IMetadataValue<string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceNameAttribute"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public ServiceNameAttribute(string value)
        {
            Requires.NotNullOrEmpty(value, nameof(value));

            this.Value = value;
        }

        /// <summary>Gets the metadata value.</summary>
        /// <value>The metadata value.</value>
        object IMetadataValue.Value => this.Value;

        /// <summary>Gets the metadata value.</summary>
        /// <value>The metadata value.</value>
        public string Value { get; }
    }
}