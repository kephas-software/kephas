// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceBehaviorContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   The default implementation of a service behavior context.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services.Behaviors
{
    using System;

    using Kephas.Injection;

    /// <summary>
    /// The default implementation of a service behavior context.
    /// </summary>
    /// <typeparam name="TContract">Type of the service contract.</typeparam>
    /// <typeparam name="TMetadata">Type of the service metadata.</typeparam>
    public class ServiceBehaviorContext<TContract, TMetadata> : Context, IServiceBehaviorContext<TContract, TMetadata>
        where TContract : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceBehaviorContext{TContract, TMetadata}" />
        /// class.
        /// </summary>
        /// <param name="injector">The injector.</param>
        /// <param name="serviceFactory">The service factory.</param>
        /// <param name="metadata">Optional. The metadata.</param>
        public ServiceBehaviorContext(IInjector injector, Func<TContract> serviceFactory, TMetadata metadata)
            : base(injector)
        {
            this.ServiceFactory = serviceFactory ?? throw new ArgumentNullException(nameof(serviceFactory));
            this.Metadata = metadata;
        }

        /// <summary>
        /// Gets the service.
        /// </summary>
        /// <value>
        /// The service.
        /// </value>
        public TContract Service => this.ServiceFactory();

        /// <summary>
        /// Gets the service factory.
        /// </summary>
        /// <value>
        /// The service factory.
        /// </value>
        public Func<TContract> ServiceFactory { get; }

        /// <summary>
        /// Gets the service metadata.
        /// </summary>
        /// <value>
        /// The service metadata.
        /// </value>
        public TMetadata Metadata { get; }
    }
}