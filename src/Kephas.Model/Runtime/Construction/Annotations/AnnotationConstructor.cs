// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AnnotationConstructor.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   A runtime factory for annotation information.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Runtime.Construction.Annotations
{
    using System;

    using Kephas.Model.Elements;
    using Kephas.Model.Factory;

    /// <summary>
    /// A default runtime factory for annotation information.
    /// </summary>
    public class AnnotationConstructor : AnnotationConstructorBase<Annotation, Attribute>
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
            return new Annotation(constructionContext, this.TryComputeNameCore(runtimeElement));
        }
    }
}