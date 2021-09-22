// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InjectionScopeAttribute.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the sharing boundary scope attribute class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Injection.SystemComposition.ScopeFactory
{
    using System;
    using Kephas.Injection.Metadata;

    /// <summary>
    /// Attribute indicating the MEF scope.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class InjectionScopeAttribute : Attribute, IMetadataValue<string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InjectionScopeAttribute"/> class.
        /// </summary>
        public InjectionScopeAttribute()
        {
            this.Value = InjectionScopeNames.Default;
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