// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceInfo.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the service information class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Lite.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Kephas.Composition;
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

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceInfo"/> class.
        /// </summary>
        /// <param name="contractType">Type of the contract.</param>
        /// <param name="instance">The instance.</param>
        public ServiceInfo(Type contractType, object instance)
        {
            this.ContractType = contractType;
            this.Instance = instance;
            this.Lifetime = AppServiceLifetime.Singleton;
            this.lazyFactory = new LazyValue(instance);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceInfo"/> class.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="contractType">Type of the contract.</param>
        /// <param name="instanceType">Type of the instance.</param>
        /// <param name="isSingleton">True if is singleton, false if not.</param>
        public ServiceInfo(IAmbientServices ambientServices, Type contractType, Type instanceType, bool isSingleton)
        {
            this.ContractType = contractType;
            this.InstanceType = instanceType;
            this.Lifetime = isSingleton ? AppServiceLifetime.Singleton : AppServiceLifetime.Transient;
            this.lazyFactory = isSingleton
                                   ? new LazyValue(() => this.GetInstanceResolver(ambientServices, instanceType)(), contractType)
                                   : new LazyFactory(() => this.GetInstanceResolver(ambientServices, instanceType)(), contractType);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceInfo"/> class.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="contractType">Type of the contract.</param>
        /// <param name="serviceFactory">The service factory.</param>
        /// <param name="isSingleton">True if is singleton, false if not.</param>
        public ServiceInfo(IAmbientServices ambientServices, Type contractType, Func<IInjector, object> serviceFactory, bool isSingleton)
        {
            this.ContractType = contractType;
            this.InstanceFactory = serviceFactory;
            this.Lifetime = isSingleton ? AppServiceLifetime.Singleton : AppServiceLifetime.Transient;
            var compositionContext = ambientServices.AsInjector();
            this.lazyFactory = isSingleton
                                   ? new LazyValue(() => serviceFactory(compositionContext), contractType)
                                   : new LazyFactory(() => serviceFactory(compositionContext), contractType);
        }

        public AppServiceLifetime Lifetime { get; }

        public bool AllowMultiple { get; internal set; } = false;

        bool IAppServiceInfo.AsOpenGeneric => this.ContractType.IsGenericType && this.ContractType.IsGenericTypeDefinition;

        Type[] IAppServiceInfo.MetadataAttributes { get; } = Array.Empty<Type>();

        public Type ContractType { get; }

        public Type ServiceType { get; internal set; }

        public object? Instance { get; internal set; }

        public bool ExternallyOwned { get; internal set; }

        public Type? InstanceType { get; }

        public Func<IInjector, object>? InstanceFactory { get; }

        /// <summary>
        /// Makes a generic service information with closed generic types.
        /// </summary>
        /// <exception cref="NotSupportedException">Thrown when the requested operation is not supported.</exception>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="genericArgs">The generic arguments.</param>
        /// <returns>
        /// An IServiceInfo.
        /// </returns>
        public IServiceInfo MakeGenericServiceInfo(IAmbientServices ambientServices, Type[] genericArgs)
        {
            if (!this.ContractType.IsGenericTypeDefinition)
            {
                throw new NotSupportedException($"Only open generic registrations may be constructed, {this} does not support this operation.");
            }

            if (this.InstanceType == null)
            {
                throw new NotSupportedException($"Only open generic registrations may be constructed, {this} does not support this operation.");
            }

            var closedContractType = this.ContractType.MakeGenericType(genericArgs);
            var closedServiceType = this.ServiceType?.MakeGenericType(genericArgs);
            var closedInstanceType = this.InstanceType.MakeGenericType(genericArgs);

            var closedServiceInfo = new ServiceInfo(ambientServices, closedContractType, closedInstanceType, this.IsSingleton())
            {
                ServiceType = closedServiceType,
                AllowMultiple = this.AllowMultiple,
                ExternallyOwned = this.ExternallyOwned,
            };

            return closedServiceInfo;
        }

        public AppServiceInfo ToAppServiceInfo(IAmbientServices ambientServices)
        {
            return new AppServiceInfo(this.ContractType, ctx => this.GetService(ambientServices), this.Lifetime)
            {
                AllowMultiple = this.AllowMultiple,
            };
        }

        public object GetService(IAmbientServices ambientServices) => this.lazyFactory.GetValue();

        public IDictionary<string, object>? Metadata { get; internal set; }

        public void Dispose()
        {
            if (!this.ExternallyOwned)
            {
                this.lazyFactory.Dispose();
            }
        }

        private Func<object> GetInstanceResolver(IAmbientServices ambientServices, Type instanceType)
        {
            if (this.instanceResolver != null)
            {
                return this.instanceResolver;
            }

            var (ctor, ctorParams) = GetConstructorInfo(ambientServices, instanceType);

            return this.instanceResolver = () => ctor.Invoke(
                       ctorParams.Select(
                           p => p.HasDefaultValue
                                    ? ambientServices.GetService(p.ParameterType) ?? p.DefaultValue
                                    : ambientServices.GetRequiredService(p.ParameterType))
                           .ToArray());
        }

        private static (ConstructorInfo maxCtor, ParameterInfo[] maxCtorParams) GetConstructorInfo(
            IAmbientServices ambientServices,
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

                var unresolvedCtorParams = ctorParams.Where(p => !p.HasDefaultValue && !ambientServices.IsRegistered(p.ParameterType)).ToList();

                if (unresolvedCtorParams.Count == 0)
                {
                    if (maxLength == ctorParams.Length && maxCtor != null)
                    {
                        throw new AmbiguousMatchException(
                            string.Format(
                                Strings.AmbientServices_AmbiguousConstructors_Exception,
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
                    string.Format(Strings.AmbientServices_MissingCompositionConstructor_Exception, instanceType, string.Join(", ", unresolvedParams)));
            }

            return (maxCtor, maxCtorParams);
        }

        private class LazyValue : LazyFactory
        {
            private object value;

            public LazyValue(Func<object> factory, Type serviceType)
                : base(factory, serviceType)
            {
            }

            public LazyValue(object value)
                : base(null, null)
            {
                this.value = value;
            }

            public override object GetValue()
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

                    return this.value = base.GetValue();
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
            private static List<LazyFactory> isProducing;

            protected readonly Func<object> factory;

            private readonly Type serviceType;

            public LazyFactory(Func<object> factory, Type serviceType)
            {
                this.factory = factory;
                this.serviceType = serviceType;
            }

            public virtual void Dispose()
            {
            }

            public virtual object GetValue()
            {
                // at one time, a single value may be produced per thread
                // otherwise it means that it occured a circular dependency
                if (isProducing == null)
                {
                    isProducing = new List<LazyFactory>();
                }
                else if (isProducing.Contains(this))
                {
                    throw new CircularDependencyException(string.Format(Strings.LazyFactory_CircularDependency_Exception, serviceType));
                }

                isProducing.Add(this);
                try
                {
                    var value = this.factory();
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