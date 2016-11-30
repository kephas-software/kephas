// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAssemblyLoader.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the IAssemblyLoader interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Reflection
{
    using System.Reflection;

    using Kephas.Services;

    /// <summary>
    /// Interface for loading assemblies.
    /// </summary>
    [SharedAppServiceContract]
    public interface IAssemblyLoader
    {
        /// <summary>
        /// Attempts to load an assembly.
        /// </summary>
        /// <param name="assemblyName">The name of the assembly to be loaded.</param>
        /// <returns>
        /// The resolved assembly reference.
        /// </returns>
        Assembly LoadAssembly(AssemblyName assemblyName);
    }
}