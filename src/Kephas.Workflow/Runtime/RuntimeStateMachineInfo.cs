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

    using Kephas.Dynamic;
    using Kephas.Operations;
    using Kephas.Reflection;
    using Kephas.Runtime;
    using Kephas.Threading.Tasks;
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
        public IEnumerable<IActivityInfo> Transitions => this.Members.OfType<IActivityInfo>();

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
        public async Task<object> ExecuteAsync(IActivity transition, object? target, IExpando? arguments, IActivityContext context, CancellationToken cancellationToken = default)
        {
            transition.Target = target;
            transition.Arguments = arguments;
            transition.Context = context;

            if (transition is IOperation operation)
            {
                return operation.Execute(context);
            }

            if (transition is IAsyncOperation asyncOperation)
            {
                return await asyncOperation.ExecuteAsync(context, cancellationToken).PreserveThreadContext();
            }

            // TODO localization
            throw new NotImplementedException($"Either implement the {typeof(IOperation).Name} or {typeof(IAsyncOperation).Name} in the transition of type '{transition?.GetType()}', or provide a specialized type info.");
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
