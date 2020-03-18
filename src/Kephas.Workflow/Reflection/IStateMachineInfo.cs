// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IStateMachineInfo.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IStateMachineInfo interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#nullable enable

namespace Kephas.Workflow.Reflection
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Dynamic;
    using Kephas.Operations;
    using Kephas.Reflection;

    /// <summary>
    /// Contract interface for state machine metadata.
    /// </summary>
    public interface IStateMachineInfo : ITypeInfo
    {
        /// <summary>
        /// Gets the type of the state. Typically it is an enumeration.
        /// </summary>
        /// <value>
        /// The type of the state.
        /// </value>
        ITypeInfo StateType { get; }

        /// <summary>
        /// Gets the type of the target controlled by the state machine.
        /// </summary>
        /// <value>
        /// The type of the target.
        /// </value>
        ITypeInfo TargetType { get; }

        /// <summary>
        /// Gets the state property of the target storing the current state.
        /// </summary>
        /// <value>
        /// The state property of the target storing the current state.
        /// </value>
        IPropertyInfo TargetStateProperty { get; }

        /// <summary>
        /// Gets the activities transitioning the target through different states.
        /// </summary>
        /// <value>
        /// The transition activities.
        /// </value>
        IEnumerable<IActivityInfo> Transitions { get; }

        /// <summary>
        /// Executes the transition asynchronously.
        /// </summary>
        /// <param name="transition">The transition to execute.</param>
        /// <param name="target">The state machine target.</param>
        /// <param name="arguments">The execution arguments.</param>
        /// <param name="context">The execution context.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result that yields the output.
        /// </returns>
        Task<object> ExecuteAsync(
            IActivity transition,
            object? target,
            IExpando? arguments,
            IActivityContext context,
            CancellationToken cancellationToken = default);
    }
}
