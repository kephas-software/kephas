// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LiteRegistrationBuilder.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the registration builder class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Injection.Lite.Conventions
{
    using System;
    using System.Collections.Generic;

    using Kephas.Collections;
    using Kephas.Logging;
    using Kephas.Resources;
    using Kephas.Services;

    /// <summary>
    /// A lightweight registration builder.
    /// </summary>
    internal class LiteRegistrationBuilder : Loggable
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
        /// Gets or sets the type of the contract.
        /// </summary>
        /// <value>
        /// The type of the contract.
        /// </value>
        public Type ContractType { get; set; }

        /// <summary>
        /// Gets or sets the instancing strategy.
        /// </summary>
        public object? InstancingStrategy { get; set; }

        /// <summary>
        /// Gets the service instance.
        /// </summary>
        /// <value>
        /// The service instance.
        /// </value>
        public object? Instance => this.Factory == null && this.ImplementationType == null ? this.InstancingStrategy : null;

        /// <summary>
        /// Gets the factory.
        /// </summary>
        /// <value>
        /// A function delegate that yields an object.
        /// </value>
        public Func<IInjector, object>? Factory => this.InstancingStrategy as Func<IInjector, object>;

        /// <summary>
        /// Gets the type of the implementation.
        /// </summary>
        /// <value>
        /// The type of the implementation.
        /// </value>
        public Type? ImplementationType => this.InstancingStrategy as Type;

        /// <summary>
        /// Gets or sets the lifetime.
        /// </summary>
        /// <value>
        /// The lifetime.
        /// </value>
        public AppServiceLifetime Lifetime { get; set; } = AppServiceLifetime.Transient;

        /// <summary>
        /// Gets or sets a value indicating whether we allow multiple.
        /// </summary>
        /// <value>
        /// True if allow multiple, false if not.
        /// </value>
        public bool AllowMultiple { get; internal set; }

        /// <summary>
        /// Gets the metadata.
        /// </summary>
        public IDictionary<string, object?>? Metadata { get; private set; }

        /// <summary>
        /// Add export metadata to the export.
        /// </summary>
        /// <param name="name">The name of the metadata item.</param>
        /// <param name="value">The metadata value.</param>
        /// <returns>
        /// An export builder allowing further configuration.
        /// </returns>
        public LiteRegistrationBuilder AddMetadata(string name, object? value)
        {
            (this.Metadata ??= new Dictionary<string, object?>())[name] = value;
            return this;
        }

        /// <summary>
        /// Specify the contract type for the export.
        /// </summary>
        /// <param name="contractType">The contract type.</param>
        /// <returns>
        /// An export builder allowing further configuration.
        /// </returns>
        public LiteRegistrationBuilder As(Type contractType)
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
            return $"{this.ContractType}/{this.Lifetime}/{implementationString}";
        }


        internal void Build()
        {
            void ConfigureService(IServiceRegistrationBuilder b)
            {
                b.WithInstancingStrategy(this.InstancingStrategy);

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
                    this.Logger.Warn("Scoped services not supported, will be registered as singleton: '{contractType}'.", this.ContractType);
                    b.Singleton();
                }

                if (this.AllowMultiple)
                {
                    b.AllowMultiple();
                }

                if (this.ContractType != null)
                {
                    b.As(this.ContractType);
                }

                if (this.Metadata != null)
                {
                    this.Metadata.ForEach(kv => b.AddMetadata(kv.Key, kv.Value));
                }
            }

            if (this.Instance != null)
            {
                this.ambientServices.Register(this.ContractType, ConfigureService);

                return;
            }

            if (this.ImplementationType != null || this.Factory != null)
            {
                this.ambientServices.Register(this.ContractType, ConfigureService);

                return;
            }

            throw new InvalidOperationException(string.Format(Strings.LiteRegistrationBuilder_InvalidRegistration_Exception, nameof(this.ImplementationType), nameof(this.Factory)));
        }
    }
}
