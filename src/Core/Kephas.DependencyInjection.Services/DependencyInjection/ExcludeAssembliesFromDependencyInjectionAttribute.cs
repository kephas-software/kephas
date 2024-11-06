// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExcludeAssembliesFromDependencyInjectionAttribute.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Reflection;
using System.Text.RegularExpressions;

namespace Kephas.DependencyInjection;

/// <summary>
/// Marks assemblies to be excluded from dependency injection type collection
/// when conforming to a specific name pattern.
/// </summary>
/// <param name="namePattern">A regular expression matching the assembly names to be excluded.</param>
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
public sealed class ExcludeAssembliesFromDependencyInjectionAttribute(string namePattern) : Attribute
{
    private readonly Regex nameRegex = new(namePattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);

    /// <summary>
    /// Gets a value indicating whether the assembly is excluded from dependency injection.
    /// </summary>
    /// <param name="assembly">The assembly to check.</param>
    /// <returns><c>true</c> if the assembly should be excluded from dependency injection, <c>false</c> otherwise.</returns>
    public bool IsExcluded(Assembly assembly) => nameRegex.IsMatch(assembly.GetName()?.Name ?? string.Empty);

    /// <summary>
    /// Gets a value indicating whether the assembly is excluded from dependency injection.
    /// </summary>
    /// <param name="assemblyName">The name of the assembly to check.</param>
    /// <returns><c>true</c> if the assembly with the provided name should be excluded from dependency injection, <c>false</c> otherwise.</returns>
    public bool IsExcluded(string assemblyName) => nameRegex.IsMatch(assemblyName ?? string.Empty);
}