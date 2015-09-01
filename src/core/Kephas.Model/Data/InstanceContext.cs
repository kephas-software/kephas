// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InstanceContext.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   The default implementation of an instance context.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data
{
    using Kephas.Model;
    using Kephas.Services;

    /// <summary>
    /// The default implementation of an instance context.
    /// </summary>
    public class InstanceContext : ContextBase, IInstanceContext
    {
        /// <summary>
        /// Gets or sets the model element.
        /// </summary>
        /// <value>
        /// The model element.
        /// </value>
        public IModelElement ModelElement { get; set; }

        /// <summary>
        /// Gets or sets the instance.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public IInstance Instance { get; set; }
    }
}