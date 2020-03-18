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
        /// <summary>
        /// Tries to create the runtime type information type for the provided raw type.
        /// </summary>
        /// <param name="type">The raw type.</param>
        /// <returns>
        /// The matching runtime type information type, or <c>null</c> if a runtime type info could not be created.
        /// </returns>
        public IRuntimeTypeInfo TryCreateRuntimeTypeInfo(Type type)
        {
            if (typeof(IActivity).IsAssignableFrom(type))
            {
                return new RuntimeActivityInfo(type);
            }

            if (typeof(IStateMachine).IsAssignableFrom(type))
            {
                return new RuntimeStateMachineInfo(type);
            }

            return null;
        }
    }
}