// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Annotation.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Definition class for annotations.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Elements
{
    using System.Collections.Generic;

    using Kephas.Model.Construction;

    /// <summary>
    /// Definition class for annotations.
    /// </summary>
    public class Annotation : NamedElementBase<IAnnotation>, IAnnotation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Annotation"/> class.
        /// </summary>
        /// <param name="constructionContext">Context for the construction.</param>
        /// <param name="name">The model element name.</param>
        public Annotation(IModelConstructionContext constructionContext, string name)
            : base(constructionContext, name)
        {
        }

        /// <summary>
        /// Gets a value indicating whether multiple annotations of the same kind are allowed to be placed the same model element.
        /// </summary>
        /// <value>
        ///   <c>true</c> if multiple annotations of the same kind are allowed; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// If multiple annotations of the same kind are allowed, the qualified name will have a generated suffix 
        /// to allow the annotation to be unique within the members collection.
        /// </remarks>
        public bool AllowMultiple { get; internal set; }

        /// <summary>
        /// Gets the element annotations.
        /// </summary>
        /// <value>
        /// The element annotations.
        /// </value>
        public override IEnumerable<IAnnotation> Annotations => ModelHelper.EmptyAnnotations;
    }
}