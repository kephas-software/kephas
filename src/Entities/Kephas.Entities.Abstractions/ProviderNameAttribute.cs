// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProviderNameAttribute.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.DependencyInjection.Metadata;
using Kephas.Entities.Features;

namespace Kephas.Entities;

/// <summary>
/// Metadata attribute for the provider name.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class ProviderNameAttribute(string providerName)
    : ServiceMetadataValueAttributeBase<string>(providerName), IServiceMetadataValue
{
    /// <summary>
    /// Gets the metadata name. If the name is not provided, it is inferred from the attribute type name.
    /// </summary>
    string? IServiceMetadataValue.Name => nameof(EntityContextFeatureMetadata.ProviderName);
}
