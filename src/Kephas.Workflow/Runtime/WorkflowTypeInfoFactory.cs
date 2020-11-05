// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkflowTypeInfoFactory.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the workflow type information factory class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Workflow.Runtime
{
    using System;

    using Kephas.Runtime;

    /// <summary>
    /// A workflow type information factory.
    /// </summary>
    public class WorkflowTypeInfoFactory : IRuntimeTypeInfoFactory
    {
        private readonly IRuntimeTypeRegistry typeRegistry;

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowTypeInfoFactory"/> class.
        /// </summary>
        /// <param name="typeRegistry">The type registry.</param>
        public WorkflowTypeInfoFactory(IRuntimeTypeRegistry typeRegistry)
        {
            this.typeRegistry = typeRegistry;
        }

        /// <summary>
        /// Tries to create the runtime type information type for the provided raw type.
        /// </summary>
        /// <param name="type">The raw type.</param>
        /// <returns>
        /// The matching runtime type information type, or <c>null</c> if a runtime type info could not be created.
        /// </returns>
        public IRuntimeTypeInfo? TryCreateRuntimeTypeInfo(Type type)
        {
            if (typeof(IActivity).IsAssignableFrom(type) && typeof(IActivity) != type)
            {
                return new RuntimeActivityInfo(this.typeRegistry, type);
            }

            if (typeof(IStateMachine).IsAssignableFrom(type) && typeof(IStateMachine) != type)
            {
                return new RuntimeStateMachineInfo(this.typeRegistry, type);
            }

            return null;
        }
    }
}