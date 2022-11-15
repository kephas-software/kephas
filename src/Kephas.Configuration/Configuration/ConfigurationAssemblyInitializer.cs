// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigurationAssemblyInitializer.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Configuration;

using Kephas.Application;

/// <summary>
/// Assembly initializer for configuration.
/// </summary>
public class ConfigurationAssemblyInitializer : IAssemblyInitializer
{
    /// <summary>
    /// Initializes the assembly.
    /// </summary>
    public void Initialize()
    {
        IAmbientServices.AddCollector(ambient => ambient.Add<IConfigurationStore, DefaultConfigurationStore>());
    }
}