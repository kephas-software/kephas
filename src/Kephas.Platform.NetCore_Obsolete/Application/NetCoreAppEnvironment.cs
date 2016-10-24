// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NetCoreAppEnvironment.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   The hosting environment for .NET Core applications.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Reflection;

    /// <summary>
    /// The application environment for .NET Core applications.
    /// </summary>
    public class NetCoreAppEnvironment : AppEnvironmentBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NetCoreAppEnvironment" /> class.
        /// </summary>
        /// <param name="assemblyLoader">The assembly loader.</param>
        /// <param name="assemblyFilter">A filter for loaded assemblies.</param>
        public NetCoreAppEnvironment(IAssemblyLoader assemblyLoader = null, Func<AssemblyName, bool> assemblyFilter = null)
            : base(assemblyLoader, assemblyFilter)
        {
        }

        /// <summary>
        /// Adds additional assemblies to the ones already collected.
        /// </summary>
        /// <param name="assemblies">The collected assemblies.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        protected override Task AddAdditionalAssembliesAsync(IList<Assembly> assemblies, CancellationToken cancellationToken)
        {
            // TODO add additional assemblies from the app folder.
            return base.AddAdditionalAssembliesAsync(assemblies, cancellationToken);
        }
    }
}