// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEntityContextSettingsProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.DependencyInjection;

namespace Kephas.Entities.Configuration;

/// <summary>
/// Provides <see cref="EntityContextSettings"/> by name.
/// </summary>
[SingletonService]
public interface IEntityContextSettingsProvider
{
    /// <summary>
    /// Gets the settings for the <see cref="IEntityContext"/> with the provided name.
    /// </summary>
    /// <param name="entityContextName">The name of the <see cref="IEntityContext"/>.</param>
    /// <returns>The <see cref="EntityContextSettings"/>.</returns>
    EntityContextSettings GetEntityContextSettings(string entityContextName);
}
