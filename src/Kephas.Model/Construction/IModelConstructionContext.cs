// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IModelConstructionContext.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
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
        /// Gets or sets the constructed classifiers.
        /// </summary>
        /// <remarks>
        /// This is not intended to be modified outside the framework.
        /// </remarks>
        /// <value>
        /// The constructed classifiers.
        /// </value>
        IEnumerable<IClassifier> ConstructedClassifiers { get; set; }

        /// <summary>
        /// Gets or sets a function to try to get a model element based on a native element information.
        /// </summary>
        /// <value>
        /// A function for getting a model element.
        /// </value>
        Func<IElementInfo, IElementInfo> TryGetElementInfo { get; set; }
    }
}