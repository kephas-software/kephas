﻿// --------------------------------------------------------------------------------------------------------------------
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
    using System.Collections.Generic;
    using System.Reflection;

    using Kephas.Dynamic;
    using Kephas.Licensing;
    using Kephas.Logging;
    using Kephas.Services;

    /// <summary>
    /// An application application runtime providing only assemblies loaded by the runtime.
    /// </summary>
    public class StaticAppRuntime : AppRuntimeBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StaticAppRuntime"/> class.
        /// </summary>
        /// <param name="getLogger">Optional. The get logger delegate.</param>
        /// <param name="checkLicense">Optional. The check license delegate.</param>
        /// <param name="defaultAssemblyFilter">Optional. A default filter applied when loading
        ///                                     assemblies.</param>
        /// <param name="appFolder">Optional. The application location. If not specified, the current
        ///                           application location is considered.</param>
        /// <param name="configFolders">Optional. The configuration folders.</param>
        /// <param name="licenseFolders">Optional. The license folders.</param>
        /// <param name="isRoot">Optional. Indicates whether the application instance is the root.</param>
        /// <param name="appId">Optional. Identifier for the application.</param>
        /// <param name="appInstanceId">Optional. Identifier for the application instance.</param>
        /// <param name="appVersion">Optional. The application version.</param>
        /// <param name="appArgs">Optional. The application arguments.</param>
        public StaticAppRuntime(
            Func<string, ILogger>? getLogger = null,
            Func<AppIdentity, IContext?, ILicenseCheckResult>? checkLicense = null,
            Func<AssemblyName, bool>? defaultAssemblyFilter = null,
            string? appFolder = null,
            IEnumerable<string>? configFolders = null,
            IEnumerable<string>? licenseFolders = null,
            bool? isRoot = null,
            string? appId = null,
            string? appInstanceId = null,
            string? appVersion = null,
            IDynamic? appArgs = null)
            : base(getLogger, checkLicense, defaultAssemblyFilter, appFolder, configFolders, licenseFolders, isRoot, appId, appInstanceId, appVersion, appArgs)
        {
        }
    }
}