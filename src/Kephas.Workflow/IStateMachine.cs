// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IStateMachine.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IStateMachine interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#nullable enable

namespace Kephas.Workflow
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Workflow.Reflection;

    /// <summary>
    /// A state machine controls a target, transitioning it through multiple states.
    /// </summary>
    /// <remarks>
    /// A state machine instance may transition the state of the target multiple times.
    /// </remarks>
    public interface IStateMachine : IInstance<IStateMachineInfo>
    {
        /// <summary>
        /// Gets the target controlled by the state machine.
        /// </summary>
        /// <remarks>
        /// The target is typically the state machine's container instance.
        /// For example, a document entity may contain state machine controlling the document state,
        /// in which case the target is the document.
        /// </remarks>
        /// <value>
        /// The target.
        /// </value>
        object Target { get; }

        /// <summary>
        /// Gets the current state of the target.
        /// </summary>
        /// <value>
        /// The current state of the target.
        /// </value>
        object CurrentState { get; }

        /// <summary>
        /// Transitions the state machine asynchronously using the information in the provided context.
        /// </summary>
        /// <param name="context">The transition  context.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the transition result.
        /// </returns>
        Task<object?> TransitionAsync(ITransitionContext context, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// A state machine controls a target, transitioning it through multiple states.
    /// </summary>
    /// <remarks>
    /// A state machine instance may transition the state of the target multiple times.
    /// </remarks>
    /// <typeparam name="TTarget">Type of the target.</typeparam>
    /// <typeparam name="TState">Type of the state.</typeparam>
    public interface IStateMachine<TTarget, TState> : IStateMachine
        where TTarget : class
    {
        /// <summary>
        /// Gets the target controlled by the state machine.
        /// </summary>
        /// <remarks>
        /// The target is typically the state machine's container instance.
        /// For example, a document entity may contain state machine controlling the document state,
        /// in which case the target is the document.
        /// </remarks>
        /// <value>
        /// The target.
        /// </value>
        new TTarget Target { get; }

        /// <summary>
        /// Gets the current state of the target.
        /// </summary>
        /// <value>
        /// The current state of the target.
        /// </value>
        new TState CurrentState { get; }
    }
}
