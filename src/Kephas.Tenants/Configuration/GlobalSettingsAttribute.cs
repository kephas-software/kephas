// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GlobalSettingsAttribute.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Configuration;

using Kephas.Services;

/// <summary>
/// Attribute for indicating global settings, not tenant-specific.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class GlobalSettingsAttribute : Attribute, IMetadataValue<bool>
{
    /// <summary>
    /// Gets a value indicating whether the annotated settings are global, not tenant-specific.
    /// </summary>
    public bool Value => true;
}