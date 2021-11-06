// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppServicesAttribute.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    using Kephas.Logging;
    using Kephas.Services.Reflection;

    /// <summary>
    /// Assembly attribute decorating an assembly and collecting the application services, both contract types and implementation types.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public sealed class AppServicesAttribute : Attribute, IAppServiceInfosProvider, IHasProcessingPriority
    {
        private readonly Lazy<ILogger> lazyLogger = new (() => LoggingHelper.DefaultLogManager.GetLogger(typeof(AppServicesAttribute)));

        /// <summary>
        /// Initializes a new instance of the <see cref="AppServicesAttribute"/> class.
        /// </summary>
        /// <param name="providerType">
        /// The type providing the application services.
        /// Should implement the <see cref="IAppServiceInfosProvider"/> interface.
        /// </param>
        /// <param name="processingPriority">Adds the processing priority in which the attributes will be processed.</param>
        public AppServicesAttribute([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type providerType, Priority processingPriority = Priority.Normal)
        {
            this.ProviderType = providerType ?? throw new ArgumentNullException(nameof(providerType));
            this.ProcessingPriority = processingPriority;

            if (!typeof(IAppServiceInfosProvider).IsAssignableFrom(this.ProviderType))
            {
                throw new ArgumentException($"The provider type must implement {nameof(IAppServiceInfosProvider)}", nameof(providerType));
            }
        }

        /// <summary>
        /// Gets the provider type.
        /// </summary>
        public Type ProviderType { get; }

        /// <summary>
        /// Gets the processing priority.
        /// </summary>
        public Priority ProcessingPriority { get; }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        private ILogger Logger => this.lazyLogger.Value;

        /// <summary>
        /// Gets an enumeration of application service information objects and their contract declaration type.
        /// The contract declaration type is the type declaring the contract: if the <see cref="AppServiceContractAttribute.ContractType"/>
        /// is not provided, the contract declaration type is also the contract type.
        /// </summary>
        /// <param name="context">Optional. The context in which the service types are requested.</param>
        /// <returns>
        /// An enumeration of application service information objects and their contract declaration type.
        /// </returns>
        public IEnumerable<ContractDeclaration> GetAppServiceContracts(dynamic? context = null)
        {
            if (this.Logger.IsTraceEnabled())
            {
                this.Logger.Trace("Creating instance of {providerType} in {operation}...", this.ProviderType, nameof(this.GetAppServiceContracts));
            }

            var provider = Activator.CreateInstance(this.ProviderType) as IAppServiceInfosProvider;
            if (this.Logger.IsTraceEnabled())
            {
                this.Logger.Trace("Instance of {providerType} created successfully in {operation}.", this.ProviderType, nameof(this.GetAppServiceContracts));
            }

            return provider?.GetAppServiceContracts(context) ?? Array.Empty<ContractDeclaration>();
        }

        /// <summary>
        /// Gets an enumeration of tuples containing the service type and the contract declaration type which it implements.
        /// </summary>
        /// <param name="context">Optional. The context in which the service types are requested.</param>
        /// <returns>
        /// An enumeration of tuples containing the service type and the contract declaration type which it implements.
        /// </returns>
        public IEnumerable<ServiceDeclaration> GetAppServices(dynamic? context = null)
        {
            if (this.Logger.IsTraceEnabled())
            {
                this.Logger.Trace("Creating instance of {providerType} in {operation}...", this.ProviderType, nameof(this.GetAppServices));
            }

            var provider = Activator.CreateInstance(this.ProviderType) as IAppServiceInfosProvider;
            if (this.Logger.IsTraceEnabled())
            {
                this.Logger.Trace("Instance of {providerType} created successfully in {operation}.", this.ProviderType, nameof(this.GetAppServices));
            }

            var services = provider?.GetAppServices(context) ?? Array.Empty<ServiceDeclaration>();
            foreach (var service in services)
            {
                if (this.Logger.IsTraceEnabled())
                {
                    this.Logger.Trace("Getting service...");
                    this.Logger.Trace($"Yielding {service.ServiceType} for {service.ContractDeclarationType}...");
                }

                yield return service;
            }
        }


        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        /// <footer><a href="https://docs.microsoft.com/en-us/dotnet/api/System.Object.ToString?view=netstandard-2.1">`Object.ToString` on docs.microsoft.com</a></footer>
        public override string ToString()
        {
            return $"AppServices/{this.ProviderType}/{this.ProcessingPriority}";
        }
    }
}