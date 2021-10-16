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
    public class ServiceBehaviorContext<TContract> : Context, IServiceBehaviorContext<TContract>
        where TContract : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceBehaviorContext{TContract}" />
        /// class.
        /// </summary>
        /// <param name="injector">The injector.</param>
        /// <param name="serviceFactory">The service factory.</param>
        /// <param name="metadata">Optional. The metadata.</param>
        /// <param name="context">Optional. Context for the behavior.</param>
        public ServiceBehaviorContext(IInjector injector, Func<TContract> serviceFactory, object? metadata = null, IContext? context = null)
            : base(injector)
        {
            this.ServiceFactory = serviceFactory ?? throw new ArgumentNullException(nameof(serviceFactory));
            this.Metadata = metadata;
            this.Context = context;
        }

        /// <summary>
        /// Gets the behavior context.
        /// </summary>
        /// <value>
        /// The behavior context.
        /// </value>
        public IContext? Context { get; }

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
        public object? Metadata { get; }
    }
}