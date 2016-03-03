// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RequiredAnnotationConstructor.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the required annotation constructor class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Runtime.Construction.Annotations
{
    using Kephas.Model.AttributedModel.Behaviors;
    using Kephas.Model.Construction;
    using Kephas.Model.Elements.Annotations;

    /// <summary>
    /// A required annotation constructor.
    /// </summary>
    public class RequiredAnnotationConstructor : AnnotationConstructorBase<RequiredAnnotation, RequiredAttribute>
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
        protected override RequiredAnnotation TryCreateModelElementCore(
            IModelConstructionContext constructionContext,
            RequiredAttribute runtimeElement)
        {
            return new RequiredAnnotation(constructionContext, this.TryComputeNameCore(runtimeElement));
        }
    }
}