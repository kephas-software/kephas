// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SettingsTypeAttribute.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the settings type attribute class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Configuration
{
    using System;

    using Kephas.Composition.Metadata;
    using Kephas.Diagnostics.Contracts;

    /// <summary>
    /// Attribute for settings type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class SettingsTypeAttribute : Attribute, IMetadataValue<Type>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsTypeAttribute"/> class.
        /// </summary>
        /// <param name="value">The metadata value.</param>
        public SettingsTypeAttribute(Type value)
        {
            Requires.NotNull(value, nameof(value));

            this.Value = value;
        }

        /// <summary>
        /// Gets the metadata value.
        /// </summary>
        /// <value>
        /// The metadata value.
        /// </value>
        object IMetadataValue.Value => this.Value;

        /// <summary>
        /// Gets the metadata value.
        /// </summary>
        /// <value>
        /// The metadata value.
        /// </value>
        public Type Value { get; }
    }
}