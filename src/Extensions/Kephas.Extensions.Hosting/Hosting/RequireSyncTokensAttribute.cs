// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RequireSyncTokensAttribute.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.DependencyInjection.Metadata;

namespace Kephas.Extensions.Hosting;

/// <summary>
/// Attribute providing the <see cref="SynchronizedHostedServiceMetadata.RequireSyncTokens"/>.
/// </summary>
/// <param name="value">The required synchronization tokens.</param>
[AttributeUsage(AttributeTargets.Class)]
public class RequireSyncTokensAttribute(params string[] value)
    : ServiceMetadataValueAttributeBase<string[]>(value);