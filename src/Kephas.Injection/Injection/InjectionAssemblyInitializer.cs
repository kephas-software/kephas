// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InjectionAssemblyInitializer.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Injection;

using Kephas.Application;
using Kephas.IO;
using Kephas.Logging;

/// <summary>
/// Assembly initializer for Kephas.Injection.
/// </summary>
public class InjectionAssemblyInitializer : IAssemblyInitializer
{
    /// <summary>
    /// Initializes the assembly.
    /// </summary>
    public void Initialize()
    {
        IAmbientServices.RegisterInitializer(ambient => ambient.Register<ILogManager, NullLogManager>());
        IAmbientServices.RegisterInitializer(ambient => ambient.Register<ILocationsManager, FolderLocationsManager>());
    }
}