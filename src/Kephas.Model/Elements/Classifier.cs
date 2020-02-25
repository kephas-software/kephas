// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Classifier.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the classifier class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#nullable enable

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
