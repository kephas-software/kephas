// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IInstance.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Contract for instances of classifiers.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data
{
    using Kephas.Model;

    /// <summary>
    /// Contract for instances of classifiers.
    /// </summary>
    public interface IInstance
    {
        /// <summary>
        /// Gets the classifier for this instance.
        /// </summary>
        /// <returns>
        /// The classifier.
        /// </returns>
        IClassifier GetClassifier();
    }
}