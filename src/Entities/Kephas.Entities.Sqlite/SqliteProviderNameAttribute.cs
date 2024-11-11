// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqliteProviderNameAttribute.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Entities;

/// <summary>
/// Metadata attribute for the SQLite provider name.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class SqliteProviderNameAttribute() : ProviderNameAttribute("SQLite");
