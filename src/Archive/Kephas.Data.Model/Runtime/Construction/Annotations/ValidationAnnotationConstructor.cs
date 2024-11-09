// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValidationAnnotationConstructor.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the validation annotation constructor class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Model.Runtime.Construction.Annotations
{
    using System.ComponentModel.DataAnnotations;

    using Kephas.Data.Model.Elements.Annotations;
    using Kephas.Model.Construction;
    using Kephas.Model.Runtime.Construction;

    /// <summary>
    /// A validation annotation constructor.
    /// </summary>
    public class ValidationAnnotationConstructor : AnnotationConstructorBase<ValidationAnnotation, ValidationAttribute>
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
        protected override ValidationAnnotation TryCreateModelElementCore(IModelConstructionContext constructionContext, ValidationAttribute runtimeElement)
        {
            return new ValidationAnnotation(constructionContext, this.TryComputeName(runtimeElement, constructionContext), runtimeElement);
        }
    }
}