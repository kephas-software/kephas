// --------------------------------------------------------------------------------------------------------------------
// <copyright file="KeyAnnotation.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the key annotation class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Model.Elements.Annotations
{
    using Kephas.Data.Model.AttributedModel;
    using Kephas.Model.Construction;
    using Kephas.Model.Elements.Annotations;

    /// <summary>
    /// Defines annotations for entity keys.
    /// </summary>
    public class KeyAnnotation : AttributeAnnotation<KeyAttribute>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="KeyAnnotation"/> class.
        /// </summary>
        /// <param name="constructionContext">Context for the construction.</param>
        /// <param name="name">The model element name.</param>
        /// <param name="attribute">The attribute.</param>
        public KeyAnnotation(IModelConstructionContext constructionContext, string name, KeyAttribute attribute)
            : base(constructionContext, name, attribute)
        {
        }
    }
}