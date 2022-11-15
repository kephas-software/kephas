// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransitionContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the transition context class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Services;

namespace Kephas.Workflow
{
    using Kephas.Dynamic;
    using Kephas.Services;
    using Kephas.Workflow.Reflection;

    /// <summary>
    /// A transition context.
    /// </summary>
    public class TransitionContext : Context, ITransitionContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TransitionContext"/> class.
        /// </summary>
        /// <param name="serviceProvider">The injector.</param>
        /// <param name="stateMachine">The state machine.</param>
        public TransitionContext(IServiceProvider serviceProvider, IStateMachine stateMachine)
            : base(serviceProvider)
        {
            this.StateMachine = stateMachine;
        }

        /// <summary>
        /// Gets the state machine.
        /// </summary>
        /// <value>
        /// The state machine.
        /// </value>
        public IStateMachine StateMachine { get; }

        /// <summary>
        /// Gets or sets the target state.
        /// </summary>
        /// <value>
        /// The target state.
        /// </value>
        public object? To { get; set; }

        /// <summary>
        /// Gets or sets the arguments.
        /// </summary>
        /// <value>
        /// The arguments.
        /// </value>
        public IDynamic? Arguments { get; set; }

        /// <summary>
        /// Gets or sets information describing the transition.
        /// </summary>
        /// <value>
        /// Information describing the transition.
        /// </value>
        public ITransitionInfo? TransitionInfo { get; set; }
    }
}
