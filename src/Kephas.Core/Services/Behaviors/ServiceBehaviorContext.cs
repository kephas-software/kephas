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
    using Kephas.Composition;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Services.Composition;

    /// <summary>
    /// The default implementation of a service behavior context.
    /// </summary>
    /// <typeparam name="TServiceContract">Type of the service contract.</typeparam>
    public class ServiceBehaviorContext<TServiceContract> : Context, IServiceBehaviorContext<TServiceContract>
            where TServiceContract : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceBehaviorContext{TServiceContract}" />
        /// class.
        /// </summary>
        /// <param name="compositionContext">The composition context.</param>
        /// <param name="service">The service.</param>
        /// <param name="metadata">The metadata (optional).</param>
        /// <param name="context">Context for the behavior (optional).</param>
        public ServiceBehaviorContext(ICompositionContext compositionContext, TServiceContract service, object? metadata = null, IContext? context = null)
            : this(compositionContext, null, service, metadata, context)
        {
            Requires.NotNull(service, nameof(service));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceBehaviorContext{TServiceContract}" />
        /// class.
        /// </summary>
        /// <param name="compositionContext">The composition context.</param>
        /// <param name="serviceFactory">The service export factory.</param>
        /// <param name="context">Context for the behavior (optional).</param>
        public ServiceBehaviorContext(ICompositionContext compositionContext, IExportFactory<TServiceContract> serviceFactory, IContext? context = null)
            : this(compositionContext, serviceFactory, null, GetExportMetadata(serviceFactory), context)
        {
            Requires.NotNull(serviceFactory, nameof(serviceFactory));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceBehaviorContext{TServiceContract}" />
        /// class.
        /// </summary>
        /// <param name="compositionContext">The composition context.</param>
        /// <param name="serviceFactory">The service export factory.</param>
        /// <param name="service">The service.</param>
        /// <param name="metadata">The metadata.</param>
        /// <param name="context">Context for the behavior (optional).</param>
        private ServiceBehaviorContext(ICompositionContext compositionContext, IExportFactory<TServiceContract> serviceFactory, TServiceContract service, object? metadata = null, IContext? context = null)
            : base(compositionContext)
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
        public TServiceContract Service { get; }

        /// <summary>
        /// Gets the service factory.
        /// </summary>
        /// <value>
        /// The service factory.
        /// </value>
        public IExportFactory<TServiceContract> ServiceFactory { get; }

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
        private static object? GetExportMetadata(IExportFactory<TServiceContract> serviceExport)
        {
            var metadata = serviceExport.ToExpando()![nameof(IExportFactory<TServiceContract, AppServiceMetadata>.Metadata)];
            return metadata;
        }
    }
}