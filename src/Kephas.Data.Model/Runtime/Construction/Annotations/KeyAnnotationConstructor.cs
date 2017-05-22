// --------------------------------------------------------------------------------------------------------------------
// <copyright file="KeyAnnotationConstructor.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the key annotation constructor class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Model.Runtime.Construction.Annotations
{
    using Kephas.Data.Model.AttributedModel;
    using Kephas.Data.Model.Elements.Annotations;
    using Kephas.Model.Construction;
    using Kephas.Model.Runtime.Construction;

    /// <summary>
    /// A key annotation constructor.
    /// </summary>
    public class KeyAnnotationConstructor : AnnotationConstructorBase<KeyAnnotation, KeyAttribute>
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
        protected override KeyAnnotation TryCreateModelElementCore(IModelConstructionContext constructionContext, KeyAttribute runtimeElement)
        {
            return new KeyAnnotation(constructionContext, this.TryComputeName(constructionContext, runtimeElement), runtimeElement);
        }

        /// <summary>
        /// Computes the model element name based on the runtime element.
        /// </summary>
        /// <param name="runtimeElement">The runtime element.</param>
        /// <returns>The element name, or <c>null</c> if the name could not be computed.</returns>
        protected override string TryComputeNameCore(object runtimeElement)
        {
            var attribute = (KeyAttribute)runtimeElement;
            if (!string.IsNullOrEmpty(attribute.Name))
            {
                return attribute.Name;
            }

            return string.Join("_", attribute.KeyProperties);
        }
    }
}