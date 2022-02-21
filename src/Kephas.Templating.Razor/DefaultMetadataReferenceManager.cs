// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultMetadataReferenceManager.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Templating.Razor;

using System.Reflection;
using System.Reflection.PortableExecutable;
using Kephas.Services;
using Microsoft.CodeAnalysis;

/// <summary>
/// The default implementation of the <see cref="IMetadataReferenceManager"/>.
/// </summary>
[OverridePriority(Priority.Low)]
public class DefaultMetadataReferenceManager : IMetadataReferenceManager
{
    /// <summary>
    /// Gets the excluded assemblies.
    /// </summary>
    /// <value>
    /// The excluded assemblies.
    /// </value>
    public HashSet<string> ExcludedAssemblies { get; } = new();

    /// <summary>
    /// Resolves the specified assemblies.
    /// </summary>
    /// <param name="assemblies">The assemblies.</param>
    /// <returns>
    /// A <see cref="IReadOnlyList{T}" /> of <see cref="MetadataReference" />s.
    /// </returns>
    public IReadOnlyList<MetadataReference> Resolve(IEnumerable<Assembly> assemblies)
    {
        var libraryPaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var context = new HashSet<string>();
        var refAssemblies = assemblies
            .SelectMany(assembly =>
                GetReferencedAssemblies(assembly, this.ExcludedAssemblies, context)
                    .Union(new[] { assembly }))
            .Distinct()
            .ToArray();
        IEnumerable<string> references = refAssemblies
            .Select(this.GetAssemblyDirectory)
            .Where(d => !string.IsNullOrEmpty(d))
            .ToList()!;

        var metadataReferences = new List<MetadataReference>();

        foreach (var reference in references)
        {
            if (!libraryPaths.Add(reference))
            {
                continue;
            }

            using var stream = File.OpenRead(reference);
            var moduleMetadata = ModuleMetadata.CreateFromStream(stream, PEStreamOptions.PrefetchMetadata);
            var assemblyMetadata = AssemblyMetadata.Create(moduleMetadata);

            metadataReferences.Add(assemblyMetadata.GetReference(filePath: reference));
        }

        return metadataReferences;
    }

    private static IEnumerable<Assembly> GetReferencedAssemblies(
        Assembly a,
        ISet<string> excludedAssemblies,
        HashSet<string>? visitedAssemblies = null)
    {
        visitedAssemblies ??= new HashSet<string>();
        if (!visitedAssemblies.Add(a.GetName().EscapedCodeBase!))
        {
            yield break;
        }

        foreach (var assemblyRef in a.GetReferencedAssemblies())
        {
            if (visitedAssemblies.Contains(assemblyRef.EscapedCodeBase!))
            {
                continue;
            }

            if (excludedAssemblies.Any(s => s.Contains(assemblyRef.Name!)))
            {
                continue;
            }

            var loadedAssembly = Assembly.Load(assemblyRef);
            yield return loadedAssembly;
            foreach (var referenced in GetReferencedAssemblies(loadedAssembly, excludedAssemblies, visitedAssemblies))
            {
                yield return referenced;
            }
        }
    }

    private string? GetAssemblyDirectory(Assembly assembly)
    {
        if (assembly.IsDynamic)
        {
            return null;
        }

        var location = assembly.Location;
        var uri = new UriBuilder(location);
        return Uri.UnescapeDataString(uri.Path);
    }
}