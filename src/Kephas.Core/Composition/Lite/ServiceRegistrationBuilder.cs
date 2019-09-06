// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceRegistrationBuilder.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the service registration builder class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Lite
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Kephas.Composition.Lite.Internal;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Resources;
    using Kephas.Services;

    internal class ServiceRegistrationBuilder : IServiceRegistrationBuilder
    {
        private Type contractType;

        private Type serviceType;

        private readonly IAmbientServices ambientServices;

        private AppServiceLifetime lifetime = AppServiceLifetime.Singleton;

        private bool allowMultiple = false;

        private object instancing;

        private IDictionary<string, object> metadata;

        public ServiceRegistrationBuilder(IAmbientServices ambientServices, Type contractType)
        {
            this.contractType = contractType;
            serviceType = contractType;
            this.ambientServices = ambientServices;
        }

        public IServiceInfo Build()
        {
            switch (instancing)
            {
                case Type implementationType:
                    return new ServiceInfo(ambientServices, contractType, implementationType, lifetime == AppServiceLifetime.Singleton)
                    {
                        AllowMultiple = allowMultiple,
                        ServiceType = serviceType,
                    };
                case Func<ICompositionContext, object> factory:
                    return new ServiceInfo(ambientServices, contractType, factory, lifetime == AppServiceLifetime.Singleton)
                    {
                        AllowMultiple = allowMultiple,
                        ServiceType = serviceType,
                    };
                default:
                    if (instancing == null)
                    {
                        throw new InvalidOperationException(Strings.ServiceRegistrationBuilder_InstancingNotProvided_Exception);
                    }

                    return new ServiceInfo(contractType, instancing)
                    {
                        AllowMultiple = allowMultiple,
                        ServiceType = serviceType,
                    };
            }
        }

        public IServiceRegistrationBuilder RegisterAs(Type contractType)
        {
            Requires.NotNull(contractType, nameof(contractType));

            if (!this.contractType.IsAssignableFrom(serviceType))
            {
                throw new InvalidOperationException(
                    string.Format(
                        Strings.AmbientServices_ServiceTypeMustBeSuperOfContractType_Exception,
                        serviceType,
                        contractType));
            }

            this.contractType = contractType;

            return this;
        }

        public IServiceRegistrationBuilder AsSingleton()
        {
            lifetime = AppServiceLifetime.Singleton;
            return this;
        }

        public IServiceRegistrationBuilder AsTransient()
        {
            lifetime = AppServiceLifetime.Transient;
            return this;
        }

        public IServiceRegistrationBuilder AllowMultiple()
        {
            allowMultiple = true;
            return this;
        }

        public IServiceRegistrationBuilder WithInstance(object instance)
        {
            Requires.NotNull(instance, nameof(instance));

            if (!this.contractType.IsInstanceOfType(instance))
            {
                throw new InvalidOperationException(
                    string.Format(
                        Strings.AmbientServices_ServiceTypeAndServiceInstanceMismatch_Exception,
                        instance.GetType(),
                        this.contractType));
            }

            this.instancing = instance;
            return this;
        }

        public IServiceRegistrationBuilder WithFactory(Func<ICompositionContext, object> factory)
        {
            Requires.NotNull(factory, nameof(factory));

            this.instancing = factory;
            return this;
        }

        public IServiceRegistrationBuilder WithType(Type implementationType)
        {
            Requires.NotNull(implementationType, nameof(implementationType));

            if (!this.contractType.IsAssignableFrom(implementationType)
                && !(this.contractType.IsGenericTypeDefinition
                     && !implementationType.IsGenericTypeDefinition
                     && implementationType.GetInterfaces().Any(i => ReferenceEquals(i.GetGenericTypeDefinition(), this.contractType)))
                && !(this.contractType.IsGenericTypeDefinition
                     && implementationType.IsGenericTypeDefinition
                     && implementationType.GetInterfaces().Any(i => i.Name == this.contractType.Name)))
            {
                throw new InvalidOperationException(
                    string.Format(
                        Strings.AmbientServices_ServiceTypeAndImplementationMismatch_Exception,
                        implementationType,
                        this.contractType));
            }

            this.instancing = implementationType;
            return this;
        }

        public IServiceRegistrationBuilder AddMetadata(string key, object value)
        {
            if (this.metadata == null)
            {
                this.metadata = new Dictionary<string, object> { { key, value } };
            }
            else
            {
                this.metadata[key] = value;
            }

            return this;
        }
    }
}