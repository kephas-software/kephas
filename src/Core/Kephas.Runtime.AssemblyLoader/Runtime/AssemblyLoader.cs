// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssemblyLoader.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Reflection;
using System.Runtime.Loader;
using Kephas.Collections;
using Kephas.Resources;
using Microsoft.Extensions.Logging;

namespace Kephas.Runtime;

/// <summary>
/// The default assembly loader.
/// </summary>
public class AssemblyLoader : IAssemblyLoader
{
    private readonly ILogger? logger;

    /// <summary>
    /// Gets the static instance of this <see cref="AssemblyLoader"/>.
    /// </summary>
    public static readonly IAssemblyLoader Instance = new AssemblyLoader();

    /// <summary>
    /// Initializes a new instance of the <see cref="AssemblyLoader"/> class.
    /// </summary>
    /// <param name="logger">Optional. The logger.</param>
    public AssemblyLoader(ILogger? logger = null)
    {
        this.logger = logger;
    }

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
    public IEnumerable<Assembly> LoadAssemblies(
        Func<AssemblyName, bool>? assemblyFilter = null,
        IEnumerable<string>? binLocations = null)
    {
        var loadedAssemblies = ((IAssemblyLoader)this).GetLoadedAssemblies().ToList();
        return FlattenReferences(loadedAssemblies, assemblyFilter);
    }

    /// <summary>
    /// Gets all the assemblies, expanding the references, starting from the provided root assemblies and filtered by name as indicated.
    /// </summary>
    /// <param name="rootAssemblies">The root assemblies.</param>
    /// <param name="nameFilter">Optional. A filter for the assembly name.</param>
    /// <returns>
    /// An enumeration of assemblies.
    /// </returns>
    protected internal IEnumerable<Assembly> FlattenReferences(IEnumerable<Assembly> rootAssemblies, Func<AssemblyName, bool>? nameFilter = null)
    {
        nameFilter ??= _ => true;

        // when computing the assemblies, use the Name and not the FullName
        // because for some obscure reasons it is possible to have the same
        // assembly with different versions loaded.
        // TODO log when such cases occur.
        var assemblies = rootAssemblies.Where(a => nameFilter(a.GetName())).ToList();
        var loadedAssemblyRefs =
            new HashSet<string>(rootAssemblies.Select(a => a.GetName().Name).Where(n => n is not null)!);
        var assembliesToCheck = new List<Assembly>(assemblies);

        while (assembliesToCheck.Count > 0)
        {
            var assemblyRefsToLoad = new HashSet<AssemblyName>();
            foreach (var referencesToLoad in assembliesToCheck
                         .Select(assembly => assembly.GetReferencedAssemblies()
                             .Where(a => (a.Name is null || !loadedAssemblyRefs.Contains(a.Name)) && nameFilter(a))
                             .ToList()))
            {
                loadedAssemblyRefs.AddRange(referencesToLoad.Select(an => an.Name!).Where(n => n is not null));
                assemblyRefsToLoad.AddRange(referencesToLoad);
            }

            assembliesToCheck = assemblyRefsToLoad
                .Select(an => TryLoadAssembly(an)!)
                .Where(assembly => assembly is not null)
                .ToList();

            assemblies.AddRange(assembliesToCheck);
        }

        return assemblies;
    }

    private Assembly? TryLoadAssembly(AssemblyName n)
    {
        try
        {
            return LoadAssemblyFromName(n);
        }
        catch (Exception ex)
        {
            logger?.LogWarning(ex, AssemblyLoaderStrings.AppRuntimeBase_CannotLoadAssembly_Exception, n);
            return null;
        }
    }

    private static Assembly LoadAssemblyFromName(AssemblyName assemblyName)
    {
        return AssemblyLoadContext.Default.LoadFromAssemblyName(assemblyName);
    }
}
