// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppServiceInfo.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the application service information class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services.Reflection
{
    using System;
    using System.Collections.Generic;

    using Kephas.Dynamic;
    using Kephas.Injection;

    /// <summary>
    /// Information about the application service.
    /// </summary>
    public class AppServiceInfo : Expando, IAppServiceInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppServiceInfo"/> class.
        /// </summary>
        /// <param name="contractType">The contract type of the export.</param>
        /// <param name="lifetime">Optional. The application service lifetime.</param>
        /// <param name="asOpenGeneric">Optional.
        ///                             <c>true</c> if the contract should be exported as an open generic;
        ///                             otherwise, <c>false</c>.
        /// </param>
        public AppServiceInfo(Type contractType, AppServiceLifetime lifetime = AppServiceLifetime.Singleton, bool asOpenGeneric = false)
        {
            contractType = contractType ?? throw new ArgumentNullException(nameof(contractType));

            this.SetContractType(contractType);
            this.AsOpenGeneric = asOpenGeneric;
            this.SetLifetime(lifetime);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppServiceInfo"/> class.
        /// </summary>
        /// <param name="contractType">The contract type of the export.</param>
        /// <param name="serviceInstance">The service instance.</param>
        public AppServiceInfo(Type contractType, object serviceInstance)
        {
            contractType = contractType ?? throw new ArgumentNullException(nameof(contractType));
            serviceInstance = serviceInstance ?? throw new ArgumentNullException(nameof(serviceInstance));

            this.SetContractType(contractType);
            this.InstancingStrategy = serviceInstance;
            this.SetLifetime(AppServiceLifetime.Singleton);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppServiceInfo"/> class.
        /// </summary>
        /// <param name="contractType">The contract type of the export.</param>
        /// <param name="serviceInstanceFactory">The service instance factory.</param>
        /// <param name="lifetime">Optional. The application service lifetime.</param>
        public AppServiceInfo(Type contractType, Func<IInjector, object> serviceInstanceFactory, AppServiceLifetime lifetime = AppServiceLifetime.Transient)
        {
            contractType = contractType ?? throw new ArgumentNullException(nameof(contractType));
            serviceInstanceFactory = serviceInstanceFactory ?? throw new ArgumentNullException(nameof(serviceInstanceFactory));

            this.SetContractType(contractType);
            this.InstancingStrategy = serviceInstanceFactory;
            this.SetLifetime(lifetime);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppServiceInfo"/> class.
        /// </summary>
        /// <param name="contractType">The contract type of the export.</param>
        /// <param name="serviceType">Type of the service instance.</param>
        /// <param name="lifetime">Optional. The application service lifetime.</param>
        /// <param name="asOpenGeneric">Optional.
        ///                             <c>true</c> if the contract should be exported as an open generic;
        ///                             otherwise, <c>false</c>.
        /// </param>
        public AppServiceInfo(Type contractType, Type serviceType, AppServiceLifetime lifetime = AppServiceLifetime.Singleton, bool asOpenGeneric = false)
        {
            contractType = contractType ?? throw new ArgumentNullException(nameof(contractType));
            serviceType = serviceType ?? throw new ArgumentNullException(nameof(serviceType));

            this.SetContractType(contractType);
            this.InstancingStrategy = serviceType;
            this.AsOpenGeneric = asOpenGeneric;
            this.SetLifetime(lifetime);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppServiceInfo"/> class.
        /// </summary>
        /// <param name="appServiceInfo">The <see cref="IAppServiceInfo"/> from which the information should be copied.</param>
        /// <param name="contractType">The contract type, in case the service information does not have it already.</param>
        /// <param name="instancingStrategy">Optional. The instancing strategy.</param>
        internal AppServiceInfo(IAppServiceInfo appServiceInfo, Type? contractType, object? instancingStrategy = null)
        {
            contractType = contractType ?? throw new ArgumentNullException(nameof(contractType));
            appServiceInfo = appServiceInfo ?? throw new ArgumentNullException(nameof(appServiceInfo));

            this.SetContractType(contractType ?? appServiceInfo.ContractType);
            this.InstancingStrategy = instancingStrategy ?? appServiceInfo.InstancingStrategy;

            this.AsOpenGeneric = appServiceInfo.AsOpenGeneric;
            this.SetLifetime(appServiceInfo.Lifetime);
            this.Metadata = appServiceInfo.Metadata;
            this.AllowMultiple = appServiceInfo.AllowMultiple;
        }

        /// <summary>
        /// Gets the application service lifetime.
        /// </summary>
        /// <value>
        /// The application service lifetime.
        /// </value>
        public AppServiceLifetime Lifetime { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether multiple services for this contract are allowed.
        /// </summary>
        /// <value>
        ///   <c>true</c> if multiple services are allowed; otherwise, <c>false</c>.
        /// </value>
        public bool AllowMultiple { get; set; }

        /// <summary>
        /// Gets a value indicating whether the contract should be exported as an open generic.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the contract should be exported as an open generic; otherwise, <c>false</c>.
        /// </value>
        public bool AsOpenGeneric { get; private set; }

        /// <summary>
        /// Gets the metadata attached to the service information.
        /// </summary>
        public IDictionary<string, object?>? Metadata { get; private set; }

        /// <summary>
        /// Gets the contract type of the export.
        /// </summary>
        /// <value>
        /// The contract type of the export.
        /// </value>
        public Type? ContractType { get; private set; }

        /// <summary>
        /// Gets or sets the supported metadata type.
        /// </summary>
        /// <value>
        /// The supported metadata type.
        /// </value>
        public Type? MetadataType { get; set; }

        /// <summary>
        /// Gets the instancing strategy: factory, type, or instance.
        /// </summary>
        public object? InstancingStrategy { get; }

        /// <summary>
        /// Gets a value indicating whether the service is externally owned and should not be disposed by the container. 
        /// </summary>
        public bool IsExternallyOwned { get; internal set; }

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
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            var multiple = this.AllowMultiple ? ", multi" : string.Empty;
            var openGeneric = this.AsOpenGeneric ? ", open generic" : string.Empty;
            IAppServiceInfo serviceInfo = this;
            var instanceTypeString = serviceInfo.InstanceType != null ? $"/type:{serviceInfo.InstanceType}" : string.Empty;
            var instanceString = serviceInfo.Instance != null ? $"/instance:{serviceInfo.Instance}" : string.Empty;
            var factoryString = serviceInfo.InstanceFactory != null ? $"/instanceFactory" : string.Empty;

            return $"{this.ContractType}{multiple}{openGeneric}, {this.Lifetime}{instanceTypeString}{instanceString}{factoryString}";
        }

        private void SetContractType(Type? contractType)
        {
            this.ContractType = contractType;
            if (contractType?.IsGenericTypeDefinition ?? false)
            {
                this.AsOpenGeneric = true;
            }
        }

        private void SetLifetime(AppServiceLifetime lifetime)
        {
            this.Lifetime = lifetime;
        }
    }
}