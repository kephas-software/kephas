﻿// --------------------------------------------------------------------------------------------------------------------
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

    using Kephas.Services;

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
            this.Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>Gets the metadata value.</summary>
        /// <value>The metadata value.</value>
        public string Value { get; }
    }
}