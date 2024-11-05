// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimeHelper.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Reflection;

namespace Kephas;

/// <summary>
/// Helper class for the runtime.
/// </summary>
public static class RuntimeHelper
{
    /// <summary>
    /// Gets the location directory for the provided assembly.
    /// </summary>
    /// <param name="assembly">The assembly to be checked.</param>
    /// <returns>
    /// The assembly location directory.
    /// </returns>
    public static string GetLocationDirectory(this Assembly assembly)
    {
        _ = assembly ?? throw new ArgumentNullException(nameof(assembly));

        var location = Path.GetDirectoryName(assembly.Location)!;
        return location;
    }
}
