// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IOAssemblyInitializer.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.IO;

using Kephas.Runtime;

/// <summary>
/// Assembly initializer for I/O.
/// </summary>
public class IOAssemblyInitializer : IAssemblyInitializer
{
    /// <summary>
    /// Initializes the assembly.
    /// </summary>
    public void Initialize()
    {
        IAmbientServices.AddCollector(ambient => ambient.Add<ILocationsManager, FolderLocationsManager>());
    }
}
