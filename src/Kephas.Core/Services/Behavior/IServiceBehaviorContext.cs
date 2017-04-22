// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IServiceBehaviorContext.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the IServiceBehaviorContext interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services.Behavior
{
    using Kephas.Application;

    /// <summary>
    /// Interface for service behavior context.
    /// </summary>
    /// <typeparam name="TServiceContract">Type of the service contract.</typeparam>
    public interface IServiceBehaviorContext<out TServiceContract> : IContext
    {
        /// <summary>
        /// Gets the application context.
        /// </summary>
        /// <value>
        /// The application context.
        /// </value>
        IAppContext AppContext { get; }

        /// <summary>
        /// Gets the service.
        /// </summary>
        /// <value>
        /// The service.
        /// </value>
        TServiceContract Service { get; }
    }
}