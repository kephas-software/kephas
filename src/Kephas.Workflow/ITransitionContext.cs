// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITransitionContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the ITransitionContext interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#nullable enable

namespace Kephas.Workflow
{
    using Kephas.Dynamic;
    using Kephas.Services;
    using Kephas.Workflow.Reflection;

    /// <summary>
    /// Interface for transition context.
    /// </summary>
    public interface ITransitionContext : IContext
    {
        /// <summary>
        /// Gets the state machine.
        /// </summary>
        /// <value>
        /// The state machine.
        /// </value>
        IStateMachine StateMachine { get; }

        /// <summary>
        /// Gets or sets the target state.
        /// </summary>
        /// <value>
        /// The target state.
        /// </value>
        object To { get; set; }

        /// <summary>
        /// Gets or sets the arguments.
        /// </summary>
        /// <value>
        /// The arguments.
        /// </value>
        IExpando? Arguments { get; set; }

        /// <summary>
        /// Gets or sets information describing the transition.
        /// </summary>
        /// <value>
        /// Information describing the transition.
        /// </value>
        ITransitionInfo? TransitionInfo { get; set; }
    }
}
