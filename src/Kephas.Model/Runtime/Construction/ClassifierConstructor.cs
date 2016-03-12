// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClassifierConstructor.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the classifier constructor class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Runtime.Construction
{
    using Kephas.Dynamic;
    using Kephas.Model.Construction;
    using Kephas.Model.Elements;
    using Kephas.Services;

    /// <summary>
    /// A constructor for generic classifiers. This class cannot be inherited.
    /// </summary>
    [ProcessingPriority(Priority.Low)]
    public sealed class ClassifierConstructor : ClassifierConstructorBase<Classifier, IClassifier>
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
        protected override Classifier TryCreateModelElementCore(IModelConstructionContext constructionContext, IDynamicTypeInfo runtimeElement)
        {
            return new Classifier(constructionContext, this.TryComputeName(constructionContext, runtimeElement));
        }
    }
}