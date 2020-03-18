// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StateMachineBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the state machine base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#nullable enable

namespace Kephas.Workflow
{
    using Kephas.Diagnostics.Contracts;
    using Kephas.Reflection;
    using Kephas.Workflow.Reflection;

    /// <summary>
    /// A state machine base.
    /// </summary>
    /// <typeparam name="TTarget">Type of the target.</typeparam>
    /// <typeparam name="TState">Type of the state.</typeparam>
    public abstract class StateMachineBase<TTarget, TState> : IStateMachine<TTarget, TState>
        where TTarget : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StateMachineBase{TTarget, TState}"/> class.
        /// </summary>
        /// <param name="target">The target instance to control.</param>
        protected StateMachineBase(TTarget target)
        {
            Requires.NotNull(target, nameof(target));

            this.Target = target;
        }

        /// <summary>
        /// Gets the target controlled by the state machine.
        /// </summary>
        /// <value>
        /// The target.
        /// </value>
        public TTarget Target { get; }

        /// <summary>
        /// Gets the current state of the target.
        /// </summary>
        /// <value>
        /// The current state of the target.
        /// </value>
        public virtual TState CurrentState => (TState)this.GetTypeInfo().TargetStateProperty.GetValue(this.Target);

        /// <summary>
        /// Gets the target controlled by the state machine.
        /// </summary>
        /// <value>
        /// The target.
        /// </value>
        object IStateMachine.Target => this.Target;

        /// <summary>
        /// Gets the current state of the target.
        /// </summary>
        /// <value>
        /// The current state of the target.
        /// </value>
        object IStateMachine.CurrentState => this.CurrentState;

        /// <summary>
        /// Gets type information.
        /// </summary>
        /// <returns>
        /// The type information.
        /// </returns>
        public virtual IStateMachineInfo GetTypeInfo() => (IStateMachineInfo)this.GetRuntimeTypeInfo();

        /// <summary>
        /// Gets the type information.
        /// </summary>
        /// <returns>
        /// The type information.
        /// </returns>
        ITypeInfo IInstance.GetTypeInfo() => this.GetTypeInfo();
    }
}
