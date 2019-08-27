// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceInfo.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the service information class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Internal
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
        private Lazy<object> lazyInstance;

        private Func<object> instanceResolver;

        public ServiceInfo(Type contractType, object instance)
        {
            this.ContractType = contractType;
            this.Instance = instance;
            this.Lifetime = AppServiceLifetime.Singleton;
            this.lazyInstance = new Lazy<object>(() => instance);
        }

        public ServiceInfo(IAmbientServices ambientServices, Type contractType, Type instanceType, bool isSingleton)
        {
            this.ContractType = contractType;
            this.InstanceType = instanceType;
            this.Lifetime = isSingleton ? AppServiceLifetime.Singleton : AppServiceLifetime.Transient;
            this.lazyInstance = isSingleton ? new Lazy<object>(() => this.GetInstanceResolver(ambientServices, instanceType)()) : null;
        }

        public ServiceInfo(IAmbientServices ambientServices, Type contractType, Func<ICompositionContext, object> serviceFactory, bool isSingleton)
        {
            this.ContractType = contractType;
            this.InstanceFactory = serviceFactory;
            this.Lifetime = isSingleton ? AppServiceLifetime.Singleton : AppServiceLifetime.Transient;
            this.lazyInstance = isSingleton ? new Lazy<object>(() => serviceFactory(ambientServices.AsCompositionContext())) : null;
        }

        public AppServiceLifetime Lifetime { get; private set; }

        bool IAppServiceInfo.AllowMultiple { get; } = false;

        bool IAppServiceInfo.AsOpenGeneric { get; } = false;

        Type[] IAppServiceInfo.MetadataAttributes { get; }

        public Type ContractType { get; }

        public object Instance { get; internal set; }

        public Type InstanceType { get; private set; }

        public Func<ICompositionContext, object> InstanceFactory { get; }

        public object GetService(IAmbientServices ambientServices)
        {
            return this.lazyInstance != null
                       ? this.lazyInstance.Value
                       : this.InstanceFactory != null
                           ? this.InstanceFactory(ambientServices.AsCompositionContext())
                           : this.GetInstanceResolver(ambientServices, this.InstanceType)();
        }

        private Func<object> GetInstanceResolver(IAmbientServices ambientServices, Type instanceType)
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