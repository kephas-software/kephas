// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAssemblyLoader.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Reflection;
using System.Runtime.Loader;

namespace Kephas.Runtime;

/// <summary>
/// Service for loading application assemblies.
/// </summary>
public interface IAssemblyLoader
{
    /// <summary>
    /// Attempts to load an assembly.
    /// </summary>
    /// <param name="assemblyFilePath">The file path of the assembly to be loaded.</param>
    /// <returns>
    /// The resolved assembly reference.
    /// </returns>
    Assembly LoadAssemblyFromPath(string assemblyFilePath) =>
        AssemblyLoadContext.Default.LoadFromAssemblyPath(assemblyFilePath);

    /// <summary>
    /// Gets the loaded assemblies.
    /// </summary>
    /// <returns>An enumeration of assemblies.</returns>
    IEnumerable<Assembly> GetLoadedAssemblies() => AssemblyLoadContext.Default.Assemblies;

    /// <summary>
    /// Loads application assemblies with the provided filter, using the additional bin locations,
    /// if any of these is provided.
    /// </summary>
    /// <remarks>
    /// Assemblies that are already loaded are not loaded anymore.
    /// </remarks>
    /// <param name="assemblyFilter">The assembly filer.</param>
    /// <param name="binLocations">The bin locations.</param>
    /// <returns>An enumeration of assemblies.</returns>
    IEnumerable<Assembly> LoadAssemblies(Func<AssemblyName, bool>? assemblyFilter = null, IEnumerable<string>? binLocations = null);
}