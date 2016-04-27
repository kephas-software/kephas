// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AmbientServices.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Provides the global ambient services.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas
{
    using System;
    using System.Collections.Concurrent;

    using Kephas.Application;
    using Kephas.Collections;
    using Kephas.Composition;
    using Kephas.Composition.Hosting;
    using Kephas.Configuration;
    using Kephas.Dynamic;
    using Kephas.Logging;
    using Kephas.Reflection;

    /// <summary>
    /// Provides the global ambient services.
    /// </summary>
    /// <remarks>
    /// It is a recommended practice to not use global services, instead get the services
    /// using the composition (the classical example is the unit testing, where the classes 
    /// should be sandboxed as much as possible). However, there may be cases when this cannot be avoided,
    /// such as static classes or classes which get instantiated outside of the developer's control 
    /// (like in the case of the entities instatiated by the ORMs). Those are cases where the
    /// <see cref="AmbientServices"/> can be safely used.
    /// </remarks>
    public class AmbientServices : Expando, IAmbientServices
    {
        /// <summary>
        /// The services.
        /// </summary>
        private readonly ConcurrentDictionary<Type, object> services = new ConcurrentDictionary<Type, object>();

        /// <summary>
        /// Initializes static members of the <see cref="AmbientServices"/> class.
        /// </summary>
        static AmbientServices()
        {
            Instance = new AmbientServices();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AmbientServices"/> class.
        /// </summary>
        internal AmbientServices()
        {
            this.RegisterService<IAppEnvironment>(new DefaultAppEnvironment())
                .RegisterService<ILogManager>(new NullLogManager())
                .RegisterService<IConfigurationManager>(new NullConfigurationManager())
                .RegisterService<ICompositionContext>(new NullCompositionContainer())
                .RegisterService<IAssemblyLoader>(new DefaultAssemblyLoader());
        }

        /// <summary>
        /// Gets the static instance of the ambient services.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public static IAmbientServices Instance { get; }

        /// <summary>
        /// Gets the composition container.
        /// </summary>
        /// <value>
        /// The composition container.
        /// </value>
        public ICompositionContext CompositionContainer => this.GetService<ICompositionContext>();

        /// <summary>
        /// Gets the application environment.
        /// </summary>
        /// <value>
        /// The application environment.
        /// </value>
        public IAppEnvironment AppEnvironment => this.GetService<IAppEnvironment>();

        /// <summary>
        /// Gets the application configuration manager.
        /// </summary>
        /// <value>
        /// The application configuration manager.
        /// </value>
        public IConfigurationManager ConfigurationManager => this.GetService<IConfigurationManager>();

        /// <summary>
        /// Gets the log manager.
        /// </summary>
        /// <value>
        /// The log manager.
        /// </value>
        public ILogManager LogManager => this.GetService<ILogManager>();

        /// <summary>
        /// Registers the provided service.
        /// </summary>
        /// <typeparam name="TService">Type of the service.</typeparam>
        /// <param name="service">The service.</param>
        /// <returns>
        /// The IAmbientServices.
        /// </returns>
        public IAmbientServices RegisterService<TService>(TService service)
            where TService : class
        {
            this.services[typeof(TService)] = service;

            return this;
        }

        /// <summary>
        /// Gets the service with the provided type.
        /// </summary>
        /// <typeparam name="TService">Type of the service.</typeparam>
        /// <returns>
        /// A service object of type <typeparamref name="TService"/>.-or- <c>null</c> if there is no service object of type <typeparamref name="TService"/>.
        /// </returns>
        public TService GetService<TService>()
            where TService : class
        {
            return (TService)this.GetService(typeof(TService));
        }

        /// <summary>
        /// Gets the service object of the specified type.
        /// </summary>
        /// <returns>
        /// A service object of type <paramref name="serviceType"/>.-or- null if there is no service object of type <paramref name="serviceType"/>.
        /// </returns>
        /// <param name="serviceType">An object that specifies the type of service object to get. </param>
        public object GetService(Type serviceType)
        {
            var service = this.services.TryGetValue(serviceType);
            return service;
        }
    }
}