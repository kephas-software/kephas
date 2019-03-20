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

    using Kephas.Composition;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Dynamic;

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
        /// <param name="allowMultiple">Optional.
        ///                             <c>true</c> if multiple services are allowed; otherwise,
        ///                             <c>false</c>.
        /// </param>
        /// <param name="asOpenGeneric">Optional.
        ///                             <c>true</c> if the contract should be exported as an open generic;
        ///                             otherwise, <c>false</c>.
        /// </param>
        public AppServiceInfo(Type contractType, AppServiceLifetime lifetime = AppServiceLifetime.Shared, bool allowMultiple = false, bool asOpenGeneric = false)
        {
            Requires.NotNull(contractType, nameof(contractType));

            this.ContractType = contractType;
            this.AllowMultiple = allowMultiple;
            this.AsOpenGeneric = asOpenGeneric;
            this.Lifetime = lifetime;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppServiceInfo"/> class.
        /// </summary>
        /// <param name="contractType">The contract type of the export.</param>
        /// <param name="serviceInstance">The service instance.</param>
        public AppServiceInfo(Type contractType, object serviceInstance)
        {
            Requires.NotNull(contractType, nameof(contractType));
            Requires.NotNull(serviceInstance, nameof(serviceInstance));

            this.ContractType = contractType;
            this.Instance = serviceInstance;
            this.Lifetime = AppServiceLifetime.Shared;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppServiceInfo"/> class.
        /// </summary>
        /// <param name="contractType">The contract type of the export.</param>
        /// <param name="serviceInstanceFactory">The service instance factory.</param>
        public AppServiceInfo(Type contractType, Func<ICompositionContext, object> serviceInstanceFactory)
        {
            Requires.NotNull(contractType, nameof(contractType));
            Requires.NotNull(serviceInstanceFactory, nameof(serviceInstanceFactory));

            this.ContractType = contractType;
            this.InstanceFactory = serviceInstanceFactory;
            this.Lifetime = AppServiceLifetime.Instance;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppServiceInfo"/> class.
        /// </summary>
        /// <param name="contractType">The contract type of the export.</param>
        /// <param name="serviceInstanceType">Type of the service instance.</param>
        /// <param name="lifetime">Optional. The application service lifetime.</param>
        public AppServiceInfo(Type contractType, Type serviceInstanceType, AppServiceLifetime lifetime = AppServiceLifetime.Shared)
        {
            Requires.NotNull(contractType, nameof(contractType));
            Requires.NotNull(serviceInstanceType, nameof(serviceInstanceType));

            this.ContractType = contractType;
            this.InstanceType = serviceInstanceType;
            this.Lifetime = lifetime;
            if (lifetime == AppServiceLifetime.ScopeShared)
            {
                this.ScopeName = CompositionScopeNames.Default;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppServiceInfo"/> class.
        /// </summary>
        /// <param name="contractType">The contract type of the export.</param>
        /// <param name="serviceInstanceType">Type of the service instance.</param>
        /// <param name="scopeName">The name of the scope for scoped shared services.</param>
        public AppServiceInfo(Type contractType, Type serviceInstanceType, string scopeName)
        {
            Requires.NotNull(contractType, nameof(contractType));
            Requires.NotNull(serviceInstanceType, nameof(serviceInstanceType));
            Requires.NotNullOrEmpty(scopeName, nameof(scopeName));

            this.ContractType = contractType;
            this.InstanceType = serviceInstanceType;
            this.Lifetime = AppServiceLifetime.ScopeShared;
            this.ScopeName = scopeName;
        }

        /// <summary>
        /// Gets the application service lifetime.
        /// </summary>
        /// <value>
        /// The application service lifetime.
        /// </value>
        public AppServiceLifetime Lifetime { get; }

        /// <summary>
        /// Gets a value indicating whether multiple services for this contract are allowed.
        /// </summary>
        /// <value>
        ///   <c>true</c> if multiple services are allowed; otherwise, <c>false</c>.
        /// </value>
        public bool AllowMultiple { get; }

        /// <summary>
        /// Gets a value indicating whether the contract should be exported as an open generic.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the contract should be exported as an open generic; otherwise, <c>false</c>.
        /// </value>
        public bool AsOpenGeneric { get; }

        /// <summary>
        /// Gets or sets the supported metadata attributes.
        /// </summary>
        /// <value>
        /// The metadata attributes.
        /// </value>
        /// <remarks>The metadata attributes are used to register the conventions for application services.</remarks>
        public Type[] MetadataAttributes { get; set; }

        /// <summary>
        /// Gets the contract type of the export.
        /// </summary>
        /// <value>
        /// The contract type of the export.
        /// </value>
        public Type ContractType { get; }

        /// <summary>
        /// Gets the service instance.
        /// </summary>
        /// <value>
        /// The service instance.
        /// </value>
        public object Instance { get; }

        /// <summary>
        /// Gets the type of the service instance.
        /// </summary>
        /// <value>
        /// The type of the service instance.
        /// </value>
        public Type InstanceType { get; }

        /// <summary>
        /// Gets the service instance factory.
        /// </summary>
        /// <value>
        /// The service instance factory.
        /// </value>
        public Func<ICompositionContext, object> InstanceFactory { get; }

        /// <summary>
        /// Gets the name of the scope for scoped shared services.
        /// </summary>
        /// <value>
        /// The name of the scope for scoped shared services.
        /// </value>
        public string ScopeName { get; }
    }
}