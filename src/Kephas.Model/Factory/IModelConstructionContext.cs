// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IModelConstructionContext.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the IModelConstructionContext interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Factory
{
    using Kephas.Model.Runtime.Factory;
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
    }
}