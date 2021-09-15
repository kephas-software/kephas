// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PatternAttribute.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Injection.Metadata;

namespace Kephas.AspNetCore.SignalR.Composition
{
    using System;

    /// <summary>
    /// Attribute for hub pattern.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class PatternAttribute : Attribute, IMetadataValue<string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PatternAttribute"/> class.
        /// </summary>
        /// <param name="pattern">The hub pattern.</param>
        public PatternAttribute(string pattern)
        {
            this.Value = pattern;
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
        public string Value { get; }
    }
}