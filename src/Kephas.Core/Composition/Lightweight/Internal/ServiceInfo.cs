// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceInfo.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the service information class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Lightweight.Internal
{
    using System;
    using System.Linq;
    using System.Reflection;

    using Kephas.Composition;
    using Kephas.Resources;
    using Kephas.Services;
    using Kephas.Services.Reflection;

    internal class ServiceInfo : IServiceInfo
    {
        private readonly LazyFactory lazyFactory;

        private Func<object> instanceResolver;

        public ServiceInfo(Type contractType, object instance)
        {
            this.ContractType = contractType;
            this.Instance = instance;
            this.Lifetime = AppServiceLifetime.Singleton;
            this.lazyFactory = new LazyValue(instance);
        }

        public ServiceInfo(IAmbientServices ambientServices, Type contractType, Type instanceType, bool isSingleton)
        {
            this.ContractType = contractType;
            this.InstanceType = instanceType;
            this.Lifetime = isSingleton ? AppServiceLifetime.Singleton : AppServiceLifetime.Transient;
            this.lazyFactory = isSingleton
                                   ? new LazyValue(() => this.GetInstanceResolver(ambientServices, instanceType)(), contractType)
                                   : new LazyFactory(() => this.GetInstanceResolver(ambientServices, instanceType)(), contractType);
        }

        public ServiceInfo(IAmbientServices ambientServices, Type contractType, Func<ICompositionContext, object> serviceFactory, bool isSingleton)
        {
            this.ContractType = contractType;
            this.InstanceFactory = serviceFactory;
            this.Lifetime = isSingleton ? AppServiceLifetime.Singleton : AppServiceLifetime.Transient;
            var compositionContext = ambientServices.AsCompositionContext();
            this.lazyFactory = isSingleton
                                   ? new LazyValue(() => serviceFactory(compositionContext), contractType)
                                   : new LazyFactory(() => serviceFactory(compositionContext), contractType);
        }

        public AppServiceLifetime Lifetime { get; }

        public bool AllowMultiple { get; internal set; } = false;

        bool IAppServiceInfo.AsOpenGeneric { get; } = false;

        Type[] IAppServiceInfo.MetadataAttributes { get; }

        public Type ContractType { get; }

        public object Instance { get; internal set; }

        public Type InstanceType { get; }

        public Func<ICompositionContext, object> InstanceFactory { get; }

        public AppServiceInfo ToAppServiceInfo(IAmbientServices ambientServices)
        {
            return new AppServiceInfo(this.ContractType, ctx => this.GetService(ambientServices), this.Lifetime) { AllowMultiple = this.AllowMultiple };
        }

        public object GetService(IAmbientServices ambientServices) => this.lazyFactory.GetValue();

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
                                    ? (ambientServices.GetService(p.ParameterType) ?? p.DefaultValue)
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
        }

        private class LazyFactory
        {
            protected readonly Func<object> factory;

            private readonly Type serviceType;

            [ThreadStatic]
            private static bool isProducing = false;

            public LazyFactory(Func<object> factory, Type serviceType)
            {
                this.factory = factory;
                this.serviceType = serviceType;
            }

            public virtual object GetValue()
            {
                // at one time, a single value may be produced per thread
                // otherwise it means that it occured a circular dependency
                if (isProducing)
                {
                    throw new InvalidOperationException(string.Format(Strings.LazyFactory_CircularDependency_Exception, this.serviceType));
                }

                isProducing = true;
                try
                {
                    var value = this.factory();
                    return value;
                }
                finally
                {
                    isProducing = false;
                }
            }
        }
    }
}