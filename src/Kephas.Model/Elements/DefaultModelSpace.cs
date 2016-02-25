// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultModelSpace.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   The default implementation of the model space.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Elements
{
    using System.Collections.Generic;

    using Kephas.Model.Factory;

    /// <summary>
    /// The default implementation of the model space.
    /// </summary>
    public class DefaultModelSpace : ModelElementBase<IModelSpace>, IModelSpace
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultModelSpace"/> class.
        /// </summary>
        /// <param name="constructionContext">Context for the construction.</param>
        public DefaultModelSpace(IModelConstructionContext constructionContext)
            : base(constructionContext, string.Empty)
        {
        }

        /// <summary>
        /// Gets the model space.
        /// </summary>
        /// <value>
        /// The model space.
        /// </value>
        public override IModelSpace ModelSpace => this;

        /// <summary>
        /// Gets the dimensions.
        /// </summary>
        /// <value>
        /// The dimensions.
        /// </value>
        public IModelDimension[] Dimensions { get; private set; }

        /// <summary>
        /// Gets the projections.
        /// </summary>
        /// <value>
        /// The projections.
        /// </value>
        public IEnumerable<IModelProjection> Projections { get; private set; }

        /// <summary>
        /// Gets the classifiers.
        /// </summary>
        /// <value>
        /// The classifiers.
        /// </value>
        public IEnumerable<IClassifier> Classifiers { get; private set; }
    }
}