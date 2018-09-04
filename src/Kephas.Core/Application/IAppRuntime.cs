// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAppRuntime.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Interface for abstracting away the runtime for the application.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Reflection;

    using Kephas.Dynamic;

    /// <summary>
    /// Interface for abstracting away the runtime for the application.
    /// </summary>
    public interface IAppRuntime : IExpando
    {
        /// <summary>
        /// Gets the application location (directory where the application lies).
        /// </summary>
        /// <returns>
        /// A path indicating the application location.
        /// </returns>
        string GetAppLocation();

        /// <summary>
        /// Gets the application assemblies.
        /// </summary>
        /// <param name="assemblyFilter">A filter for the assemblies (optional).</param>
        /// <returns>
        /// An enumeration of application assemblies.
        /// </returns>
        IEnumerable<Assembly> GetAppAssemblies(Func<AssemblyName, bool> assemblyFilter = null);

        /// <summary>
        /// Gets host address.
        /// </summary>
        /// <returns>
        /// The host address.
        /// </returns>
        IPAddress GetHostAddress();

        /// <summary>
        /// Gets host name.
        /// </summary>
        /// <returns>
        /// The host name.
        /// </returns>
        string GetHostName();
    }
}