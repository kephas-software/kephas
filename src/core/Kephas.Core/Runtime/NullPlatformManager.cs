// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NullPlatformManager.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   A platform manager doing nothing.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Runtime
{
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading.Tasks;

    using Kephas.Services;

    /// <summary>
    /// A platform manager doing nothing.
    /// </summary>
    [OverridePriority(Priority.Lowest)]
    public class NullPlatformManager : IPlatformManager
    {
        /// <summary>
        /// Gets the application assemblies.
        /// </summary>
        /// <returns>
        /// An enumeration of application assemblies.
        /// </returns>
        public Task<IEnumerable<Assembly>> GetAppAssembliesAsync()
        {
            return Task.FromResult((IEnumerable<Assembly>)new Assembly[0]);
        }
    }
}