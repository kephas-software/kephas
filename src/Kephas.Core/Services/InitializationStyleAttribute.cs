// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InitializationStyleAttribute.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the service initialization attribute class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services
{
    using System;
    using Kephas.Composition.Metadata;

    /// <summary>
    /// Values that represent initialization styles.
    /// </summary>
    /// <seealso/>
    public enum InitializationStyle
    {
        /// <summary>
        /// The service must be initialized before the application is initialized.
        /// </summary>
        BeforeAppInitialize,

        /// <summary>
        /// The service must be initialized before the application is initialized.
        /// </summary>
        AfterAppInitialize,
    }

    /// <summary>
    /// Attribute for service initialization style. This class cannot be inherited.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class InitializationStyleAttribute : Attribute, IMetadataValue<InitializationStyle>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InitializationStyleAttribute"/> class.
        /// </summary>
        /// <param name="value">The metadata value.</param>
        public InitializationStyleAttribute(InitializationStyle value)
        {
            Value = value;
        }

        /// <summary>
        /// Gets the metadata value.
        /// </summary>
        /// <value>
        /// The metadata value.
        /// </value>
        public InitializationStyle Value { get; }

        /// <summary>
        /// Gets the metadata value.
        /// </summary>
        /// <value>
        /// The metadata value.
        /// </value>
        object IMetadataValue.Value => Value;
    }
}
