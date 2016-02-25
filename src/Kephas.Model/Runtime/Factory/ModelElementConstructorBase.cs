// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimeModelElementInfoProviderBase.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Base runtime provider for model element information.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Reflection;

using Kephas.Model.AttributedModel;

namespace Kephas.Model.Runtime.Factory
{
    using System.Collections.Generic;
    using System.Linq;

    using Kephas.Dynamic;
    using Kephas.Model.Elements;
    using Kephas.Model.Factory;
    using Kephas.Reflection;

    /// <summary>
    /// Base runtime provider for model element information.
    /// </summary>
    /// <typeparam name="TModel">The type of the element information.</typeparam>
    /// <typeparam name="TRuntime">The type of the runtime information.</typeparam>
    public abstract class ModelElementConstructorBase<TModel, TRuntime> : NamedElementConstructorBase<TModel, TRuntime>
        where TModel : ModelElementBase<TModel>
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