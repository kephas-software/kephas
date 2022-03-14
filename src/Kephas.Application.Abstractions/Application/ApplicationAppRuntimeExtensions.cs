// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationAppRuntimeExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application;

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Kephas.Application.Reflection;

/// <summary>
/// Extension methods for <see cref="IAppRuntime"/>.
/// </summary>
public static class ApplicationAppRuntimeExtensions
{
    internal const string FeaturesKey = "Features";

    /// <summary>
    /// Gets the application features.
    /// </summary>
    /// <param name="appRuntime">The app runtime to act on.</param>
    /// <returns>
    /// The application features.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<IFeatureInfo> GetFeatures(this IAppRuntime appRuntime)
        => appRuntime?[FeaturesKey] as IEnumerable<IFeatureInfo> ?? Array.Empty<IFeatureInfo>();

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
        appRuntime[FeaturesKey] = features;

        return appRuntime;
    }
}