// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppRuntime.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Reflection;
using System.Text.RegularExpressions;
using Kephas.Application;
using Kephas.Runtime.Configuration;

namespace Kephas.Runtime;

/// <summary>
/// The default implementation of <see cref="IAppRuntime"/>.
/// </summary>
public class AppRuntime : IAppRuntime
{
    private const string AssemblyExtension = ".dll";
    private Regex? assemblyNameRegex;

    private readonly AppRuntimeSettings settings;
    private string? environment;
    private string? appName;
    private string? appLocation;
    private Assembly? entryAssembly;

    /// <summary>
    /// Initializes a new instance of the <see cref="AppRuntime"/> class
    /// </summary>
    public AppRuntime()
        : this(new AppRuntimeSettings())
    {
    }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="AppRuntime"/> class
    /// </summary>
    public AppRuntime(AppRuntimeSettings settings) =>
        this.settings = settings ?? throw new ArgumentNullException(nameof(settings));

    /// <summary>
    /// Gets the application's entry assembly.
    /// </summary>
    public Assembly? GetEntryAssembly() => entryAssembly ??= (settings.EntryAssembly ?? Assembly.GetEntryAssembly());
    
    /// <summary>
    /// Gets the root path of the application.
    /// </summary>
    /// <returns>The root path of the application.</returns>
    public string? GetAppLocation() => appLocation ??= GetEntryAssembly()?.GetLocationDirectory();

    /// <summary>
    /// Gets the name identifying the application.
    /// </summary>
    /// <returns>
    /// The name of the application.
    /// </returns>
    public string? GetAppName() => appName ??= settings.AppName ?? IAppRuntime.GetAppNameFromAssembly(GetEntryAssembly());

    /// <summary>
    /// Gets the running environment.
    /// </summary>
    /// <returns>The running environment.</returns>
    public string? GetEnvironment() => environment ??= (settings.Environment ?? IAppRuntime.GetEnvironmentFromEnvironmentVariable(GetAppName()));

    /// <summary>
    /// Gets the application assemblies.
    /// </summary>
    /// <returns>
    /// An enumeration of application assemblies.
    /// </returns>
    public IEnumerable<Assembly> GetAppAssemblies()
    {
        var completed = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);

        var mainAssembly = GetEntryAssembly() ?? throw new InvalidOperationException("The entry assembly is not set.");
        var isMatch = GetMatchPredicate(completed);
        if (isMatch(mainAssembly.GetName()))
        {
            yield return mainAssembly;
        }

        completed.Add(mainAssembly.GetName().Name!);

        var pending = new HashSet<AssemblyName>(mainAssembly.GetReferencedAssemblies().Where(isMatch));
        foreach (var assemblyName in GetLazyReferencedAssemblies(mainAssembly).Where(isMatch))
        {
            pending.Add(assemblyName);
        }

        while (pending.Count > 0)
        {
            var assemblyName = pending.First();
            pending.Remove(assemblyName);

            if (completed.Contains(assemblyName.Name!))
            {
                continue;
            }
            
            var assembly = Assembly.Load(assemblyName);
            completed.Add(assemblyName.Name!);
            yield return assembly;

            foreach (var refAssemblyName in assembly.GetReferencedAssemblies().Where(isMatch))
            {
                pending.Add(refAssemblyName);
            }
        }
    }

    private Func<AssemblyName, bool> GetMatchPredicate(ICollection<string> completed)
    {
        var nameRegex = GetAssemblyNameRegex();
        return an =>
            an.Name is not null && nameRegex.IsMatch(an.Name) && !completed.Contains(an.Name);
    }

    private IEnumerable<AssemblyName> GetLazyReferencedAssemblies(Assembly assembly)
    {
        return Directory.EnumerateFiles(assembly.GetLocationDirectory()!)
            .Where(fileName => fileName.EndsWith(AssemblyExtension, StringComparison.InvariantCultureIgnoreCase))
            .Select(Path.GetFileName)
            .Select(fileName => new AssemblyName(fileName![..^AssemblyExtension.Length]));
    }

    private Regex GetAssemblyNameRegex() => assemblyNameRegex ??=
        new Regex(settings.AppAssemblyNamePattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
}