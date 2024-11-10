// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FailureStatusAttribute.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.DependencyInjection.Metadata;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Kephas.Extensions.Diagnostics.HealthChecks;

/// <summary>
/// Metadata value for the <see cref="HealthCheckMetadata.FailureStatus"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class FailureStatusAttribute(HealthStatus value) : ServiceMetadataValueAttributeBase<HealthStatus>(value);