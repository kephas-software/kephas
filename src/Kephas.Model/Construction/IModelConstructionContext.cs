// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IModelConstructionContext.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the IModelConstructionContext interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Construction
{
    using System;
    using System.Collections.Generic;

    using Kephas.Model.Runtime.Construction;
    using Kephas.Reflection;
    using Kephas.Services;

    /// <summary>
    /// Contract for model construction contexts.
    /// </summary>
    public interface IModelConstructionContext : IContext
    {
        /// <summary>
        /// Gets the model space.
        /// </summary>
        /// <value>
        /// The model space.
        /// </value>
        IModelSpace ModelSpace { get; }

        /// <summary>
        /// Gets or sets the model element factory.
        /// </summary>
        /// <value>
        /// The model element factory.
        /// </value>
        IRuntimeModelElementFactory RuntimeModelElementFactory { get; set; }

        /// <summary>
        /// Gets the element infos.
        /// </summary>
        /// <value>
        /// The element infos.
        /// </value>
        IEnumerable<IElementInfo> ElementInfos { get; }

        /// <summary>
        /// Gets the constructed classifiers.
        /// </summary>
        /// <value>
        /// The constructed classifiers.
        /// </value>
        IEnumerable<IClassifier> ConstructedClassifiers { get; }

        /// <summary>
        /// Gets or sets a function used to compare two classifier from the dependency perspective.
        /// A classifier is "greater" than another classifier if the other one is a part of it.
        /// Otherwise they are not comparable.
        /// </summary>
        /// <returns>
        /// A function used to compare the two classifiers.
        /// </returns>
        Func<IClassifier, IClassifier, int?> ClassifierDependencyCompararer { get; set; }
    }
}