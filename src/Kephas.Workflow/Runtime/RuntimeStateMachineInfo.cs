// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimeStateMachineInfo.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the runtime state machine information class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#nullable enable

namespace Kephas.Workflow.Runtime
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Kephas.Collections;
    using Kephas.Dynamic;
    using Kephas.Operations;
    using Kephas.Reflection;
    using Kephas.Runtime;
    using Kephas.Threading.Tasks;
    using Kephas.Workflow.AttributedModel;
    using Kephas.Workflow.Reflection;

    /// <summary>
    /// Information about the runtime state machine.
    /// </summary>
    public class RuntimeStateMachineInfo : RuntimeTypeInfo, IStateMachineInfo
    {
        private ITypeInfo? targetType;
        private ITypeInfo? stateType;
        private IPropertyInfo? targetStateProperty;

        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeStateMachineInfo"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        protected internal RuntimeStateMachineInfo(Type type)
            : base(type)
        {
        }

        /// <summary>
        /// Gets the type of the state. Typically it is an enumeration.
        /// </summary>
        /// <value>
        /// The type of the state.
        /// </value>
        public ITypeInfo StateType => this.stateType ??= this.ComputeStateType();

        /// <summary>
        /// Gets the type of the target controlled by the state machine.
        /// </summary>
        /// <value>
        /// The type of the target.
        /// </value>
        public ITypeInfo TargetType => this.targetType ??= this.ComputeTargetType();

        /// <summary>
        /// Gets the state property of the target storing the current state.
        /// </summary>
        /// <value>
        /// The state property of the target storing the current state.
        /// </value>
        public IPropertyInfo TargetStateProperty => this.targetStateProperty ??= this.ComputeTargetStateProperty();

        /// <summary>
        /// Gets the activities transitioning the target through different states.
        /// </summary>
        /// <value>
        /// The transition activities.
        /// </value>
        public IEnumerable<ITransitionInfo> Transitions => this.Members.Values.OfType<ITransitionInfo>();

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
        public Task<object> TransitionAsync(IStateMachine stateMachine, object targetState, ITransitionInfo? transitionInfo, IExpando? arguments, ITransitionContext context, CancellationToken cancellationToken = default)
        {
            context.To = targetState;
            if (transitionInfo != null)
            {
                context.TransitionInfo = transitionInfo;
            }

            if (arguments != null)
            {
                context.Arguments = arguments;
            }

            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates the member infos.
        /// </summary>
        /// <param name="membersConfig">Optional. The members configuration.</param>
        /// <returns>
        /// The new member infos.
        /// </returns>
        protected override IDictionary<string, IRuntimeElementInfo> CreateMemberInfos(Action<IDictionary<string, IRuntimeElementInfo>>? membersConfig = null)
        {
            void AddTransitions(IDictionary<string, IRuntimeElementInfo> m)
            {
                m.Where(kv => kv.Value is IRuntimeMethodInfo mi && mi.GetAttribute<TransitionAttribute>() != null)
                    .ToList()
                    .ForEach(kv => m.Add($"{kv.Value.Name}#trans", new RuntimeTransitionMethodInfo(((IRuntimeMethodInfo)kv.Value).MethodInfo)));

                membersConfig?.Invoke(m);
            }

            return base.CreateMemberInfos(AddTransitions);
        }

        private ITypeInfo? ComputeStateType()
        {
            var stateMachineInterface = this.Type.GetBaseConstructedGenericOf(typeof(IStateMachine<,>));
            if (stateMachineInterface == null)
            {
                return typeof(object).AsRuntimeTypeInfo();
            }

            return stateMachineInterface.GetGenericArguments()[1].AsRuntimeTypeInfo();
        }

        private ITypeInfo? ComputeTargetType()
        {
            var stateMachineInterface = this.Type.GetBaseConstructedGenericOf(typeof(IStateMachine<,>));
            if (stateMachineInterface == null)
            {
                return typeof(object).AsRuntimeTypeInfo();
            }

            return stateMachineInterface.GetGenericArguments()[0].AsRuntimeTypeInfo();
        }

        private IPropertyInfo? ComputeTargetStateProperty()
        {
            var stateType = this.StateType;
            var stateProperties = this.TargetType.Properties.Where(p => p.ValueType == stateType).ToList();
            if (stateProperties.Count == 0)
            {
                throw new WorkflowException($"Type {this.TargetType} does not have a state property of type {stateType}.");
            }

            if (stateProperties.Count > 1)
            {
                throw new WorkflowException($"Type {this.TargetType} has multiple properties of type {stateType}: {string.Join(",", stateProperties.Select(p => p.Name))}.");
            }

            return stateProperties[0];
        }
    }
}
