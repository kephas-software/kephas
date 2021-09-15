// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OverrideAttribute.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the override attribute class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Injection.Metadata;

namespace Kephas.Model.AttributedModel
{
    using System;

    /// <summary>
    /// Attribute for indicating that classifiers or members override their base. This class cannot be inherited.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Property | AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class OverrideAttribute : Attribute, IMetadataValue<bool>
    {
        /// <summary>
        /// Gets the metadata value.
        /// </summary>
        /// <value>
        /// The metadata value.
        /// </value>
        object IMetadataValue.Value => true;

        /// <summary>
        /// Gets the metadata value.
        /// </summary>
        /// <value>
        /// The metadata value.
        /// </value>
        public bool Value => true;
    }
}