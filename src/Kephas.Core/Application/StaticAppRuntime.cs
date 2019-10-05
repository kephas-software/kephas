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

    using Kephas.Dynamic;
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
        /// <param name="appLocation">Optional. The application location. If not specified, the current
        ///                           application location is considered.</param>
        /// <param name="appId">Optional. Identifier for the application.</param>
        /// <param name="appVersion">Optional. The application version.</param>
        /// <param name="appArgs">Optional. The application arguments.</param>
        public StaticAppRuntime(
            IAssemblyLoader assemblyLoader = null, 
            ILogManager logManager = null, 
            Func<AssemblyName, bool> defaultAssemblyFilter = null,
            string appLocation = null,
            string appId = null,
            string appVersion = null,
            IExpando appArgs = null)
            : base(assemblyLoader, logManager, defaultAssemblyFilter, appLocation, appId, appVersion, appArgs)
        {
        }
    }
}