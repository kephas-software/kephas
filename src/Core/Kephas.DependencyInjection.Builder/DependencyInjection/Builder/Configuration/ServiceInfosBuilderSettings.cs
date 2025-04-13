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
public record ServiceInfosBuilderSettings
{
    /// <summary>Function to retrieve application assemblies.</summary>
    public required Func<IEnumerable<Assembly>> GetAppAssemblies { get; set; }

    /// <summary>Indicates whether the internal implementations should be considered or not.</summary>
    public bool IncludeInternal { get; set; } = true;

    /// <summary>The <see cref="IServiceInfoCollection"/> to be built.</summary>
    public IServiceInfoCollection? Services { get; set; }
    
    /// <summary>
    /// Callback to configure the builder.
    /// </summary>
    public Action<ServiceInfoCollectionBuilder>? ConfigureBuilder { get; set; }
}