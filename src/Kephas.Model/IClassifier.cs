// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IClassifier.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Contract for classifiers.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model
{
    using System.Collections.Generic;

    using Kephas.Reflection;

    /// <summary>
    /// Contract for classifiers.
    /// </summary>
    public interface IClassifier : IModelElement, ITypeInfo
    {
        /// <summary>
        /// Gets the projection where the model element is defined.
        /// </summary>
        /// <value>
        /// The projection.
        /// </value>
        IModelProjection Projection { get; }

        /// <summary>
        /// Gets the classifier properties.
        /// </summary>
        /// <value>
        /// The classifier properties.
        /// </value>
        new IEnumerable<IProperty> Properties { get; }

        /// <summary>
        /// Gets a value indicating whether this classifier is a mixin.
        /// </summary>
        /// <value>
        /// <c>true</c> if this classifier is a mixin, <c>false</c> if not.
        /// </value>
        bool IsMixin { get; }

        /// <summary>
        /// Gets the base classifier.
        /// </summary>
        /// <value>
        /// The base classifier.
        /// </value>
        IClassifier BaseClassifier { get; }

        /// <summary>
        /// Gets the base mixins.
        /// </summary>
        /// <value>
        /// The base mixins.
        /// </value>
        IEnumerable<IClassifier> BaseMixins { get; }
    }
}