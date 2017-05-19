// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValidationAnnotation.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the validation annotation class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Model.Elements.Annotations
{
    using Kephas.Model.Construction;
    using Kephas.Model.Elements;

    /// <summary>
    /// A validation annotation.
    /// </summary>
    public class ValidationAnnotation : Annotation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationAnnotation"/> class.
        /// </summary>
        /// <param name="constructionContext">Context for the construction.</param>
        /// <param name="name">The model element name.</param>
        public ValidationAnnotation(IModelConstructionContext constructionContext, string name)
            : base(constructionContext, name)
        {
        }
    }
}