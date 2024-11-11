// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConnectionNameAttribute.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.DependencyInjection.Metadata;

namespace Kephas.Entities;

/// <summary>
/// Metadata attribute providing the connection name.
/// </summary>
/// <param name="connectionName">The connection name.</param>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class ConnectionNameAttribute(string connectionName)
    : ServiceMetadataValueAttributeBase<string>(connectionName);
