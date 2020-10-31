// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IServiceBehaviorContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IServiceBehaviorContext interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services.Behaviors
{
    using Kephas.Composition;

    /// <summary>
    /// Interface for service behavior context.
    /// </summary>
    /// <typeparam name="TServiceContract">Type of the service contract.</typeparam>
    public interface IServiceBehaviorContext<out TServiceContract> : IContext
        where TServiceContract : class
    {
        /// <summary>
        /// Gets the behavior context.
        /// </summary>
        /// <value>
        /// The behavior context.
        /// </value>
        IContext? Context { get; }

        /// <summary>
        /// Gets the service.
        /// </summary>
        /// <value>
        /// The service.
        /// </value>
        TServiceContract? Service { get; }

        /// <summary>
        /// Gets the service factory.
        /// </summary>
        /// <value>
        /// The service factory.
        /// </value>
        IExportFactory<TServiceContract>? ServiceFactory { get; }

        /// <summary>
        /// Gets the service metadata.
        /// </summary>
        /// <value>
        /// The service metadata.
        /// </value>
        object? Metadata { get; }
    }
}