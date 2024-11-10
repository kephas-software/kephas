// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TimeoutAttribute.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Globalization;
using Kephas.DependencyInjection.Metadata;

namespace Kephas.Extensions.Diagnostics.HealthChecks;

/// <summary>
/// Metadata value for the <see cref="HealthCheckMetadata.Timeout"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class TimeoutAttribute(string value)
    : ServiceMetadataValueAttributeBase<TimeSpan>(TimeSpan.Parse(value, CultureInfo.InvariantCulture));