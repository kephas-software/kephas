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
        /// <param name="service">The service.</param>
        /// <param name="metadata">The metadata (optional).</param>
        /// <param name="context">Context for the behavior (optional).</param>
        public ServiceBehaviorContext(IInjector injector, TContract service, object? metadata = null, IContext? context = null)
            : this(injector, null, service, metadata, context)
        {
            service = service ?? throw new ArgumentNullException(nameof(service));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceBehaviorContext{TContract}" />
        /// class.
        /// </summary>
        /// <param name="injector">The injector.</param>
        /// <param name="serviceFactory">The service export factory.</param>
        /// <param name="context">Context for the behavior (optional).</param>
        public ServiceBehaviorContext(IInjector injector, IExportFactory<TContract> serviceFactory, IContext? context = null)
            : this(injector, serviceFactory, null, GetExportMetadata(serviceFactory), context)
        {
            serviceFactory = serviceFactory ?? throw new ArgumentNullException(nameof(serviceFactory));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceBehaviorContext{TContract}" />
        /// class.
        /// </summary>
        /// <param name="injector">The injector.</param>
        /// <param name="serviceFactory">The service export factory.</param>
        /// <param name="service">The service.</param>
        /// <param name="metadata">The metadata.</param>
        /// <param name="context">Context for the behavior (optional).</param>
        private ServiceBehaviorContext(IInjector injector, IExportFactory<TContract>? serviceFactory, TContract? service, object? metadata = null, IContext? context = null)
            : base(injector)
        {
            this.ServiceFactory = serviceFactory;
            this.Service = service;
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
        public TContract? Service { get; }

        /// <summary>
        /// Gets the service factory.
        /// </summary>
        /// <value>
        /// The service factory.
        /// </value>
        public IExportFactory<TContract>? ServiceFactory { get; }

        /// <summary>
        /// Gets the service metadata.
        /// </summary>
        /// <value>
        /// The service metadata.
        /// </value>
        public object? Metadata { get; }

        /// <summary>
        /// Gets export metadata.
        /// </summary>
        /// <param name="serviceExport">The service export.</param>
        /// <returns>
        /// The export metadata.
        /// </returns>
        private static object? GetExportMetadata(IExportFactory<TContract> serviceExport)
        {
            var metadata = serviceExport.ToDynamic()[nameof(IExportFactory<TContract, AppServiceMetadata>.Metadata)];
            return metadata;
        }
    }
}