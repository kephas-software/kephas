// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkflowModelRegistry.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the messaging model registry class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Workflow.Model.Runtime.ModelRegistries
{
    using Kephas.Application;
    using Kephas.Logging;
    using Kephas.Model.Runtime;
    using Kephas.Reflection;
    using Kephas.Services;

    /// <summary>
    /// A workflow model registry.
    /// </summary>
    public class WorkflowModelRegistry : ConventionsRuntimeModelRegistryBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowModelRegistry"/> class.
        /// </summary>
        /// <param name="injectableFactory">The injectable factory.</param>
        /// <param name="appRuntime">The application runtime.</param>
        /// <param name="typeLoader">The type loader.</param>
        /// <param name="logManager">Optional. The log manager.</param>
        public WorkflowModelRegistry(
            IInjectableFactory injectableFactory,
            IAppRuntime appRuntime,
            ITypeLoader typeLoader,
            ILogManager? logManager = null)
            : base(
                injectableFactory,
                appRuntime,
                typeLoader,
                context =>
                {
                    context.IncludeClasses = true;
                    context.IncludeAbstractClasses = false;
                    context.MarkerBaseTypes = new[] { typeof(IActivity), typeof(IStateMachine) };
                },
                logManager)
        {
        }
    }
}