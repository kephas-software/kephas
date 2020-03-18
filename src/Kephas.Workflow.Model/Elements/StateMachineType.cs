// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StateMachineType.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the state machine type class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#nullable enable

namespace Kephas.Workflow.Model.Elements
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Dynamic;
    using Kephas.Model.Construction;
    using Kephas.Model.Elements;
    using Kephas.Reflection;
    using Kephas.Workflow;
    using Kephas.Workflow.Reflection;

    /// <summary>
    /// Classifier for state machines.
    /// </summary>
    public class StateMachineType : ClassifierBase<IStateMachineType>, IStateMachineType
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StateMachineType"/> class.
        /// </summary>
        /// <param name="constructionContext">Context for the construction.</param>
        /// <param name="name">The name.</param>
        public StateMachineType(IModelConstructionContext constructionContext, string name)
            : base(constructionContext, name)
        {
        }

        /// <summary>
        /// Gets the type of the state.
        /// </summary>
        /// <value>
        /// The type of the state.
        /// </value>
        public ITypeInfo StateType { get; protected set; }

        /// <summary>
        /// Gets the type of the target.
        /// </summary>
        /// <value>
        /// The type of the target.
        /// </value>
        public ITypeInfo TargetType { get; protected set; }

        /// <summary>
        /// Gets target state property.
        /// </summary>
        /// <value>
        /// The target state property.
        /// </value>
        public IPropertyInfo TargetStateProperty { get; protected set; }

        /// <summary>
        /// Gets the transitions.
        /// </summary>
        /// <value>
        /// The transitions.
        /// </value>
        public IEnumerable<IActivityInfo> Transitions => this.Members.OfType<IActivityInfo>();

        /// <summary>
        /// Executes the asynchronous operation.
        /// </summary>
        /// <param name="transition">The transition.</param>
        /// <param name="target">Target for the.</param>
        /// <param name="arguments">The arguments.</param>
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the execution output.
        /// </returns>
        public Task<object> ExecuteAsync(IActivity transition, object target, IExpando arguments, IActivityContext context, CancellationToken cancellationToken = default)
        {
            return transition.GetTypeInfo().ExecuteAsync(transition, target, arguments, context, cancellationToken);
        }

        /// <summary>
        /// Called when the construction is complete.
        /// </summary>
        /// <param name="constructionContext">Context for the construction.</param>
        protected override void OnCompleteConstruction(IModelConstructionContext constructionContext)
        {
            base.OnCompleteConstruction(constructionContext);

            var stateMachinePart = this.Parts.OfType<IStateMachineInfo>().FirstOrDefault();
            this.StateType = stateMachinePart?.StateType;
            this.TargetType = stateMachinePart?.TargetType;
            this.TargetStateProperty = stateMachinePart?.TargetStateProperty;
        }
    }
}
