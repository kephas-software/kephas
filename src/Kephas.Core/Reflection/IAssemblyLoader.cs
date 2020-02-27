// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAssemblyLoader.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IAssemblyLoader interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#nullable enable

namespace Kephas.Reflection
{
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// Interface for loading assemblies.
    /// </summary>
    public interface IAssemblyLoader
    {
        /// <summary>
        /// Attempts to load an assembly from its given assembly name.
        /// </summary>
        /// <param name="assemblyName">The name of the assembly to be loaded.</param>
        /// <returns>
        /// The resolved assembly reference.
        /// </returns>
        Assembly LoadAssemblyFromName(AssemblyName assemblyName);

        /// <summary>
        /// Attempts to load an assembly.
        /// </summary>
        /// <param name="assemblyFilePath">The file path of the assembly to be loaded.</param>
        /// <returns>
        /// The resolved assembly reference.
        /// </returns>
        Assembly LoadAssemblyFromPath(string assemblyFilePath);

        /// <summary>
        /// Gets the loaded assemblies.
        /// </summary>
        /// <returns>
        /// The loaded assemblies.
        /// </returns>
        IEnumerable<Assembly> GetAssemblies();
    }
}