// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceBehaviorContext.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   The default implementation of a service behavior context.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services.Behavior
{
    using System.Diagnostics.Contracts;

    /// <summary>
    /// The default implementation of a service behavior context.
    /// </summary>
    /// <typeparam name="TServiceContract">Type of the service contract.</typeparam>
    public class ServiceBehaviorContext<TServiceContract> : ContextBase, IServiceBehaviorContext<TServiceContract>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceBehaviorContext{TServiceContract}" /> class.
        /// </summary>
        /// <param name="service">The service.</param>
        public ServiceBehaviorContext(TServiceContract service)
        {
            Contract.Requires(service != null);

            this.Service = service;
        }

        /// <summary>
        /// Gets or sets the service.
        /// </summary>
        /// <value>
        /// The service.
        /// </value>
        public TServiceContract Service { get; set; }
    }
}