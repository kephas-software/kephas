// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IModelSpace.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   The model space is the root model element.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model
{
    using System.Collections.Generic;

    using Kephas.Reflection;
    using Kephas.Services;

    /// <summary>
    /// The model space is the root model element.
    /// </summary>
    public interface IModelSpace : IModelElement
    {
        /// <summary>
        /// Gets the dimensions.
        /// </summary>
        /// <value>
        /// The dimensions.
        /// </value>
        IReadOnlyList<IModelDimension> Dimensions { get; }

        /// <summary>
        /// Gets the projections.
        /// </summary>
        /// <value>
        /// The projections.
        /// </value>
        IEnumerable<IModelProjection> Projections { get; }

        /// <summary>
        /// Gets the classifiers.
        /// </summary>
        /// <value>
        /// The classifiers.
        /// </value>
        IEnumerable<IClassifier> Classifiers { get; }

        /// <summary>
        /// Tries to get the classifier associated to the provided <see cref="ITypeInfo"/>.
        /// </summary>
        /// <param name="typeInfo">The <see cref="ITypeInfo"/>.</param>
        /// <param name="findContext">Context to control the finding of classifiers.</param>
        /// <returns>
        /// The classifier, or <c>null</c> if the classifier was not found.
        /// </returns>
        IClassifier TryGetClassifier(ITypeInfo typeInfo, IContext? findContext = null);
    }
}