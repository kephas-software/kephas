// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompositionScopeAttribute.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the sharing boundary scope attribute class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Mef.ScopeFactory
{
    using System;

    using Kephas.Composition.Metadata;
    using Kephas.Diagnostics.Contracts;

    /// <summary>
    /// Attribute for MEF scope.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class CompositionScopeAttribute : Attribute, IMetadataValue<string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CompositionScopeAttribute"/> class.
        /// </summary>
        public CompositionScopeAttribute()
        {
            this.Value = CompositionScopeNames.Default;
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