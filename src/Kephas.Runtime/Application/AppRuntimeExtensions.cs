// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppRuntimeExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application;

using Kephas.IO;

/// <summary>
/// Extension methods for <see cref="IAppRuntime"/>.
/// </summary>
public static class AppRuntimeExtensions
{
    private const string GetLocationsToken = $"__{nameof(GetLocationsToken)}";

    /// <summary>
    /// Gets a value indicating whether the assembly with the provided name is an application-specific assembly.
    /// </summary>
    /// <param name="appRuntime">The application runtime.</param>
    /// <param name="logicalName">The logical name of the locations.</param>
    /// <param name="basePath">The base path.</param>
    /// <param name="relativePaths">The relative paths.</param>
    /// <returns>The assembly filter.</returns>
    public static ILocations GetLocations(this IAppRuntime appRuntime, string logicalName, string basePath, IEnumerable<string> relativePaths)
    {
        appRuntime = appRuntime ?? throw new ArgumentNullException(nameof(appRuntime));

        if (appRuntime[GetLocationsToken] is Func<string, string, IEnumerable<string>, ILocations> getLocations)
        {
            return getLocations(logicalName, basePath, relativePaths);
        }

        return new FolderLocations(relativePaths, basePath, logicalName);
    }
}