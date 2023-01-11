// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoggingAssemblyInitializer.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services;

using Kephas.Logging;
using Kephas.Runtime;

/// <summary>
/// Assembly initializer for Kephas.Services.
/// </summary>
public class LoggingAssemblyInitializer : IAssemblyInitializer
{
    /// <summary>
    /// Initializes the assembly.
    /// </summary>
    public void Initialize()
    {
        IAppServiceCollection.AddCollector(ambient => ambient.Add<ILogManager, NullLogManager>());
    }
}