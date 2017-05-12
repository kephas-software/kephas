// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NullAppRuntime.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the null application runtime class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application
{
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// The <c>null</c> application runtime.
    /// </summary>
    public class NullAppRuntime : AppRuntimeBase
    {
        /// <summary>
        /// Gets the application location.
        /// </summary>
        /// <returns>
        /// A path indicating the application location.
        /// </returns>
        public override string GetAppLocation()
        {
            return string.Empty;
        }

        /// <summary>
        /// Gets the loaded assemblies.
        /// </summary>
        /// <returns>
        /// The loaded assemblies.
        /// </returns>
        protected override IList<Assembly> GetLoadedAssemblies()
        {
            return new List<Assembly> { this.GetType().GetTypeInfo().Assembly };
        }

        /// <summary>
        /// Gets the referenced assemblies.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns>
        /// An array of assembly name.
        /// </returns>
        protected override AssemblyName[] GetReferencedAssemblies(Assembly assembly)
        {
            return new AssemblyName[0];
        }

        /// <summary>
        /// Gets the file name of the provided assembly.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns>
        /// The assembly file name.
        /// </returns>
        protected override string GetFileName(Assembly assembly)
        {
            return string.Empty;
        }
    }
}