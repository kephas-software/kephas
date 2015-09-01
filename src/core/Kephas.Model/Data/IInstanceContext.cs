// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IInstanceContext.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Provides a runtime context for an instance.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data
{
    using Kephas.Model;
    using Kephas.Services;

    /// <summary>
    /// Provides a runtime context for an instance.
    /// </summary>
    public interface IInstanceContext : IContext
    {
        /// <summary>
        /// Gets or sets the classifier.
        /// </summary>
        /// <value>
        /// The classifier.
        /// </value>
        IClassifier Classifier { get; set; }

        /// <summary>
        /// Gets or sets the instance.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        IInstance Instance { get; set; }
    }
}