﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReflectionAssemblyInitializer.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Reflection;

using Kephas.Application;
using Kephas.Runtime;

/// <summary>
/// Assembly initializer for Kephas.Reflection.
/// </summary>
public class ReflectionAssemblyInitializer : IAssemblyInitializer
{
    /// <summary>
    /// Initializes the assembly.
    /// </summary>
    public void Initialize()
    {
        IAppServiceCollection.AddAppServicesCollector(ambient => ambient.TryAdd<ITypeLoader, DefaultTypeLoader>());
        IAppServiceCollection.AddAppServicesCollector(ambient => ambient.TryAdd<IRuntimeTypeRegistry>(RuntimeTypeRegistry.Instance, b => b.ExternallyOwned()));
    }
}