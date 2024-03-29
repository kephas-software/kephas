﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAppServiceInfo.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IAppServiceInfo interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services.Reflection
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Text;

    using Kephas.Services;

    /// <summary>
    /// Contract interface providing information about an application service.
    /// </summary>
    public interface IAppServiceInfo
    {
        /// <summary>
        /// Gets the application service lifetime.
        /// </summary>
        /// <value>
        /// The application service lifetime.
        /// </value>
        AppServiceLifetime Lifetime { get; }

        /// <summary>
        /// Gets a value indicating whether multiple services for this contract are allowed.
        /// </summary>
        /// <value>
        ///   <c>true</c> if multiple services are allowed; otherwise, <c>false</c>.
        /// </value>
        bool AllowMultiple { get; }

        /// <summary>
        /// Gets a value indicating whether the contract should be exported as an open generic.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the contract should be exported as an open generic; otherwise, <c>false</c>.
        /// </value>
        bool AsOpenGeneric { get; }

        /// <summary>
        /// Gets the supported metadata.
        /// </summary>
        /// <value>
        /// The metadata.
        /// </value>
        IDictionary<string, object?>? Metadata => null;

        /// <summary>
        /// Gets the contract type of the service.
        /// </summary>
        /// <value>
        /// The contract type of the service.
        /// </value>
        Type? ContractType
        {
#if NETSTANDARD2_1
#else
            [return: DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
#endif
            get;
        }

        /// <summary>
        /// Gets the type declaring the contract type.
        /// </summary>
        /// <remarks>
        /// This is the type annotated with the [AppServiceContract] attribute, but which declares a base contract type.
        /// Typically this is a generic type used for collecting metadata, redirecting to the non-generic contract type.
        /// </remarks>
        /// <value>
        /// The type declaring the contract type.
        /// </value>
        Type? ContractDeclarationType
        {
#if NETSTANDARD2_1
#else
            [return: DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
#endif
            get => this.ContractType;
        }

        /// <summary>
        /// Gets or sets the supported metadata type.
        /// </summary>
        /// <value>
        /// The supported metadata type.
        /// </value>
        Type? MetadataType
        {
#if NETSTANDARD2_1
#else
            [return: DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
#endif
            get;
            set;
        }

        /// <summary>
        /// Gets the instancing strategy: factory, type, or instance.
        /// </summary>
        object? InstancingStrategy { get; }

        /// <summary>
        /// Gets the service instance.
        /// </summary>
        /// <value>
        /// The service instance.
        /// </value>
        object? Instance => this.InstanceType != null || this.InstanceFactory != null ? null : this.InstancingStrategy;

        /// <summary>
        /// Gets the type of the service instance.
        /// </summary>
        /// <value>
        /// The type of the service instance.
        /// </value>
        Type? InstanceType
        {
#if NETSTANDARD2_1
#else
            [return: DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
#endif
            get => this.InstancingStrategy as Type;
        }

        /// <summary>
        /// Gets a value indicating whether the service is externally owned and should not be disposed by the container.
        /// </summary>
        bool IsExternallyOwned => false;

        /// <summary>
        /// Gets the service instance factory.
        /// </summary>
        /// <value>
        /// The service instance factory.
        /// </value>
        Func<IServiceProvider, object>? InstanceFactory => this.InstancingStrategy as Func<IServiceProvider, object>;

        /// <summary>
        /// Indicates whether the service information is only a contract definition (<c>true</c>)
        /// or can be used for service registration.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the service information is only a contract definition;
        /// otherwise <c>false</c>.</returns>
        bool IsContractDefinition() => this.InstancingStrategy == null;

        /// <summary>
        /// Adds the metadata with the provided name and value.
        /// </summary>
        /// <param name="name">The metadata name.</param>
        /// <param name="value">The metadata value.</param>
        /// <returns>This <see cref="IAppServiceInfo"/>.</returns>
        public IAppServiceInfo AddMetadata(string name, object? value)
        {
            if (this.Metadata == null)
            {
                throw new InvalidOperationException("Cannot add metadata as long as the instance is not set.");
            }

            this.Metadata[name] = value;

            return this;
        }

        /// <summary>
        /// Adds the metadata with the provided name and value.
        /// </summary>
        /// <param name="metadata">The metadata to add.</param>
        /// <returns>This <see cref="IAppServiceInfo"/>.</returns>
        public IAppServiceInfo AddMetadata(IDictionary<string, object?>? metadata)
        {
            if (metadata == null)
            {
                return this;
            }

            foreach (var (name, value) in metadata)
            {
                this.AddMetadata(name, value);
            }

            return this;
        }

        /// <summary>
        /// Gets the JSON string out of this <see cref="IAppServiceInfo"/>.
        /// </summary>
        /// <returns>The JSON string.</returns>
        public string ToJsonString()
        {
            var appServiceInfo = this;
            var sb = new StringBuilder();
            sb.Append($"{{ multi: {appServiceInfo.AllowMultiple}, lifetime: '{appServiceInfo.Lifetime}'");

            if (appServiceInfo.AsOpenGeneric)
            {
                sb.Append(", asOpenGeneric: true");
            }

            if (appServiceInfo.InstanceType != null)
            {
                sb.Append($", instanceType: '{appServiceInfo.InstanceType}'");
            }

            if (appServiceInfo.Instance != null)
            {
                sb.Append($", instance: '{appServiceInfo.Instance}'");
            }

            if (appServiceInfo.InstanceFactory != null)
            {
                sb.Append(", instanceFactory: '(function)'");
            }

            if (appServiceInfo.MetadataType != null)
            {
                sb.Append($", metadataType: '{appServiceInfo.MetadataType}'");
            }

            if (appServiceInfo.Metadata is { Count: > 0 })
            {
                sb.Append(", metadata: { ");

                foreach (var (key, value) in appServiceInfo.Metadata)
                {
                    sb.Append($"'{key}': '{value}', ");
                }

                sb.Length -= 2;
                sb.Append(" }");
            }

            sb.Append(" }");

            return sb.ToString();
        }
    }
}