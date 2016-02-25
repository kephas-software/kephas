// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelConstructionContext.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the model construction context class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Factory
{
    using Kephas.Model.Runtime.Factory;
    using Kephas.Services;

    /// <summary>
    /// A model construction context.
    /// </summary>
    public class ModelConstructionContext : ContextBase, IModelConstructionContext
    {
        /// <summary>
        /// Gets the model space.
        /// </summary>
        /// <value>
        /// The model space.
        /// </value>
        public IModelSpace ModelSpace { get; internal set; }

        /// <summary>
        /// Gets the model element factory.
        /// </summary>
        /// <value>
        /// The model element factory.
        /// </value>
        public IRuntimeModelElementFactory RuntimeModelElementFactory { get; set; }
    }
}