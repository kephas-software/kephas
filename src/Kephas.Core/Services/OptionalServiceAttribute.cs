// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OptionalServiceAttribute.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the optional service attribute class
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services
{
    using System;

    using Kephas.Composition.Metadata;

    /// <summary>
    /// Attribute decorating an optional service.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class OptionalServiceAttribute : Attribute, IMetadataValue<bool>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OptionalServiceAttribute"/> class.
        /// </summary>
        public OptionalServiceAttribute()
        {
            this.Value = true;
        }

        /// <summary>
        /// Gets the metadata value.
        /// </summary>
        /// <value>
        /// The metadata value.
        /// </value>
        object IMetadataValue.Value => this.Value;

        /// <summary>
        /// Gets a value indicating whether the service is optional for its consumers.
        /// </summary>
        /// <value>
        /// The metadata value.
        /// </value>
        public bool Value { get; }
    }
}