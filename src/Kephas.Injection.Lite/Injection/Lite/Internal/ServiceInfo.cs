// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceInfo.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the service information class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Injection.Lite.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Kephas.Reflection;
    using Kephas.Resources;
    using Kephas.Services;
    using Kephas.Services.Reflection;

    /// <summary>
    /// Information about the service.
    /// </summary>
    internal class ServiceInfo : IServiceInfo, IDisposable
    {
        private readonly LazyFactory lazyFactory;

        private Func<object>? instanceResolver;
        private readonly WeakReference<IAppServiceRegistry> weakServiceRegistry;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceInfo"/> class.
        /// </summary>
        /// <param name="serviceRegistry">The service registry.</param>
        /// <param name="contractType">Type of the contract.</param>
        /// <param name="instance">The instance.</param>
        public ServiceInfo(IAppServiceRegistry serviceRegistry, Type contractType, object instance)
            : this(serviceRegistry)
        {
            this.ContractType = contractType;
            this.InstancingStrategy = instance;
            this.Lifetime = AppServiceLifetime.Singleton;
            this.lazyFactory = new LazyValue(instance);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceInfo"/> class.
        /// </summary>
        /// <param name="serviceRegistry">The service registry.</param>
        /// <param name="contractType">Type of the contract.</param>
        /// <param name="instanceType">Type of the instance.</param>
        /// <param name="isSingleton">True if is singleton, false if not.</param>
        public ServiceInfo(IAppServiceRegistry serviceRegistry, Type contractType, Type instanceType, bool isSingleton)
            : this(serviceRegistry)
        {
            this.ContractType = contractType;
            this.InstancingStrategy = instanceType;
            this.Lifetime = isSingleton ? AppServiceLifetime.Singleton : AppServiceLifetime.Transient;
            this.lazyFactory = isSingleton
                                   ? new LazyValue(injector => this.GetInstanceResolver(serviceRegistry, instanceType)(), contractType)
                                   : new LazyFactory(injector => this.GetInstanceResolver(serviceRegistry, instanceType)(), contractType);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceInfo"/> class.
        /// </summary>
        /// <param name="serviceRegistry">The service registry.</param>
        /// <param name="contractType">Type of the contract.</param>
        /// <param name="serviceFactory">The service factory.</param>
        /// <param name="isSingleton">True if is singleton, false if not.</param>
        public ServiceInfo(IAppServiceRegistry serviceRegistry, Type contractType, Func<IServiceProvider?, object> serviceFactory, bool isSingleton)
            : this(serviceRegistry)
        {
            this.ContractType = contractType;
            this.InstancingStrategy = serviceFactory;
            this.Lifetime = isSingleton ? AppServiceLifetime.Singleton : AppServiceLifetime.Transient;
            this.lazyFactory = isSingleton
                                   ? new LazyValue(serviceFactory, contractType)
                                   : new LazyFactory(serviceFactory, contractType);
        }

        private ServiceInfo(IAppServiceRegistry serviceRegistry)
        {
            this.weakServiceRegistry = new WeakReference<IAppServiceRegistry>(serviceRegistry);
        }

        public AppServiceLifetime Lifetime { get; }

        public bool AllowMultiple { get; internal set; } = false;

        bool IAppServiceInfo.AsOpenGeneric => this.ContractType.IsGenericType && this.ContractType.IsGenericTypeDefinition;

        public Type? ContractType { get; }

        public Type? MetadataType { get; set; }

        public object? InstancingStrategy { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the service is externally owned and should not be disposed by the container.
        /// </summary>
        public bool IsExternallyOwned { get; internal set; }

        public IDictionary<string, object?>? Metadata { get; internal set; }

        /// <summary>
        /// Adds the metadata with the provided name and value.
        /// </summary>
        /// <param name="name">The metadata name.</param>
        /// <param name="value">The metadata value.</param>
        /// <returns>This <see cref="AppServiceInfo"/>.</returns>
        public IAppServiceInfo AddMetadata(string name, object? value)
        {
            ((this.Metadata ??= new Dictionary<string, object?>())!)[name] = value;
            return this;
        }

        /// <summary>
        /// Makes a generic service information with closed generic types.
        /// </summary>
        /// <exception cref="NotSupportedException">Thrown when the requested operation is not supported.</exception>
        /// <param name="genericArgs">The generic arguments.</param>
        /// <returns>
        /// An IServiceInfo.
        /// </returns>
        public IServiceInfo MakeGenericServiceInfo(Type[] genericArgs)
        {
            if (!this.ContractType.IsGenericTypeDefinition)
            {
                throw new NotSupportedException($"Only open generic registrations may be constructed, {this} does not support this operation.");
            }

            IAppServiceInfo self = this;
            if (self.InstanceType == null)
            {
                throw new NotSupportedException($"Only open generic registrations may be constructed, {this} does not support this operation.");
            }

            var closedContractType = this.ContractType.MakeGenericType(genericArgs);
            var closedInstanceType = self.InstanceType.MakeGenericType(genericArgs);

            if (!this.weakServiceRegistry.TryGetTarget(out var serviceProvider))
            {
                throw new ObjectDisposedException("The service provider is disposed.");
            }

            var closedServiceInfo = new ServiceInfo(serviceProvider, closedContractType, closedInstanceType, this.IsSingleton())
            {
                AllowMultiple = this.AllowMultiple,
                IsExternallyOwned = this.IsExternallyOwned,
            };

            return closedServiceInfo;
        }

        public AppServiceInfo ToAppServiceInfo()
        {
            return new AppServiceInfo(this.ContractType!, this.GetServiceCore, this.Lifetime)
            {
                AllowMultiple = this.AllowMultiple,
                IsExternallyOwned = this.IsExternallyOwned,
            };
        }

        public bool IsMatch(Type contractType) => contractType == this.ContractType;

        public object GetService(System.IServiceProvider serviceProvider, Type contractType) => this.GetServiceCore(serviceProvider as IServiceProvider);

        protected virtual object GetServiceCore(IServiceProvider? injector) => this.lazyFactory.GetValue(injector);

        public void Dispose()
        {
            if (!this.IsExternallyOwned)
            {
                this.lazyFactory.Dispose();
            }
        }

        private Func<object?> GetInstanceResolver(IAppServiceRegistry serviceRegistry, Type instanceType)
        {
            if (this.instanceResolver != null)
            {
                return this.instanceResolver;
            }

            var (ctor, ctorParams) = GetConstructorInfo(serviceRegistry, instanceType);

            return this.instanceResolver = () => ctor.Invoke(
                       ctorParams.Select(
                           p => p.HasDefaultValue
                                    ? serviceRegistry.GetService(p.ParameterType) ?? p.DefaultValue
                                    : serviceRegistry.GetRequiredService(p.ParameterType))
                           .ToArray());
        }

        private static (ConstructorInfo maxCtor, ParameterInfo[] maxCtorParams) GetConstructorInfo(
            IAppServiceRegistry serviceRegistry,
            Type instanceType)
        {
            var unresolvedParams = new List<ParameterInfo>();
            var ctors = instanceType.GetConstructors().Where(c => c.IsStatic == false && c.IsPublic)
                .OrderByDescending(c => c.GetParameters().Length)
                .ToList();
            var maxLength = -1;
            ConstructorInfo? maxCtor = null;
            ParameterInfo[]? maxCtorParams = null;
            foreach (var ctor in ctors)
            {
                var ctorParams = ctor.GetParameters();
                if (maxLength > ctorParams.Length)
                {
                    break;
                }

                var unresolvedCtorParams = ctorParams.Where(p => !p.HasDefaultValue && !serviceRegistry.IsRegistered(p.ParameterType)).ToList();

                if (unresolvedCtorParams.Count == 0)
                {
                    if (maxLength == ctorParams.Length && maxCtor != null)
                    {
                        throw new AmbiguousMatchException(
                            string.Format(
                                AbstractionStrings.AmbientServices_AmbiguousConstructors_Exception,
                                instanceType,
                                string.Join(",", ctorParams.Select(p => p.ParameterType.Name)),
                                string.Join(",", maxCtorParams.Select(p => p.ParameterType.Name))));
                    }

                    maxCtor = ctor;
                    maxCtorParams = ctorParams;
                    maxLength = ctorParams.Length;
                }
                else
                {
                    unresolvedParams.AddRange(unresolvedCtorParams);
                }
            }

            if (maxCtor == null)
            {
                throw new InjectionException(
                    string.Format(AbstractionStrings.AmbientServices_MissingCompositionConstructor_Exception, instanceType, string.Join(", ", unresolvedParams)));
            }

            return (maxCtor, maxCtorParams);
        }

        private class LazyValue : LazyFactory
        {
            private object? value;

            public LazyValue(Func<IServiceProvider?, object> factory, Type serviceType)
                : base(factory, serviceType)
            {
            }

            public LazyValue(object value)
                : base(null, null)
            {
                this.value = value;
            }

            public override object GetValue(IServiceProvider? injector)
            {
                if (this.value != null)
                {
                    return this.value;
                }

                lock (this.factory)
                {
                    if (this.value != null)
                    {
                        return this.value;
                    }

                    return this.value = base.GetValue(injector);
                }
            }

            public override void Dispose()
            {
                (this.value as IDisposable)?.Dispose();
                this.value = null;
                base.Dispose();
            }
        }

        private class LazyFactory : IDisposable
        {
            [ThreadStatic]
            private static List<LazyFactory>? isProducing;

            protected readonly Func<IServiceProvider?, object> factory;

            private readonly Type serviceType;

            public LazyFactory(Func<IServiceProvider?, object> factory, Type serviceType)
            {
                this.factory = factory;
                this.serviceType = serviceType;
            }

            public virtual void Dispose()
            {
            }

            public virtual object GetValue(IServiceProvider? injector)
            {
                // at one time, a single value may be produced per thread
                // otherwise it means that it occured a circular dependency
                if (isProducing == null)
                {
                    isProducing = new List<LazyFactory>();
                }
                else if (isProducing.Contains(this))
                {
                    throw new CircularDependencyException(string.Format(AbstractionStrings.LazyFactory_CircularDependency_Exception, serviceType));
                }

                isProducing.Add(this);
                try
                {
                    var value = this.factory(injector);
                    return value;
                }
                finally
                {
                    isProducing.Remove(this);
                }
            }
        }
    }
}