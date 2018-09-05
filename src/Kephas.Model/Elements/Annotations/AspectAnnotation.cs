// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AspectAnnotation.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the aspect annotation class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Elements.Annotations
{
    using System;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Model.Construction;

    /// <summary>
    /// Annotation foe aspect classifiers.
    /// </summary>
    public class AspectAnnotation : MixinAnnotation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AspectAnnotation"/> class.
        /// </summary>
        /// <param name="constructionContext">Context for the construction.</param>
        /// <param name="name">The model element name.</param>
        /// <param name="classifierFilter">A filter to select classifiers for which the annotated classifier is an aspect.</param>
        public AspectAnnotation(IModelConstructionContext constructionContext, string name, Func<IClassifier, bool> classifierFilter)
            : base(constructionContext, name)
        {
            Requires.NotNull(classifierFilter, nameof(classifierFilter));

            this.IsAspectOf = classifierFilter;
        }

        /// <summary>
        /// Gets a filter function to select classifiers for which the annotated classifier is an aspect.
        /// </summary>
        /// <value>
        /// A filter function.
        /// </value>
        public Func<IClassifier, bool> IsAspectOf { get; }
    }
}