// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MethodConstructor.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the method constructor class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Runtime.Construction
{
    using System.Collections.Generic;
    using System.Linq;

    using Kephas.Collections;
    using Kephas.Model.Construction;
    using Kephas.Model.Elements;
    using Kephas.Runtime;
    using Kephas.Services;

    /// <summary>
    /// A method constructor.
    /// </summary>
    [ProcessingPriority(Priority.Low)]
    public class MethodConstructor : ModelElementConstructorBase<Method, IMethod, IRuntimeMethodInfo>
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
        protected override Method? TryCreateModelElementCore(IModelConstructionContext constructionContext, IRuntimeMethodInfo runtimeElement)
        {
            var method = new Method(constructionContext, this.TryComputeNameCore(runtimeElement, constructionContext))
                               {
                               };
            return method;
        }

        /// <summary>
        /// Computes the members from the runtime element.
        /// </summary>
        /// <param name="constructionContext">The model construction context.</param>
        /// <param name="runtimeElement">The runtime member information.</param>
        /// <returns>
        /// An enumeration of <see cref="INamedElement"/>.
        /// </returns>
        protected override IEnumerable<INamedElement> ComputeMembers(IModelConstructionContext constructionContext, IRuntimeMethodInfo runtimeElement)
        {
            var members = (IList<INamedElement>)base.ComputeMembers(constructionContext, runtimeElement);

            var parameters = this.ComputeMemberParameters(constructionContext, runtimeElement);
            if (parameters != null)
            {
                members.AddRange(parameters);
            }

            return members;
        }

        /// <summary>
        /// Computes the member parameters from the runtime element.
        /// </summary>
        /// <param name="constructionContext">The model construction context.</param>
        /// <param name="runtimeElement">The runtime member information.</param>
        /// <returns>
        /// An enumeration of <see cref="INamedElement"/>.
        /// </returns>
        protected virtual IEnumerable<INamedElement> ComputeMemberParameters(IModelConstructionContext constructionContext, IRuntimeMethodInfo runtimeElement)
        {
            var runtimeModelElementFactory = constructionContext.RuntimeModelElementFactory;

            var parameters = runtimeElement.Parameters.Values
                .Select(p => runtimeModelElementFactory.TryCreateModelElement(constructionContext, p))
                .Where(parameter => parameter != null);
            return parameters;
        }
    }
}