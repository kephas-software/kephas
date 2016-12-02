// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InstanceContext.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   The default implementation of an instance context.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Behaviors
{
    using Kephas.Model;
    using Kephas.Services;

    /// <summary>
    /// The default implementation of an instance context.
    /// </summary>
    public class InstanceContext : ContextBase, IInstanceContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InstanceContext"/> class.
        /// </summary>
        /// <param name="ambientServices">The ambient services (optional). If not provided, <see cref="AmbientServices.Instance"/> will be considered.</param>
        public InstanceContext(IAmbientServices ambientServices = null)
            : base(ambientServices)
        {
        }

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