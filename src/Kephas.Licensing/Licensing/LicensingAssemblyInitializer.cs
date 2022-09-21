// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LicensingAssemblyInitializer.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Licensing;

using Kephas.Application;

/// <summary>
/// Assembly initializer for Kephas.Licensing.
/// </summary>
public class LicensingAssemblyInitializer : IAssemblyInitializer
{
    /// <summary>
    /// Initializes the assembly.
    /// </summary>
    public void Initialize()
    {
        IAmbientServices.RegisterCollector(ambient => ambient.Register<ILicensingManager, NullLicensingManager>());
    }
}