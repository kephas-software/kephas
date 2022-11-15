// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TargetPackageAttribute.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the target package attribute class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Setup.AttributedModel
{
    using System;

    using Kephas.Injection;

    /// <summary>
    /// Attribute for indicating the package targeted by the <see cref="IDataInstaller"/>.
    /// </summary>
    public class TargetPackageAttribute : Attribute, IMetadataValue<string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TargetPackageAttribute"/> class.
        /// </summary>
        /// <param name="target">The target package.</param>
        public TargetPackageAttribute(string target)
        {
            this.Value = target;
        }

        /// <summary>
        /// Gets the metadata value.
        /// </summary>
        /// <value>
        /// The metadata value.
        /// </value>
        public string Value { get; }

        /// <summary>
        /// Gets the metadata name. If the name is not provided, it is inferred from the attribute type name.
        /// </summary>
        string? IMetadataValue.Name => nameof(DataInstallerMetadata.Target);
    }
}