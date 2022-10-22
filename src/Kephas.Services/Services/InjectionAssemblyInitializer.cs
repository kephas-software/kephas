// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InjectionAssemblyInitializer.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services;

using Kephas.Application;
using Kephas.IO;
using Kephas.Logging;

/// <summary>
/// Assembly initializer for Kephas.Services.
/// </summary>
public class InjectionAssemblyInitializer : IAssemblyInitializer
{
    /// <summary>
    /// Initializes the assembly.
    /// </summary>
    public void Initialize()
    {
        IAmbientServices.AddCollector(ambient => ambient.Add<ILogManager, NullLogManager>());
        IAmbientServices.AddCollector(ambient => ambient.Add<ILocationsManager, FolderLocationsManager>());
    }
}