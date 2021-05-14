// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClassifierConstructor.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the classifier constructor class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Runtime.Construction
{
    using Kephas.Model.Construction;
    using Kephas.Model.Elements;
    using Kephas.Runtime;
    using Kephas.Services;

    /// <summary>
    /// A constructor for generic classifiers. This class cannot be inherited.
    /// </summary>
    [ProcessingPriority(Priority.Low)]
    public sealed class ClassifierConstructor : ClassifierConstructorBase<Classifier, IClassifier>
    {
        /// <summary>
        /// Determines whether a model element can be created for the provided runtime element.
        /// </summary>
        /// <param name="constructionContext">Context for the construction.</param>
        /// <param name="runtimeElement">The runtime element.</param>
        /// <returns>
        /// <c>true</c>true if a model element can be created, <c>false</c> if not.
        /// </returns>
        protected override bool CanCreateModelElement(IModelConstructionContext constructionContext, IRuntimeTypeInfo runtimeElement)
        {
            // fallback everything to classifier.
            return true;
        }

        /// <summary>
        /// Core implementation of trying to get the element information.
        /// </summary>
        /// <param name="constructionContext">Context for the construction.</param>
        /// <param name="runtimeElement">The runtime element.</param>
        /// <returns>
        /// A new element information based on the provided runtime element information, or <c>null</c>
        /// if the runtime element information is not supported.
        /// </returns>
        protected override Classifier? TryCreateModelElementCore(IModelConstructionContext constructionContext, IRuntimeTypeInfo runtimeElement)
        {
            return new Classifier(constructionContext, this.TryComputeName(runtimeElement, constructionContext));
        }
    }
}