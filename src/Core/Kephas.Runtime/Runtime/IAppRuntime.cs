// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAppRuntime.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Interface for abstracting away the runtime for the application.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Net;
using System.Reflection;

namespace Kephas.Runtime;

/// <summary>
/// Interface for abstracting away the runtime for the application.
/// </summary>
public interface IAppRuntime
{
    /// <summary>
    /// The Instance ID is the current timestamp offset related to 2024.10.01 UTC (to keep the value smaller)
    /// </summary>
    private static readonly string? InstanceId = (DateTime.UtcNow.Ticks - new DateTimeOffset(2024, 10, 1, 0, 0, 0, TimeSpan.Zero).Ticks).ToString();
    private static string? _machineName;

    /// <summary>
    /// Gets the application's entry assembly.
    /// </summary>
    Assembly? GetEntryAssembly() => Assembly.GetEntryAssembly();

    /// <summary>
    /// Gets the root path of the application.
    /// </summary>
    /// <returns>The root path of the application.</returns>
    string? GetAppLocation() => (GetEntryAssembly() ?? Assembly.GetExecutingAssembly()).GetLocationDirectory();
    
    /// <summary>
    /// Gets the application assemblies.
    /// </summary>
    /// <returns>
    /// An enumeration of application assemblies.
    /// </returns>
    IEnumerable<Assembly> GetAppAssemblies();

    /// <summary>
    /// Gets the name identifying the application.
    /// </summary>
    /// <returns>
    /// The name of the application.
    /// </returns>
    string? GetAppName() => GetAppNameFromAssembly(GetEntryAssembly());

    /// <summary>
    /// Gets the version of the application.
    /// </summary>
    /// <returns>
    /// The version of the application.
    /// </returns>
    string? GetAppVersion() => GetAppVersionFromAssembly(GetEntryAssembly());

    /// <summary>
    /// Gets the identifier of the application instance.
    /// </summary>
    /// <returns>
    /// The identifier of the application instance.
    /// </returns>
    string? GetAppInstanceId() => GetAppInstanceIdFromAssembly(GetEntryAssembly());

    /// <summary>
    /// Gets the running environment.
    /// </summary>
    /// <returns>The running environment.</returns>
    string? GetEnvironment() => GetEnvironmentFromEnvironmentVariable(GetAppName());

    /// <summary>
    /// Gets the machine name.
    /// </summary>
    /// <returns>The machine name.</returns>
    string GetMachineName()
    {
        if (_machineName is not null)
        {
            return _machineName;
        }

        try
        {
            return _machineName = Dns.GetHostName();
        }
        catch
        {
            try
            {
                return _machineName = Environment.MachineName;
            }
            catch
            {
                return ".";
            }
        }
    }

    /// <summary>
    /// Gets the application name from the assembly.
    /// </summary>
    /// <remarks>
    /// Tries to get the application name from the assembly, querying the assembly attributes in this order:
    /// <see cref="AssemblyTitleAttribute"/>,
    /// <see cref="AssemblyProductAttribute"/>.
    /// It falls back to the assembly name.
    /// </remarks>
    public static string? GetAppNameFromAssembly(Assembly? assembly) =>
        assembly?.GetCustomAttribute<AssemblyTitleAttribute>()?.Title ??
        assembly?.GetCustomAttribute<AssemblyProductAttribute>()?.Product ??
        assembly?.GetName().Name;

    /// <summary>
    /// Gets the application version from the assembly.
    /// </summary>
    /// <remarks>
    /// Tries to get the application version from the assembly, querying the assembly attributes in this order:
    /// <see cref="AssemblyInformationalVersionAttribute"/>,
    /// <see cref="AssemblyFileVersionAttribute"/>,
    /// <see cref="AssemblyVersionAttribute"/>.
    /// </remarks>
    public static string? GetAppVersionFromAssembly(Assembly? assembly) =>
        assembly?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ??
        assembly?.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version ??
        assembly?.GetCustomAttribute<AssemblyVersionAttribute>()?.Version;

    /// <summary>
    /// Gets the application instance from the assembly.
    /// </summary>
    /// <remarks>
    /// It appends to the <see cref="GetAppNameFromAssembly"/> a computed <see cref="InstanceId"/>.
    /// </remarks>
    public static string GetAppInstanceIdFromAssembly(Assembly? assembly) =>
        GetAppInstanceIdFromAppName(GetAppNameFromAssembly(assembly) ?? "APP");

    /// <summary>
    /// Gets the application instance from the app name.
    /// </summary>
    /// <remarks>
    /// It appends to the <paramref name="appName"/> a computed <see cref="InstanceId"/>.
    /// </remarks>
    public static string GetAppInstanceIdFromAppName(string appName) =>
        $"{appName}-{InstanceId}";
    
    /// <summary>
    /// Gets the environment from the 
    /// </summary>
    /// <remarks>
    /// The default value is the environment variable named: {AppName}_ENVIRONMENT.
    /// </remarks>
    public static string? GetEnvironmentFromEnvironmentVariable(string? appName) => Environment.GetEnvironmentVariable($"{appName?.ToUpper() ?? "APP"}_ENVIRONMENT");
}