// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelElementConstructorBase.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Base runtime provider for model element information.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Runtime.Construction
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Kephas.Dynamic;
    using Kephas.Model.AttributedModel;
    using Kephas.Model.Construction;
    using Kephas.Model.Elements;
    using Kephas.Reflection;

    /// <summary>
    /// Base runtime provider for model element information.
    /// </summary>
    /// <typeparam name="TModel">The type of the element information.</typeparam>
    /// <typeparam name="TModelContract">Type of the model contract.</typeparam>
    /// <typeparam name="TRuntime">The type of the runtime information.</typeparam>
    public abstract class ModelElementConstructorBase<TModel, TModelContract, TRuntime> : NamedElementConstructorBase<TModel, TModelContract, TRuntime>
        where TModel : ModelElementBase<TModelContract>
        where TModelContract : class, IModelElement
        where TRuntime : class, IElementInfo, IDynamicElementInfo
    {
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
            var attributes = runtimeElement.GetUnderlyingMemberInfo().GetCustomAttributes(inherit: false);
            return attributes.Select(runtimeElement1 => runtimeModelElementFactory.TryCreateModelElement(constructionContext, runtimeElement1))
                             .Where(annotationInfo => annotationInfo != null);
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
            var typeInfo = runtimeElement as IDynamicTypeInfo;
            if (typeInfo == null)
            {
                return new List<INamedElement>();
            }

            // TODO optimize typeInfo.DeclaredProperties
            var properties = typeInfo.Properties.Values
                .Where(p => p.DeclaringContainer == typeInfo && p.PropertyInfo.GetCustomAttribute<ExcludeFromModelAttribute>() == null)
                .Select(p => runtimeModelElementFactory.TryCreateModelElement(constructionContext, p))
                .Where(property => property != null);
            return properties;
        }
    }
}