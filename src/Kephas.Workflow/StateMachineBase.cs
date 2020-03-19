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
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

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

        /// <summary>
        /// Transitions the state machine asynchronously.
        /// </summary>
        /// <param name="context">The transition context.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the transition result.
        /// </returns>
        public Task<object?> TransitionAsync(ITransitionContext context, CancellationToken cancellationToken = default)
        {
            Requires.NotNull(context, nameof(context));

            var transitionInfo = this.GetTransitionInfo(context);

            if (transitionInfo == null)
            {
                throw new InvalidTransitionException($"Cound not identify transition from '{this.CurrentState}' to '{context.To}'");
            }

            var returnType = transitionInfo.ReturnType.AsType();
            if (returnType == typeof(Task))
            {
                var task = (Task)transitionInfo.Invoke(this.Target, this.GetInvocationArguments(transitionInfo, context, cancellationToken))!;
                return task.ContinueWith(t => (object?)null);
            }

            if (returnType.IsConstructedGenericOf(typeof(Task<>)))
            {
                var task = (Task)transitionInfo.Invoke(this.Target, this.GetInvocationArguments(transitionInfo, context, cancellationToken))!;
                return task.ContinueWith(t => t.GetPropertyValue(nameof(Task<int>.Result)));
            }

#if NETSTANDARD2_1
            if (returnType == typeof(ValueTask))
            {
                var valueTask = (ValueTask)transitionInfo.Invoke(this.Target, this.GetInvocationArguments(transitionInfo, context, cancellationToken))!;
                return valueTask.AsTask().ContinueWith(t => (object?)null);
            }

            if (returnType.IsConstructedGenericOf(typeof(ValueTask<>)))
            {
                var valueTaskObject = transitionInfo.Invoke(this.Target, this.GetInvocationArguments(transitionInfo, context, cancellationToken))!;
                var task = (Task)valueTaskObject.GetRuntimeTypeInfo().Invoke(valueTaskObject, nameof(ValueTask<int>.AsTask), Array.Empty<object>());
                return task.ContinueWith(t => t.GetPropertyValue(nameof(Task<int>.Result)));
            }
#endif

            return Task.FromResult(transitionInfo.Invoke(this.Target, this.GetInvocationArguments(transitionInfo, context, cancellationToken)));
        }

        /// <summary>
        /// Gets the transition information from the transition context.
        /// </summary>
        /// <exception cref="AmbiguousMatchException">Thrown when the Ambiguous Match error condition
        ///                                           occurs.</exception>
        /// <param name="context">The transition context.</param>
        /// <returns>
        /// The transition information.
        /// </returns>
        protected virtual ITransitionInfo? GetTransitionInfo(ITransitionContext context)
        {
            var currentState = this.CurrentState;

            if (context.TransitionInfo != null)
            {
                if (context.To != null && !object.Equals(context.TransitionInfo.To, context.To))
                {
                    throw new InvalidTransitionException($"The target state '{context.To}' does not match the '{context.TransitionInfo.Name}' transition's target state '{context.TransitionInfo.To}'.");
                }

                if (!context.TransitionInfo.From.Contains(currentState))
                {
                    throw new InvalidTransitionException($"The current state '{currentState}' does not match the '{context.TransitionInfo.Name}' transition's source states '{context.TransitionInfo.To}'.");
                }

                return context.TransitionInfo;
            }

            if (context.To == null)
            {
                return null;
            }

            var matchingTransitions = this.GetTypeInfo().Transitions
                .Where(t => context.To.Equals(t.To) && t.From.Contains(currentState))
                .ToList();
            if (matchingTransitions.Count == 0)
            {
                return null;
            }

            if (matchingTransitions.Count == 1)
            {
                return matchingTransitions[0];
            }

            throw new AmbiguousMatchException($"Multiple transitions found from '{currentState}' to '{context.To}': '{string.Join("', '", matchingTransitions.Select(t => t.Name))}'.");
        }

        private IEnumerable<object?> GetInvocationArguments(ITransitionInfo transitionInfo, ITransitionContext context, CancellationToken cancellationToken)
        {
            foreach (var paramInfo in transitionInfo.Parameters)
            {
                if (context.Arguments?.HasDynamicMember(paramInfo.Name) ?? false)
                {
                    yield return context.Arguments[paramInfo.Name];
                }

                if (paramInfo.ValueType.AsType() == typeof(CancellationToken))
                {
                    yield return cancellationToken;
                }

                yield return null;
            }
        }
    }
}
