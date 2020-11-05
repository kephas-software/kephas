// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransitionConstructor.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Workflow.Model.Runtime.Construction
{
    using Kephas.Model.Construction;
    using Kephas.Model.Runtime.Construction;
    using Kephas.Workflow.Model.Elements;
    using Kephas.Workflow.Reflection;
    using Kephas.Workflow.Runtime;

    /// <summary>
    /// A <see cref="Transition"/> constructor.
    /// </summary>
    public class TransitionConstructor : ModelElementConstructorBase<Transition, ITransition, RuntimeTransitionMethodInfo>
    {
        /// <summary>
        /// Core implementation of trying to get the element information.
        /// </summary>
        /// <param name="constructionContext">Context for the construction.</param>
        /// <param name="runtimeElement">The runtime element.</param>
        /// <returns>
        /// A new element information based on the provided runtime element information, or <c>null</c>
        /// if the runtime element information is not supported.
        /// </returns>
        protected override Transition TryCreateModelElementCore(
            IModelConstructionContext constructionContext,
            RuntimeTransitionMethodInfo runtimeElement)
        {
            var transition = new Transition(constructionContext, this.TryComputeNameCore(runtimeElement, constructionContext))
            {
                From = runtimeElement.From,
                To = runtimeElement.To,
            };
            return transition;
        }
    }
}