// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimeHelper.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Reflection;
using System.Runtime.Loader;
using Kephas.Collections;
using Kephas.Logging;
using Kephas.Resources;

namespace Kephas;

/// <summary>
/// Helper class for the runtime.
/// </summary>
public static class RuntimeHelper
{
    /// <summary>
    /// Gets all the assemblies, expanding the references, starting from the provided root assemblies and filtered by name as indicated.
    /// </summary>
    /// <param name="rootAssemblies">The root assemblies.</param>
    /// <param name="nameFilter">Optional. A filter for the assembly name.</param>
    /// <param name="logger">Optional. A logger to log errors.</param>
    /// <returns>
    /// An enumeration of assemblies.
    /// </returns>
    public static IEnumerable<Assembly> Flatten(this IEnumerable<Assembly> rootAssemblies, Func<AssemblyName, bool>? nameFilter = null, ILogger? logger = null)
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
                .Select(an => TryLoadAssembly(an, logger)!)
                .Where(assembly => assembly is not null)
                .ToList();

            assemblies.AddRange(assembliesToCheck);
        }

        return assemblies;
    }

    /// <summary>
    /// Gets the loaded assemblies.
    /// </summary>
    /// <returns>An enumeration of assemblies.</returns>
    public static IEnumerable<Assembly> GetLoadedAssemblies()
    {
#if NETSTANDARD2_1
        return AppDomain.CurrentDomain.GetAssemblies();
#else
        return AssemblyLoadContext.Default.Assemblies;
#endif
    }

    private static Assembly? TryLoadAssembly(AssemblyName n, ILogger? logger)
    {
        try
        {
            return LoadAssemblyFromName(n);
        }
        catch (Exception ex)
        {
            logger.Warn(ex, AbstractionStrings.AppRuntimeBase_CannotLoadAssembly_Exception, n);
            return null;
        }
    }

    private static Assembly LoadAssemblyFromName(AssemblyName assemblyName)
    {
        return AssemblyLoadContext.Default.LoadFromAssemblyName(assemblyName);
    }
}