// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AttributeAnnotationConstructor.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   A runtime factory for annotation information.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Runtime.Construction.Annotations
{
    using System;

    using Kephas.Model.Construction;
    using Kephas.Model.Elements;
    using Kephas.Model.Elements.Annotations;
    using Kephas.Reflection;
    using Kephas.Services;

    /// <summary>
    /// A default runtime factory for annotation information.
    /// </summary>
    [ProcessingPriority(Priority.Low)]
    public class AttributeAnnotationConstructor : AnnotationConstructorBase<Annotation, Attribute>
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
        protected override Annotation TryCreateModelElementCore(IModelConstructionContext constructionContext, Attribute runtimeElement)
        {
            var annotationType = typeof(AttributeAnnotation<>).MakeGenericType(runtimeElement.GetType());
            var annotation = annotationType.AsRuntimeTypeInfo(constructionContext.AmbientServices?.TypeRegistry).CreateInstance(
                new object[]
                {
                    constructionContext,
                    this.TryComputeNameCore(runtimeElement),
                    runtimeElement,
                });
            return (Annotation)annotation;
        }
    }
}