// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValidationAnnotationConstructor.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
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
        /// The static instance of the <see cref="ValidationAnnotationConstructor"/>.
        /// </summary>
        public static readonly ValidationAnnotationConstructor Instance = new ValidationAnnotationConstructor();

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
            return new ValidationAnnotation(constructionContext, this.TryComputeName(constructionContext, runtimeElement), runtimeElement);
        }
    }
}