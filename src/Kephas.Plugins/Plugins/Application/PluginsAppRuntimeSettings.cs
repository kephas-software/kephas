// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PluginsAppRuntimeSettings.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Plugins.Application;

using Kephas.Application;

/// <summary>
/// Settings for the <see cref="PluginsAppRuntime"/>.
/// </summary>
public class PluginsAppRuntimeSettings : AppRuntimeSettings
{
    /// <summary>
    /// Gets or sets a value indicating whether plugins are enabled.
    /// </summary>
    public bool? EnablePlugins { get; set; }

    /// <summary>
    /// Gets or sets a value indicating the plugins installation folder.
    /// </summary>
    public string? PluginsFolder { get; set; }

    /// <summary>
    /// Gets or sets the plugin repository.
    /// </summary>
    public IPluginStore? PluginRepository { get; set; }
}