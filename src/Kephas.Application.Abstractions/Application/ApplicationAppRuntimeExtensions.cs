// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationAppRuntimeExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application;

using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using Kephas.Application.Reflection;
using Kephas.Reflection;

/// <summary>
/// Extension methods for <see cref="IAppRuntime"/>.
/// </summary>
public static class ApplicationAppRuntimeExtensions
{
    private const string GetAppAssemblyFilterToken = $"__{nameof(GetAppAssemblyFilterToken)}";
    internal const string FeaturesToken = $"__{nameof(FeaturesToken)}";

    /// <summary>
    /// Gets the application features.
    /// </summary>
    /// <param name="appRuntime">The app runtime to act on.</param>
    /// <returns>
    /// The application features.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<IFeatureInfo> GetFeatures(this IAppRuntime appRuntime)
        => appRuntime?[FeaturesToken] as IEnumerable<IFeatureInfo> ?? Array.Empty<IFeatureInfo>();

    /// <summary>
    /// Indicates whether the application runtime contains the indicated feature.
    /// </summary>
    /// <remarks>
    /// The name comparison is case insensitive.
    /// </remarks>
    /// <param name="appRuntime">The application runtime to act on.</param>
    /// <param name="featureName">Name of the feature.</param>
    /// <returns>
    /// True if the application runtime contains the indicated feature, false otherwise.
    /// </returns>
    public static bool ContainsFeature(this IAppRuntime? appRuntime, string? featureName)
    {
        if (appRuntime == null || featureName == null)
        {
            return false;
        }

        return appRuntime.GetFeatures().Any(f => string.Equals(f.Name, featureName, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Sets the callback for filtering the assemblies.
    /// </summary>
    /// <typeparam name="T">The <see cref="IAppRuntime"/> type.</typeparam>
    /// <param name="appRuntime">The application runtime.</param>
    /// <param name="assemblyFilter">The assembly filter.</param>
    /// <returns>The provided application runtime.</returns>
    public static T OnGetAppAssembliesFilter<T>(this T appRuntime, Func<AssemblyName, bool> assemblyFilter)
        where T : IAppRuntime
    {
        appRuntime = appRuntime ?? throw new ArgumentNullException(nameof(appRuntime));
        assemblyFilter = assemblyFilter ?? throw new ArgumentNullException(nameof(assemblyFilter));

        appRuntime[GetAppAssemblyFilterToken] = assemblyFilter;

        return appRuntime;
    }

    /// <summary>
    /// Gets the callback for filtering the assemblies.
    /// </summary>
    /// <param name="appRuntime">The application runtime.</param>
    /// <returns>The assembly filter.</returns>
    public static Func<AssemblyName, bool> GetAssemblyFilter(this IAppRuntime appRuntime)
    {
        appRuntime = appRuntime ?? throw new ArgumentNullException(nameof(appRuntime));

        if (appRuntime[GetAppAssemblyFilterToken] is Func<AssemblyName, bool> assemblyFilter)
        {
            return assemblyFilter;
        }

        return a => !a.IsSystemAssembly();
    }

    /// <summary>
    /// Gets the application features.
    /// </summary>
    /// <typeparam name="T">The application runtime type.</typeparam>
    /// <param name="appRuntime">The app runtime to act on.</param>
    /// <param name="features">The features.</param>
    /// <returns>
    /// The application runtime.
    /// </returns>
    [return: NotNull]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static T SetFeatures<T>([DisallowNull] this T appRuntime, IEnumerable<IFeatureInfo> features)
        where T : IAppRuntime
    {
        appRuntime[FeaturesToken] = features;

        return appRuntime;
    }
}