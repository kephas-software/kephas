// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InjectionTypeExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas;

using Kephas.Services;

/// <summary>
/// Extension methods for Type relative to injection.
/// </summary>
public static class InjectionTypeExtensions
{
    /// <summary>
    /// Gets a value indicating whether the provided type is injectable.
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <returns><c>true</c> if the type is injectable, <c>false</c> otherwise.</returns>
    public static bool IsInjectable(this Type type)
        => typeof(IInjectable).IsAssignableFrom(type);
}