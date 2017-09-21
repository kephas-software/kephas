// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SharingBoundaryScopeAttribute.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
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
    public class SharingBoundaryScopeAttribute : Attribute, IMetadataValue<string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SharingBoundaryScopeAttribute"/> class.
        /// </summary>
        /// <param name="scopeName">Name of the scope.</param>
        public SharingBoundaryScopeAttribute(string scopeName)
        {
            Requires.NotNullOrEmpty(scopeName, nameof(scopeName));

            this.Value = scopeName;
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