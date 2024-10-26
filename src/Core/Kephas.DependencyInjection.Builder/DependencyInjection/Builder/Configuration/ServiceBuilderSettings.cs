// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceBuilderSettings.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Reflection;
using Kephas.DependencyInjection.Reflection;

namespace Kephas.DependencyInjection.Builder.Configuration;

/// <summary>
/// Settings for the <see cref="ServiceInfoCollectionBuilder"/>.
/// </summary>
/// <param name="GetAppAssemblies">Function to retrieve application assemblies.</param>
/// <param name="IncludeInternal">Indicates whether the internal implementations should be considered or not.</param>
/// <param name="Services">The <see cref="IServiceInfoCollection"/> to be built.</param>
public record ServiceBuilderSettings(
    Func<IEnumerable<Assembly>> GetAppAssemblies,
    bool IncludeInternal = true,
    IServiceInfoCollection? Services = null);