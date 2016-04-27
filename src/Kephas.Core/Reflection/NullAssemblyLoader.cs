// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NullAssemblyLoader.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the null assembly loader class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Reflection
{
    using System;
    using System.Reflection;

    /// <summary>
    /// A null assembly loader.
    /// </summary>
    public class NullAssemblyLoader : IAssemblyLoader
    {
        /// <summary>
        /// Attempts to load an assembly.
        /// </summary>
        /// <param name="assemblyName">The name of the assembly to be loaded.</param>
        /// <returns>
        /// The resolved assembly reference.
        /// </returns>
        public Assembly LoadAssembly(AssemblyName assemblyName)
        {
            throw new NotSupportedException($"The {nameof(NullAssemblyLoader)} cannot resolve the assembly '{assemblyName}'.");
        }
    }
}