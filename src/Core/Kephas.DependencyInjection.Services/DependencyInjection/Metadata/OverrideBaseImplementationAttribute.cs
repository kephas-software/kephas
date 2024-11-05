// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OverrideAttribute.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the override attribute class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.DependencyInjection.Metadata;

/// <summary>
/// Attribute for indicating that classifiers or members override their base. This class cannot be inherited.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Property | AttributeTargets.Method, Inherited = false)]
public sealed class OverrideBaseImplementationAttribute : Attribute, IServiceMetadataValue<bool>
{
    /// <summary>
    /// Gets a value indicating whether the decorated element is an override.
    /// </summary>
    /// <value>
    /// The metadata value.
    /// </value>
    public bool Value => true;

    /// <summary>
    /// Gets the metadata name. If the name is not provided, it is inferred from the attribute type name.
    /// </summary>
    string? IServiceMetadataValue.Name => nameof(ServiceMetadata.OverrideBaseImplementation);
}
