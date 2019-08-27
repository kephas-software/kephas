// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AmbientServices.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Provides the global ambient services.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Kephas.Application;
    using Kephas.Collections;
    using Kephas.Composition;
    using Kephas.Composition.AttributedModel;
    using Kephas.Composition.Hosting;
    using Kephas.Configuration;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Dynamic;
    using Kephas.Internal;
    using Kephas.Logging;
    using Kephas.Reflection;
    using Kephas.Resources;
    using Kephas.Services;
    using Kephas.Services.Reflection;

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
    [ExcludeFromComposition]
    public class AmbientServices : Expando, IAmbientServices
    {
        /// <summary>
        /// The internal global instance.
        /// </summary>
        private static IAmbientServices instance;

        private readonly ConcurrentDictionary<Type, IAppServiceInfo> services = new ConcurrentDictionary<Type, IAppServiceInfo>();

        private readonly ICompositionContext asCompositionContext;

        private readonly List<IServiceSource> serviceSources = new List<IServiceSource>();

        /// <summary>
        /// Initializes a new instance of the <see cref="AmbientServices"/> class.
        /// </summary>
        public AmbientServices()
        {
            this.asCompositionContext = this.ToCompositionContext();
            var logManager = new NullLogManager();

            this.RegisterService<IAmbientServices>(this)
                .RegisterService<ICompositionContext>(this.asCompositionContext)
                .RegisterService<IConfigurationStore, DefaultConfigurationStore>(isSingleton: true)
                .RegisterService<ILogManager>(logManager)
                .RegisterService<IAssemblyLoader, DefaultAssemblyLoader>(isSingleton: true)
                .RegisterService<ITypeLoader, DefaultTypeLoader>(isSingleton: true)
                .RegisterService<IAppRuntime, DefaultAppRuntime>(isSingleton: true);

            this.serviceSources.AddRange(new[]
                                             {
                                                 new ExportFactoryServiceSource(this),
                                             });
        }

        /// <summary>
        /// Gets or sets the static instance of the ambient services.
        /// </summary>
        /// <remarks>
        /// Setting the globally available instance must be executed
        /// before getting its value for the first time, otherwise
        /// it will be initialized with the default instance.
        /// </remarks>
        /// <value>
        /// The instance.
        /// </value>
        public static IAmbientServices Instance
        {
            get => instance ?? (instance = new AmbientServices());
            set
            {
                Requires.NotNull(value, nameof(value));

                if (instance != null)
                {
                    throw new InvalidOperationException(Strings.AmbientServices_Instance_MayBeSetOnlyOnce_Exception);
                }

                instance = value;
            }
        }

        /// <summary>
        /// Gets the configuration store.
        /// </summary>
        /// <value>
        /// The configuration store.
        /// </value>
        public IConfigurationStore ConfigurationStore => this.GetService<IConfigurationStore>();

        /// <summary>
        /// Gets the composition container.
        /// </summary>
        /// <value>
        /// The composition container.
        /// </value>
        public ICompositionContext CompositionContainer => this.GetService<ICompositionContext>();

        /// <summary>
        /// Gets the assembly loader.
        /// </summary>
        /// <value>
        /// The assembly loader.
        /// </value>
        public IAssemblyLoader AssemblyLoader => this.GetService<IAssemblyLoader>();

        /// <summary>
        /// Gets the application runtime.
        /// </summary>
        /// <value>
        /// The application runtime.
        /// </value>
        public IAppRuntime AppRuntime => this.GetService<IAppRuntime>();

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
        public virtual IAmbientServices RegisterService(Type serviceType, object service)
        {
            Requires.NotNull(serviceType, nameof(serviceType));
            Requires.NotNull(service, nameof(service));

            var declaredServiceTypeInfo = serviceType.GetTypeInfo();
            var serviceTypeInfo = service.GetType().GetTypeInfo();
            if (!declaredServiceTypeInfo.IsAssignableFrom(serviceTypeInfo))
            {
                throw new InvalidOperationException(
                      string.Format(
                          Strings.AmbientServices_ServiceTypeAndServiceInstanceMismatch_Exception,
                          service.GetType(),
                          serviceType));
            }

            this.services[serviceType] = new ServiceInfo(serviceType, service);
            return this;
        }

        /// <summary>
        /// Registers the provided service.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="serviceImplementationType">The service implementation type.</param>
        /// <param name="isSingleton">Optional. Indicates whether the function should be evaluated only
        ///                           once, or each time it is called.</param>
        /// <returns>
        /// The IAmbientServices.
        /// </returns>
        public virtual IAmbientServices RegisterService(Type serviceType, Type serviceImplementationType, bool isSingleton = false)
        {
            Requires.NotNull(serviceType, nameof(serviceType));
            Requires.NotNull(serviceImplementationType, nameof(serviceImplementationType));

            var declaredServiceTypeInfo = serviceType.GetTypeInfo();
            var serviceTypeInfo = serviceImplementationType.GetTypeInfo();
            if (!declaredServiceTypeInfo.IsAssignableFrom(serviceTypeInfo))
            {
                throw new InvalidOperationException(
                    string.Format(
                        Strings.AmbientServices_ServiceTypeAndImplementationMismatch_Exception,
                        serviceImplementationType,
                        serviceType));
            }

            this.services[serviceType] = new ServiceInfo(this, serviceType, serviceImplementationType, isSingleton);
            return this;
        }

        /// <summary>
        /// Registers the provided service factory.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="serviceFactory">The service factory.</param>
        /// <param name="isSingleton">Indicates whether the function should be evaluated only once, or each time it is called.</param>
        /// <returns>
        /// The IAmbientServices.
        /// </returns>
        public virtual IAmbientServices RegisterService(
            Type serviceType,
            Func<ICompositionContext, object> serviceFactory,
            bool isSingleton = false)
        {
            Requires.NotNull(serviceType, nameof(serviceType));
            Requires.NotNull(serviceFactory, nameof(serviceFactory));

            this.services[serviceType] = new ServiceInfo(this, serviceType, serviceFactory, isSingleton);
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
            if (this.services.TryGetValue(serviceType, out var serviceRegistration))
            {
                return ((ServiceInfo)serviceRegistration).GetService(this);
            }

            foreach (var source in this.serviceSources)
            {
                if (source.IsMatch(serviceType))
                {
                    return source.GetService(serviceType);
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the application service infos in this collection.
        /// </summary>
        /// <param name="candidateTypes">List of types of the candidates.</param>
        /// <param name="registrationContext">Context for the registration.</param>
        /// <returns>
        /// An enumerator that allows foreach to be used to process the application service infos in this
        /// collection.
        /// </returns>
        public IEnumerable<(Type contractType, IAppServiceInfo appServiceInfo)> GetAppServiceInfos(IList<Type> candidateTypes, ICompositionRegistrationContext registrationContext)
        {
            // exclude the composition context from the list as it is the responsibility
            // of each composition context implementation to register itself in the DI container.
            return this.services
                .Where(kv => !ReferenceEquals(kv.Key, typeof(ICompositionContext)))
                .Select(kv => (kv.Key, this.GetAppServiceInfo(kv.Value)))
                .ToList();
        }

        private IAppServiceInfo GetAppServiceInfo(IAppServiceInfo appServiceInfo)
        {
            if (appServiceInfo is ServiceInfo serviceInfo)
            {
                return new AppServiceInfo(serviceInfo.ContractType, ctx => serviceInfo.GetService(this), serviceInfo.Lifetime);
            }

            return appServiceInfo;
        }

        private class ServiceInfo : IAppServiceInfo
        {
            private Lazy<object> lazyInstance;

            private Func<object> instanceResolver;

            public ServiceInfo(Type contractType, object instance)
            {
                this.ContractType = contractType;
                this.Instance = instance;
                this.Lifetime = AppServiceLifetime.Singleton;
                this.lazyInstance = new Lazy<object>(() => instance);
            }

            public ServiceInfo(AmbientServices ambientServices, Type contractType, Type instanceType, bool isSingleton)
            {
                this.ContractType = contractType;
                this.InstanceType = instanceType;
                this.Lifetime = isSingleton ? AppServiceLifetime.Singleton : AppServiceLifetime.Transient;
                this.lazyInstance = isSingleton ? new Lazy<object>(() => this.GetInstanceResolver(ambientServices, instanceType)()) : null;
            }

            public ServiceInfo(AmbientServices ambientServices, Type contractType, Func<ICompositionContext, object> serviceFactory, bool isSingleton)
            {
                this.ContractType = contractType;
                this.InstanceFactory = serviceFactory;
                this.Lifetime = isSingleton ? AppServiceLifetime.Singleton : AppServiceLifetime.Transient;
                this.lazyInstance = isSingleton ? new Lazy<object>(() => serviceFactory(ambientServices.asCompositionContext)) : null;
            }

            public AppServiceLifetime Lifetime { get; private set; }

            bool IAppServiceInfo.AllowMultiple { get; } = false;

            bool IAppServiceInfo.AsOpenGeneric { get; } = false;

            Type[] IAppServiceInfo.MetadataAttributes { get; }

            public Type ContractType { get; }

            public object Instance { get; internal set; }

            public Type InstanceType { get; private set; }

            public Func<ICompositionContext, object> InstanceFactory { get; }

            public object GetService(AmbientServices ambientServices)
            {
                return this.lazyInstance != null
                           ? this.lazyInstance.Value
                           : this.InstanceFactory != null
                               ? this.InstanceFactory(ambientServices.asCompositionContext)
                               : this.GetInstanceResolver(ambientServices, this.InstanceType)();
            }

            private Func<object> GetInstanceResolver(AmbientServices ambientServices, Type instanceType)
            {
                if (this.instanceResolver != null)
                {
                    return this.instanceResolver;
                }

                var ctors = instanceType.GetConstructors()
                    .Where(c => c.IsStatic == false && c.IsPublic)
                    .OrderByDescending(c => c.GetParameters().Length)
                    .ToList();
                var maxLength = -1;
                ConstructorInfo maxCtor = null;
                ParameterInfo[] maxCtorParams = null;
                foreach (var ctor in ctors)
                {
                    var ctorParams = ctor.GetParameters();
                    if (maxLength > ctorParams.Length)
                    {
                        break;
                    }

                    if (ctorParams.All(p => p.HasDefaultValue || ambientServices.IsRegistered(p.ParameterType)))
                    {
                        if (maxLength == ctorParams.Length && maxCtor != null)
                        {
                            throw new AmbiguousMatchException(string.Format(
                                Strings.AmbientServices_AmbiguousConstructors_Exception,
                                instanceType,
                                string.Join(",", ctorParams.Select(p => p.ParameterType.Name)),
                                string.Join(",", maxCtorParams.Select(p => p.ParameterType.Name))));
                        }

                        maxCtor = ctor;
                        maxCtorParams = ctorParams;
                        maxLength = ctorParams.Length;
                    }
                }

                if (maxCtor == null)
                {
                    throw new CompositionException(string.Format(
                        Strings.AmbientServices_MissingCompositionConstructor_Exception,
                        instanceType));
                }

                return this.instanceResolver = () => maxCtor.Invoke(
                           maxCtorParams.Select(
                               p => p.HasDefaultValue
                                        ? (ambientServices.GetService(p.ParameterType) ?? p.DefaultValue)
                                        : ambientServices.GetRequiredService(p.ParameterType))
                               .ToArray());
            }
        }
    }
}