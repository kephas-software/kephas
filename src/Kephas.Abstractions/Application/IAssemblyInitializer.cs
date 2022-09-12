// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAssemblyInitializer.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application;

#if NET6_0_OR_GREATER

/// <summary>
/// Interface for components doing assembly initialization.
/// Initializers implementing this interface must have a parameterless constructor.
/// </summary>
public interface IAssemblyInitializer
{
    /// <summary>
    /// Initializes the assembly.
    /// </summary>
    void Initialize();
}

#endif