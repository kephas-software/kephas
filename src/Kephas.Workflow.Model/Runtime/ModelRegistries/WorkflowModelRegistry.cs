﻿// --------------------------------------------------------------------------------------------------------------------
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
        /// <param name="contextFactory">The context factory.</param>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="typeLoader">The type loader.</param>
        /// <param name="logManager">Optional. The log manager.</param>
        public WorkflowModelRegistry(
            IContextFactory contextFactory,
            IAmbientServices ambientServices,
            ITypeLoader typeLoader,
            ILogManager? logManager = null)
            : base(
                contextFactory,
                ambientServices,
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