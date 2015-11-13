// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NullHostingEnvironment.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   A platform manager doing nothing.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Hosting
{
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Services;

    /// <summary>
    /// A platform manager doing nothing.
    /// </summary>
    [OverridePriority(Priority.Lowest)]
    public class NullHostingEnvironment : IHostingEnvironment
    {
        /// <summary>
        /// Gets the application assemblies.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A promise of an enumeration of application assemblies.
        /// </returns>
        public Task<IEnumerable<Assembly>> GetAppAssembliesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult((IEnumerable<Assembly>)new Assembly[0]);
        }
    }
}