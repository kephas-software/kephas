// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StaticAppRuntime.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the static application runtime base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application
{
    using System;
    using System.Reflection;
    using Kephas.Logging;
    using Kephas.Reflection;

    /// <summary>
    /// An application application runtime providing only assemblies loaded by the runtime.
    /// </summary>
    public class StaticAppRuntime : AppRuntimeBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StaticAppRuntime"/> class.
        /// </summary>
        /// <param name="assemblyLoader">Optional. The assembly loader.</param>
        /// <param name="logManager">Optional. The log manager.</param>
        /// <param name="defaultAssemblyFilter">Optional. A default filter applied when loading
        ///                                     assemblies.</param>
        /// <param name="appLocation">Optional. The application location. If not specified, the
        ///                           current application location is considered.</param>
        public StaticAppRuntime(IAssemblyLoader assemblyLoader = null, ILogManager logManager = null, Func<AssemblyName, bool> defaultAssemblyFilter = null, string appLocation = null)
            : base(assemblyLoader, logManager, defaultAssemblyFilter, appLocation)
        {
        }
    }
}