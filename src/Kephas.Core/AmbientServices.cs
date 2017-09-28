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
    using System.Reflection;

    using Kephas.Application;
    using Kephas.Application.Configuration;
    using Kephas.Collections;
    using Kephas.Composition;
    using Kephas.Composition.Hosting;
    using Kephas.Configuration;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Dynamic;
    using Kephas.Logging;
    using Kephas.Reflection;
    using Kephas.Resources;

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
        private readonly ConcurrentDictionary<Type, ServiceRegistration> services = new ConcurrentDictionary<Type, ServiceRegistration>();

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
        public AmbientServices()
        {
            this.RegisterService<IAmbientServices>(this)
                .RegisterService<IAppRuntime>(new NullAppRuntime())
                .RegisterService<ILogManager>(new NullLogManager())
                .RegisterService<IAppConfiguration>(new NullAppConfiguration())
                .RegisterService<ICompositionContext>(new NullCompositionContainer())
                .RegisterService<IAssemblyLoader>(new DefaultAssemblyLoader())
                .RegisterService<ITypeLoader>(new DefaultTypeLoader(this));
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
        /// Gets the application runtime.
        /// </summary>
        /// <value>
        /// The application runtime.
        /// </value>
        public IAppRuntime AppRuntime => this.GetService<IAppRuntime>();

        /// <summary>
        /// Gets the application configuration.
        /// </summary>
        /// <value>
        /// The application configuration.
        /// </value>
        public IAppConfiguration AppConfiguration => this.GetService<IAppConfiguration>();

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
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="service">The service.</param>
        /// <returns>
        /// The IAmbientServices.
        /// </returns>
        public IAmbientServices RegisterService(Type serviceType, object service)
        {
            Requires.NotNull(serviceType, nameof(serviceType));
            Requires.NotNull(service, nameof(service));

            var declaredServiceTypeInfo = IntrospectionExtensions.GetTypeInfo(serviceType);
            var serviceTypeInfo = IntrospectionExtensions.GetTypeInfo(service.GetType());
            if (!declaredServiceTypeInfo.IsAssignableFrom(serviceTypeInfo))
            {
                throw new InvalidOperationException(
                      string.Format(
                          Strings.AmbientServices_ServiceTypeAndServiceInstanceMismatch_Exception,
                          service.GetType(),
                          serviceType));
            }

            this.services[serviceType] = new ServiceRegistration
            {
                ServiceContract = serviceType,
                ServiceFactory = () => service
            };

            return this;
        }

        /// <summary>
        /// Registers the provided service factory.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="serviceFactory">The service factory.</param>
        /// <returns>
        /// The IAmbientServices.
        /// </returns>
        public IAmbientServices RegisterService(Type serviceType, Func<object> serviceFactory)
        {
            Requires.NotNull(serviceType, nameof(serviceType));
            Requires.NotNull(serviceFactory, nameof(serviceFactory));

            this.services[serviceType] = new ServiceRegistration
            {
                ServiceContract = serviceType,
                ServiceFactory = serviceFactory
            };
            return this;
        }

        /// <summary>
        /// Gets a value indicating whether the service with the provided contract is registered.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <returns>
        /// <c>true</c> if the service is registered, <c>false</c> if not.
        /// </returns>
        public bool IsRegistered(Type serviceType)
        {
            return serviceType != null && this.services.ContainsKey(serviceType);
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
            var serviceRegistration = this.services.TryGetValue(serviceType);
            return serviceRegistration?.ServiceFactory();
        }

        /// <summary>
        /// A service registration.
        /// </summary>
        private class ServiceRegistration
        {
            /// <summary>
            /// Gets or sets the service contract.
            /// </summary>
            /// <value>
            /// The service contract.
            /// </value>
            public Type ServiceContract { get; set; }

            /// <summary>
            /// Gets or sets the service factory.
            /// </summary>
            /// <value>
            /// The service factory.
            /// </value>
            public Func<object> ServiceFactory { get; set; }
        }
    }
}