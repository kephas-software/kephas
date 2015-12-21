// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IInstanceContext.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Provides a runtime context for an instance.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Behaviors
{
    using Kephas.Model;
    using Kephas.Services;

    /// <summary>
    /// Provides a runtime context for an instance.
    /// </summary>
    public interface IInstanceContext : IContext
    {
        /// <summary>
        /// Gets or sets the model element.
        /// </summary>
        /// <remarks>
        /// The model element could be the instance's classifier or one of its properties. 
        /// For example, when evaluating the behaviors of a property, the model element contains the property definition,
        /// and when evaluating the behaviors of an instance, the model element contains the instance's classifier.
        /// </remarks>
        /// <value>
        /// The model element.
        /// </value>
        IModelElement ModelElement { get; set; }

        /// <summary>
        /// Gets or sets the instance.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        IInstance Instance { get; set; }
    }
}