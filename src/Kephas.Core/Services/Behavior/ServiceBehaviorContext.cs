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
    using Kephas.Application;
    using Kephas.Diagnostics.Contracts;

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
        /// <param name="service">The service.</param>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="appContext">Context for the application (optional).</param>
        public ServiceBehaviorContext(TServiceContract service, IAmbientServices ambientServices, IAppContext appContext = null)
            : base(ambientServices)
        {
            Requires.NotNull(service, nameof(service));

            this.Service = service;
            this.AppContext = appContext;
        }

        /// <summary>
        /// Gets the application context.
        /// </summary>
        /// <value>
        /// The application context.
        /// </value>
        public IAppContext AppContext { get; }

        /// <summary>
        /// Gets the service.
        /// </summary>
        /// <value>
        /// The service.
        /// </value>
        public TServiceContract Service { get; }
    }
}