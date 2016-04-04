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
    using System.Collections.Generic;

    using Kephas.Model.Runtime.Construction;
    using Kephas.Reflection;
    using Kephas.Services;

    /// <summary>
    /// A model construction context.
    /// </summary>
    public class ModelConstructionContext : ContextBase, IModelConstructionContext
    {
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
    }
}