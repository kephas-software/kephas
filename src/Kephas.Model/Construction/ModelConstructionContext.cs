// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelConstructionContext.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the model construction context class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Construction
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Model.Runtime.Construction;
    using Kephas.Reflection;
    using Kephas.Services;

    /// <summary>
    /// A model construction context.
    /// </summary>
    public class ModelConstructionContext : Context, IModelConstructionContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModelConstructionContext"/> class.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        public ModelConstructionContext(IAmbientServices ambientServices)
            : base(ambientServices)
        {
            Requires.NotNull(ambientServices, nameof(ambientServices));
        }

        /// <summary>
        /// Gets or sets the model space.
        /// </summary>
        /// <value>
        /// The model space.
        /// </value>
        public IModelSpace ModelSpace { get; set; }

        /// <summary>
        /// Gets or sets the model element factory.
        /// </summary>
        /// <value>
        /// The model element factory.
        /// </value>
        public IRuntimeModelElementFactory RuntimeModelElementFactory { get; set; }

        /// <summary>
        /// Gets or sets the element infos.
        /// </summary>
        /// <value>
        /// The element infos.
        /// </value>
        public IEnumerable<IElementInfo> ElementInfos { get; set; }

        /// <summary>
        /// Gets or sets the constructed classifiers.
        /// </summary>
        /// <value>
        /// The constructed classifiers.
        /// </value>
        public IEnumerable<IClassifier> ConstructedClassifiers { get; set; }

        /// <summary>
        /// Gets or sets a function used to compare two classifier from the dependency perspective.
        /// A classifier is "greater" than another classifier if the other one is a part of it.
        /// Otherwise they are not comparable.
        /// </summary>
        /// <returns>
        /// A function used to compare the two classifiers.
        /// </returns>
        public Func<IClassifier, IClassifier, int?> ClassifierDependencyCompararer { get; set; }
    }
}