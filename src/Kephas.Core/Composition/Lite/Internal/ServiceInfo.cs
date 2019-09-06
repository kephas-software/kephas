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
    using Kephas.Dynamic;
    using Kephas.Reflection;
    using Kephas.Resources;
    using Kephas.Services;
    using Kephas.Services.Reflection;

    internal class ServiceInfo : IServiceInfo
    {
        private readonly LazyFactory lazyFactory;

        private Func<object> instanceResolver;

        public ServiceInfo(Type contractType, object instance)
        {
            ContractType = contractType;
            Instance = instance;
            Lifetime = AppServiceLifetime.Singleton;
            lazyFactory = new LazyValue(instance);
        }

        public ServiceInfo(IAmbientServices ambientServices, Type contractType, Type instanceType, bool isSingleton)
        {
            ContractType = contractType;
            InstanceType = instanceType;
            Lifetime = isSingleton ? AppServiceLifetime.Singleton : AppServiceLifetime.Transient;
            lazyFactory = isSingleton
                                   ? new LazyValue(() => GetInstanceResolver(ambientServices, instanceType)(), contractType)
                                   : new LazyFactory(() => GetInstanceResolver(ambientServices, instanceType)(), contractType);
        }

        public ServiceInfo(IAmbientServices ambientServices, Type contractType, Func<ICompositionContext, object> serviceFactory, bool isSingleton)
        {
            ContractType = contractType;
            InstanceFactory = serviceFactory;
            Lifetime = isSingleton ? AppServiceLifetime.Singleton : AppServiceLifetime.Transient;
            var compositionContext = ambientServices.AsCompositionContext();
            lazyFactory = isSingleton
                                   ? new LazyValue(() => serviceFactory(compositionContext), contractType)
                                   : new LazyFactory(() => serviceFactory(compositionContext), contractType);
        }

        public AppServiceLifetime Lifetime { get; }

        public bool AllowMultiple { get; internal set; } = false;

        bool IAppServiceInfo.AsOpenGeneric { get; } = false;

        Type[] IAppServiceInfo.MetadataAttributes { get; }

        public Type ContractType { get; }

        public Type ServiceType { get; internal set; }

        public object Instance { get; internal set; }

        public Type InstanceType { get; }

        public Func<ICompositionContext, object> InstanceFactory { get; }

        public AppServiceInfo ToAppServiceInfo(IAmbientServices ambientServices)
        {
            return new AppServiceInfo(ContractType, ctx => GetService(ambientServices), Lifetime) { AllowMultiple = AllowMultiple };
        }

        public object GetService(IAmbientServices ambientServices) => lazyFactory.GetValue();

        public IDictionary<string, object> Metadata { get; internal set; }

        private Func<object> GetInstanceResolver(IAmbientServices ambientServices, Type instanceType)
        {
            if (instanceResolver != null)
            {
                return instanceResolver;
            }

            var (ctor, ctorParams) = GetConstructorInfo(ambientServices, instanceType);

            return instanceResolver = () => ctor.Invoke(
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
            var ctors = instanceType.GetConstructors().Where(c => c.IsStatic == false && c.IsPublic)
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
            }

            if (maxCtor == null)
            {
                throw new CompositionException(
                    string.Format(Strings.AmbientServices_MissingCompositionConstructor_Exception, instanceType));
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
                if (value != null)
                {
                    return value;
                }

                lock (factory)
                {
                    if (value != null)
                    {
                        return value;
                    }

                    return value = base.GetValue();
                }
            }
        }

        private class LazyFactory
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
                    var value = factory();
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