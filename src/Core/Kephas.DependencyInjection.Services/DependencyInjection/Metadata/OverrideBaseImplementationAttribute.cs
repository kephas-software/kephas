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
[AttributeUsage(
    AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Property | AttributeTargets.Method,
    Inherited = false)]
public sealed class OverrideBaseImplementationAttribute() : ServiceMetadataValueAttributeBase<bool>(true);