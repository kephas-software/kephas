// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TagsAttribute.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.DependencyInjection.Metadata;

namespace Kephas.Extensions.Diagnostics.HealthChecks;

/// <summary>
/// Metadata attribute for <see cref="HealthCheckMetadata.Tags"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class TagsAttribute(params string[] value)
    : ServiceMetadataValueAttributeBase<IEnumerable<string>>(value);
