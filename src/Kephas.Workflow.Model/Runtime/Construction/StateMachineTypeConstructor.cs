// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StateMachineTypeConstructor.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the state machine type constructor class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Kephas.Model;
using Kephas.Workflow.Reflection;

namespace Kephas.Workflow.Model.Runtime.Construction
{
    using System.Reflection;

    using Kephas.Model.Construction;
    using Kephas.Model.Runtime.Construction;
    using Kephas.Runtime;
    using Kephas.Workflow.Model.Elements;

    /// <summary>
    /// A state machine type constructor.
    /// </summary>
    public class StateMachineTypeConstructor : ClassifierConstructorBase<StateMachineType, IStateMachineType>
    {
        /// <summary>
        /// The state machine discriminator.
        /// </summary>
        public static readonly string StateMachineDiscriminator = "StateMachine";

        /// <summary>
        /// Gets the element name discriminator.
        /// </summary>
        /// <value>
        /// The element name discriminator.
        /// </value>
        /// <remarks>
        /// This discriminator can be used as a suffix in the name to identify the element type.
        /// </remarks>
        protected override string ElementNameDiscriminator => StateMachineDiscriminator;

        /// <summary>
        /// Determines whether a model element can be created for the provided runtime element.
        /// </summary>
        /// <param name="constructionContext">Context for the construction.</param>
        /// <param name="runtimeElement">The runtime element.</param>
        /// <returns>
        /// <c>true</c> if a model element can be created, <c>false</c> if not.
        /// </returns>
        protected override bool CanCreateModelElement(IModelConstructionContext constructionContext, IRuntimeTypeInfo runtimeElement)
        {
            return runtimeElement is IStateMachineInfo;
        }

        /// <summary>
        /// Core implementation of trying to get the element information.
        /// </summary>
        /// <param name="constructionContext">Context for the construction.</param>
        /// <param name="runtimeElement">The runtime element.</param>
        /// <returns>
        /// A new element information based on the provided runtime element information, or <c>null</c>
        /// if the runtime element information is not supported.
        /// </returns>
        protected override StateMachineType TryCreateModelElementCore(
            IModelConstructionContext constructionContext,
            IRuntimeTypeInfo runtimeElement)
        {
            return new StateMachineType(constructionContext, this.TryComputeName(runtimeElement, constructionContext));
        }

        /// <summary>
        /// Computes the members from the runtime element.
        /// </summary>
        /// <param name="constructionContext">The model construction context.</param>
        /// <param name="runtimeElement">The runtime member information.</param>
        /// <returns>
        /// An enumeration of <see cref="INamedElement"/>.
        /// </returns>
        protected override IEnumerable<INamedElement> ComputeMembers(IModelConstructionContext constructionContext, IRuntimeTypeInfo runtimeElement)
        {
            var members = new List<INamedElement>(base.ComputeMembers(constructionContext, runtimeElement));

            var transitions = this.ComputeMemberTransitions(constructionContext, runtimeElement);
            if (transitions != null)
            {
                members.AddRange(transitions);
            }

            return members;
        }

        /// <summary>
        /// Computes the member properties from the runtime element.
        /// </summary>
        /// <param name="constructionContext">The model construction context.</param>
        /// <param name="runtimeElement">The runtime member information.</param>
        /// <returns>
        /// An enumeration of <see cref="INamedElement"/>.
        /// </returns>
        protected virtual IEnumerable<INamedElement> ComputeMemberTransitions(IModelConstructionContext constructionContext, IRuntimeTypeInfo runtimeElement)
        {
            var runtimeModelElementFactory = constructionContext.RuntimeModelElementFactory;
            if (!(runtimeElement is IStateMachineInfo typeInfo))
            {
                return new List<INamedElement>();
            }

            var transitions = typeInfo.Transitions
                .Select(p => runtimeModelElementFactory.TryCreateModelElement(constructionContext, p))
                .Where(property => property != null);
            return transitions;
        }
    }
}
