// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Classifier.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the classifier class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Elements
{
    using Kephas.Model.Construction;

    /// <summary>
    /// A class for generic classifiers. This class cannot be inherited.
    /// </summary>
    public sealed class Classifier : ClassifierBase<IClassifier>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Classifier" /> class.
        /// </summary>
        /// <param name="constructionContext">Context for the construction.</param>
        /// <param name="name">The name.</param>
        public Classifier(IModelConstructionContext constructionContext, string name)
            : base(constructionContext, name)
        {
        }
    }
}
