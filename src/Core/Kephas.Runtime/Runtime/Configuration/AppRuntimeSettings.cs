// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppRuntimeSettings.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Reflection;

namespace Kephas.Runtime.Configuration;

/// <summary>
/// Settings for the application runtime.
/// </summary>
public record AppRuntimeSettings
{
    /// <summary>
    /// The default value for the <see cref="AppAssemblyNamePattern"/>.
    /// </summary>
    /// <remarks>
    /// System assemblies and test assemblies are excluded.
    /// </remarks>
    public static readonly string DefaultAppAssemblyNamePattern = "^(?!System)(?!Microsoft)(?!mscorlib)(?!netstandard)(?!vshost32)(?!Mono\\.)[A-Za-z0-9\\.]+(?!<Test(s)?)$";

    /// <summary>
    /// Gets or sets the entry assembly.
    /// </summary>
    public Assembly? EntryAssembly { get; set; } = Assembly.GetEntryAssembly();

    /// <summary>
    /// Gets or sets the name pattern for application specific assemblies.
    /// </summary>
    public string AppAssemblyNamePattern { get; set; } = DefaultAppAssemblyNamePattern;

    /// <summary>
    /// Gets or sets the application name.
    /// </summary>
    public string? AppName { get; set; }

    /// <summary>
    /// Gets or sets the environment name.
    /// </summary>
    public string? Environment { get; set; }
}