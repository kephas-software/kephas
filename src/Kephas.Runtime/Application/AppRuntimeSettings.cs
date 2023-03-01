// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppRuntimeSettings.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application;

using System.Reflection;
using Kephas.Dynamic;
using Kephas.Logging;

/// <summary>
/// Settings for the application runtime.
/// </summary>
public record AppRuntimeSettings
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AppRuntimeSettings"/> class.
    /// </summary>
    public AppRuntimeSettings()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AppRuntimeSettings"/> class.
    /// </summary>
    /// <param name="appArgs">The application arguments.</param>
    public AppRuntimeSettings(IDynamic appArgs)
    {
        appArgs = appArgs ?? throw new ArgumentNullException(nameof(appArgs));
        this.AppArgs = appArgs as IAppArgs ?? new AppArgs(appArgs);
    }

    /// <summary>
    /// Gets or sets the entry assembly.
    /// </summary>
    public Assembly? EntryAssembly { get; set; } = Assembly.GetEntryAssembly();

    /// <summary>
    /// Gets or sets the application arguments.
    /// </summary>
    public IAppArgs? AppArgs { get; set; }

    /// <summary>
    /// Gets or sets the application ID.
    /// </summary>
    public string? AppId { get; set; }

    /// <summary>
    /// Gets or sets the application instance ID.
    /// </summary>
    public string? AppInstanceId { get; set; }

    /// <summary>
    /// Gets or sets the application version.
    /// </summary>
    public string? AppVersion { get; set; }

    /// <summary>
    /// Gets or sets the application assemblies.
    /// </summary>
    public IEnumerable<Assembly>? AppAssemblies { get; set; }

    /// <summary>
    /// Gets or sets the regular expression for identifying the application assemblies by name.
    /// </summary>
    public string? AppAssemblyNamePattern { get; set; }

    /// <summary>
    /// Gets or sets the function for matching application assemblies by name.
    /// </summary>
    public Func<AssemblyName, bool>? IsAppAssembly { get; set; }

    /// <summary>
    /// Gets or sets the application folder.
    /// </summary>
    public string? AppFolder { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the application is the root.
    /// </summary>
    public bool? IsRoot { get; set; }

    /// <summary>
    /// Gets or sets the configuration folders.
    /// </summary>
    public IEnumerable<string>? ConfigFolders { get; set; }

    /// <summary>
    /// Gets or sets the license folders.
    /// </summary>
    public IEnumerable<string>? LicenseFolders { get; set; }

    /// <summary>
    /// Gets or sets the function for getting the logger.
    /// </summary>
    public Func<string, ILogger>? GetLogger { get; set; }
}