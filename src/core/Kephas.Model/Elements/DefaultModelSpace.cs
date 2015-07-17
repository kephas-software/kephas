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

    using Kephas.Model.Elements.Construction;
    using Kephas.Model.Elements.Construction.Internal;

    /// <summary>
    /// The default implementation of the model space.
    /// </summary>
    public class DefaultModelSpace : ModelElementBase<IModelSpace, IModelSpaceInfo>, IModelSpace, IModelSpaceConstructor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultModelSpace"/> class.
        /// </summary>
        /// <param name="elementInfo">The element information.</param>
        public DefaultModelSpace(IModelSpaceInfo elementInfo)
            : base(elementInfo, null)
        {
        }

        /// <summary>
        /// Gets the model space.
        /// </summary>
        /// <value>
        /// The model space.
        /// </value>
        public override IModelSpace ModelSpace
        {
            get { return this; }
        }

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