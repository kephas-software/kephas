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
    using System.Text;

    using Kephas.Injection;

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
        /// Gets the supported metadata attributes.
        /// </summary>
        /// <value>
        /// The metadata attributes.
        /// </value>
        /// <remarks>The metadata attributes are used to register the conventions for application services.</remarks>
        Type[]? DummyMetadataAttributes => null;

        /// <summary>
        /// Gets the supported metadata.
        /// </summary>
        /// <value>
        /// The metadata.
        /// </value>
        IDictionary<string, object?>? Metadata => null;

        /// <summary>
        /// Gets the contract type of the export.
        /// </summary>
        /// <value>
        /// The contract type of the export.
        /// </value>
        Type? ContractType { get; }

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
        Type? InstanceType => this.InstancingStrategy as Type;

        /// <summary>
        /// Gets the service instance factory.
        /// </summary>
        /// <value>
        /// The service instance factory.
        /// </value>
        Func<IInjector, object>? InstanceFactory => this.InstancingStrategy as Func<IInjector, object>;

        /// <summary>
        /// Indicates whether the service information is only a contract definition (<c>true</c>)
        /// or can be used for service registration.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the service information is only a contract definition;
        /// otherwise <c>false</c>.</returns>
        bool IsContractDefinition() => this.InstancingStrategy == null;

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