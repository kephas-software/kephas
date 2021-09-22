// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LiteRegistrationBuilder.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the registration builder class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Kephas.Injection.Conventions;
using Kephas.Logging;
using Kephas.Resources;
using Kephas.Services;

namespace Kephas.Injection.Lite.Conventions
{
    /// <summary>
    /// A lightweight registration builder.
    /// </summary>
    internal class LiteRegistrationBuilder : Loggable, IExportConventionsBuilder
    {
        private readonly IAmbientServices ambientServices;

        /// <summary>
        /// Initializes a new instance of the <see cref="LiteRegistrationBuilder"/> class.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        public LiteRegistrationBuilder(IAmbientServices ambientServices)
            : base(ambientServices)
        {
            this.ambientServices = ambientServices;
        }

        /// <summary>
        /// Gets or sets the type of the service.
        /// </summary>
        /// <value>
        /// The type of the service.
        /// </value>
        public Type ServiceType { get; set; }

        /// <summary>
        /// Gets or sets the type of the contract.
        /// </summary>
        /// <value>
        /// The type of the contract.
        /// </value>
        public Type ContractType { get; set; }

        /// <summary>
        /// Gets or sets the service instance.
        /// </summary>
        /// <value>
        /// The service instance.
        /// </value>
        public object? Instance { get; set; }

        /// <summary>
        /// Gets or sets the factory.
        /// </summary>
        /// <value>
        /// A function delegate that yields an object.
        /// </value>
        public Func<IInjector, object>? Factory { get; set; }

        /// <summary>
        /// Gets or sets the type of the implementation.
        /// </summary>
        /// <value>
        /// The type of the implementation.
        /// </value>
        public Type? ImplementationType { get; set; }

        /// <summary>
        /// Gets or sets the lifetime.
        /// </summary>
        /// <value>
        /// The lifetime.
        /// </value>
        public AppServiceLifetime Lifetime { get; set; } = AppServiceLifetime.Transient;

        /// <summary>
        /// Gets or sets the export configuration.
        /// </summary>
        /// <value>
        /// The export configuration.
        /// </value>
        public Action<Type, IExportConventionsBuilder>? ExportConfiguration { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether we allow multiple.
        /// </summary>
        /// <value>
        /// True if allow multiple, false if not.
        /// </value>
        public bool AllowMultiple { get; internal set; }

        /// <summary>
        /// Add export metadata to the export.
        /// </summary>
        /// <param name="name">The name of the metadata item.</param>
        /// <param name="value">The metadata value.</param>
        /// <returns>
        /// An export builder allowing further configuration.
        /// </returns>
        public IExportConventionsBuilder AddMetadata(string name, object value)
        {
            if (this.Logger.IsTraceEnabled())
            {
                this.Logger.Trace("Metadata {metadataName} is automatically added for {registrationBuilder}.", name, this);
            }

            return this;
        }

        /// <summary>
        /// Add export metadata to the export.
        /// </summary>
        /// <param name="name">The name of the metadata item.</param>
        /// <param name="getValueFromPartType">A function that calculates the metadata value based on
        ///                                    the type.</param>
        /// <returns>
        /// An export builder allowing further configuration.
        /// </returns>
        public IExportConventionsBuilder AddMetadata(string name, Func<Type, object> getValueFromPartType)
        {
            if (this.Logger.IsTraceEnabled())
            {
                this.Logger.Trace("Metadata {metadataName} is automatically added for {registrationBuilder}.", name, this);
            }

            return this;
        }

        /// <summary>
        /// Specify the contract type for the export.
        /// </summary>
        /// <param name="contractType">The contract type.</param>
        /// <returns>
        /// An export builder allowing further configuration.
        /// </returns>
        public IExportConventionsBuilder AsContractType(Type contractType)
        {
            this.ContractType = contractType;
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
            var implementationString = this.ImplementationType?.ToString()
                                       ?? (this.Factory != null
                                                ? "factory"
                                                : this.Instance != null ? "instance" : "unknown");
            var serviceTypeString = this.ContractType == null || this.ContractType == this.ServiceType
                                        ? this.ServiceType?.ToString()
                                        : $"{this.ServiceType}({this.ContractType})";
            return $"{serviceTypeString}/{this.Lifetime}/{implementationString}";
        }


        internal void Build()
        {
            void ConfigureService(IServiceRegistrationBuilder b)
            {
                if (this.ImplementationType != null)
                {
                    b.WithType(this.ImplementationType);
                }
                else if (this.Factory != null)
                {
                    b.WithFactory(this.Factory);
                }
                else if (this.Instance != null)
                {
                    b.WithInstance(this.Instance);
                }

                if (this.Lifetime == AppServiceLifetime.Singleton)
                {
                    b.Singleton();
                }
                else if (this.Lifetime == AppServiceLifetime.Transient)
                {
                    b.Transient();
                }
                else if (this.Lifetime == AppServiceLifetime.Scoped)
                {
                    throw new NotSupportedException("Scoped services not supported");
                }

                if (this.AllowMultiple)
                {
                    b.AllowMultiple();
                }

                if (this.ContractType != null && this.ContractType != this.ServiceType)
                {
                    b.As(this.ContractType);
                }
            }

            if (this.Instance != null)
            {
                this.ambientServices.Register(this.ServiceType, ConfigureService);

                return;
            }

            if (this.ImplementationType != null || this.Factory != null)
            {
                this.ExportConfiguration?.Invoke(this.ImplementationType, this);
                this.ambientServices.Register(this.ServiceType, ConfigureService);

                return;
            }

            throw new InvalidOperationException(string.Format(Strings.LiteRegistrationBuilder_InvalidRegistration_Exception, nameof(this.ImplementationType), nameof(this.Factory)));
        }
    }
}
