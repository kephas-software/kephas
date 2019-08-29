// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceRegistrationBuilder.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the service registration builder class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Lightweight
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Design;

    using Kephas.Composition.Lightweight.Internal;
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
            this.serviceType = contractType;
            this.ambientServices = ambientServices;
        }

        public IServiceInfo Build()
        {
            switch (this.instancing)
            {
                case Type implementationType:
                    return new ServiceInfo(this.ambientServices, this.contractType, implementationType, this.lifetime == AppServiceLifetime.Singleton)
                    {
                        AllowMultiple = this.allowMultiple,
                        ServiceType = this.serviceType,
                    };
                case Func<ICompositionContext, object> factory:
                    return new ServiceInfo(this.ambientServices, this.contractType, factory, this.lifetime == AppServiceLifetime.Singleton)
                    {
                        AllowMultiple = this.allowMultiple,
                        ServiceType = this.serviceType,
                    };
                default:
                    return new ServiceInfo(this.contractType, this.instancing)
                    {
                        AllowMultiple = this.allowMultiple,
                        ServiceType = this.serviceType,
                    };
            }
        }

        public IServiceRegistrationBuilder RegisteredAs(Type contractType)
        {
            Requires.NotNull(contractType, nameof(contractType));

            if (!this.contractType.IsAssignableFrom(this.serviceType))
            {
                throw new InvalidOperationException(
                    string.Format(
                        Strings.AmbientServices_ServiceTypeMustBeSuperOfContractType_Exception,
                        this.serviceType,
                        contractType));
            }

            this.contractType = contractType;

            return this;
        }

        public IServiceRegistrationBuilder AsSingleton()
        {
            this.lifetime = AppServiceLifetime.Singleton;
            return this;
        }

        public IServiceRegistrationBuilder AsTransient()
        {
            this.lifetime = AppServiceLifetime.Transient;
            return this;
        }

        public IServiceRegistrationBuilder AllowMultiple()
        {
            this.allowMultiple = true;
            return this;
        }

        public IServiceRegistrationBuilder WithInstance(object instance)
        {
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
            this.instancing = factory;
            return this;
        }

        public IServiceRegistrationBuilder WithType(Type implementationType)
        {
            if (!this.contractType.IsAssignableFrom(implementationType))
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