// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelElementConstructorBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Base runtime provider for model element information.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Runtime.Construction
{
    using System.Collections.Generic;
    using System.Linq;

    using Kephas.Model.Construction;
    using Kephas.Model.Construction.Internal;
    using Kephas.Model.Elements;
    using Kephas.Model.Reflection;
    using Kephas.Reflection;
    using Kephas.Runtime;

    /// <summary>
    /// Base runtime provider for model element information.
    /// </summary>
    /// <typeparam name="TModel">The type of the element information.</typeparam>
    /// <typeparam name="TModelContract">Type of the model contract.</typeparam>
    /// <typeparam name="TRuntime">The type of the runtime information.</typeparam>
    public abstract class ModelElementConstructorBase<TModel, TModelContract, TRuntime> : NamedElementConstructorBase<TModel, TModelContract, TRuntime>
        where TModel : ModelElementBase<TModelContract>
        where TModelContract : class, IModelElement
        where TRuntime : class, IElementInfo, IRuntimeElementInfo
    {
        /// <summary>
        /// Constructs the model element content.
        /// </summary>
        /// <param name="constructionContext">Context for the construction.</param>
        /// <param name="runtimeElement">The runtime element.</param>
        /// <param name="element">The element being constructed.</param>
        protected override void ConstructModelElementContent(IModelConstructionContext constructionContext, TRuntime runtimeElement, TModel element)
        {
            var members = this.ComputeMembers(constructionContext, runtimeElement);
            var writableElement = (IConstructibleElement)element;
            foreach (var member in members)
            {
                writableElement.AddMember(member);
            }
        }

        /// <summary>
        /// Computes the members from the runtime element.
        /// </summary>
        /// <param name="constructionContext">The model construction context.</param>
        /// <param name="runtimeElement">The runtime member information.</param>
        /// <returns>
        /// An enumeration of <see cref="INamedElement"/>.
        /// </returns>
        protected virtual IEnumerable<INamedElement> ComputeMembers(IModelConstructionContext constructionContext, TRuntime runtimeElement)
        {
            var members = new List<INamedElement>();

            var annotations = this.ComputeMemberAnnotations(constructionContext, runtimeElement);
            if (annotations != null)
            {
                members.AddRange(annotations);
            }

            var properties = this.ComputeMemberProperties(constructionContext, runtimeElement);
            if (properties != null)
            {
                members.AddRange(properties);
            }


            var methods = this.ComputeMemberMethods(constructionContext, runtimeElement);
            if (methods != null)
            {
                members.AddRange(methods);
            }

            return members;
        }

        /// <summary>
        /// Computes the member annotations from the runtime element.
        /// </summary>
        /// <param name="constructionContext">The model construction context.</param>
        /// <param name="runtimeElement">The runtime member information.</param>
        /// <returns>
        /// An enumeration of <see cref="INamedElement"/>.
        /// </returns>
        protected virtual IEnumerable<INamedElement> ComputeMemberAnnotations(IModelConstructionContext constructionContext, TRuntime runtimeElement)
        {
            var runtimeModelElementFactory = constructionContext.RuntimeModelElementFactory;
            var attributes = runtimeElement.GetUnderlyingElementInfo().GetCustomAttributes(inherit: false);
            return attributes.Select(attr => runtimeModelElementFactory.TryCreateModelElement(constructionContext, attr))
                             .Where(annotation => annotation != null);
        }

        /// <summary>
        /// Computes the member properties from the runtime element.
        /// </summary>
        /// <param name="constructionContext">The model construction context.</param>
        /// <param name="runtimeElement">The runtime member information.</param>
        /// <returns>
        /// An enumeration of <see cref="INamedElement"/>.
        /// </returns>
        protected virtual IEnumerable<INamedElement> ComputeMemberProperties(IModelConstructionContext constructionContext, TRuntime runtimeElement)
        {
            var runtimeModelElementFactory = constructionContext.RuntimeModelElementFactory;
            if (!(runtimeElement is IRuntimeTypeInfo typeInfo))
            {
                return new List<INamedElement>();
            }

            // TODO optimize typeInfo.DeclaredProperties
            var properties = typeInfo.Properties.Values
                .Where(p => p.DeclaringContainer == typeInfo && !p.PropertyInfo.IsExcludedFromModel())
                .Select(p => runtimeModelElementFactory.TryCreateModelElement(constructionContext, p))
                .Where(property => property != null);
            return properties;
        }

        /// <summary>
        /// Computes the member properties from the runtime element.
        /// </summary>
        /// <param name="constructionContext">The model construction context.</param>
        /// <param name="runtimeElement">The runtime member information.</param>
        /// <returns>
        /// An enumeration of <see cref="INamedElement"/>.
        /// </returns>
        protected virtual IEnumerable<INamedElement> ComputeMemberMethods(IModelConstructionContext constructionContext, TRuntime runtimeElement)
        {
            var runtimeModelElementFactory = constructionContext.RuntimeModelElementFactory;
            if (!(runtimeElement is IRuntimeTypeInfo typeInfo))
            {
                return new List<INamedElement>();
            }

            // TODO optimize typeInfo.DeclaredMethods
            var properties = typeInfo.Methods.SelectMany(m => m.Value)
                .Where(p => p.DeclaringContainer == typeInfo && !p.MethodInfo.IsExcludedFromModel() && !p.MethodInfo.IsSpecialName)
                .Select(p => runtimeModelElementFactory.TryCreateModelElement(constructionContext, p))
                .Where(method => method != null);
            return properties;
        }
    }
}