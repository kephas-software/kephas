// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PermissionsModelRegistry.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Security.Permissions.Runtime.ModelRegistries
{
    using Kephas.Application;
    using Kephas.Logging;
    using Kephas.Model.Runtime;
    using Kephas.Reflection;
    using Kephas.Security.Permissions.AttributedModel;
    using Kephas.Services;

    /// <summary>
    /// Model serviceRegistry for permissions.
    /// </summary>
    public class PermissionsModelRegistry : ConventionsRuntimeModelRegistryBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PermissionsModelRegistry"/> class.
        /// </summary>
        /// <param name="contextFactory">The context factory.</param>
        /// <param name="appRuntime">The application runtime.</param>
        /// <param name="typeLoader">Optional. The type loader.</param>
        /// <param name="logManager">Optional. The log manager.</param>
        public PermissionsModelRegistry(
            IContextFactory contextFactory,
            IAppRuntime appRuntime,
            ITypeLoader? typeLoader = null,
            ILogManager? logManager = null)
            : base(
                contextFactory,
                appRuntime,
                typeLoader,
                context =>
                {
                    context.IncludeClasses = true;
                    context.IncludeAbstractClasses = true;
                    context.IncludeInterfaces = true;
                    context.ExcludeMarkers = true;
                    context.MarkerAttributeTypes = new[] { typeof(IPermissionInfoAnnotation) };
                },
                logManager)
        {
        }
    }
}