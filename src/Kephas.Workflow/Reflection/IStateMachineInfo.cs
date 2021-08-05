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
        /// Gets the operations transitioning the target through different states.
        /// </summary>
        /// <value>
        /// The transition operations.
        /// </value>
        IEnumerable<ITransitionInfo> Transitions { get; }

        /// <summary>
        /// Transitions the state machine asynchronously.
        /// </summary>
        /// <param name="stateMachine">The state machine.</param>
        /// <param name="targetState">State of the target.</param>
        /// <param name="transitionInfo">Information describing the transition.</param>
        /// <param name="arguments">The execution arguments.</param>
        /// <param name="context">The execution context.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result that yields the transition result.
        /// </returns>
        Task<object?> TransitionAsync(
            IStateMachine stateMachine,
            object targetState,
            ITransitionInfo? transitionInfo,
            IDynamic? arguments,
            ITransitionContext context,
            CancellationToken cancellationToken = default);
    }
}
