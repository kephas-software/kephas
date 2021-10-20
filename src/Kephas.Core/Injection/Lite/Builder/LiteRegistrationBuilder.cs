// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LiteRegistrationBuilder.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the registration builder class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Injection.Lite.Builder
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    using Kephas.Collections;
    using Kephas.Injection.Builder;
    using Kephas.Logging;
    using Kephas.Resources;
    using Kephas.Services;

    /// <summary>
    /// A lightweight registration builder.
    /// </summary>
    internal class LiteRegistrationBuilder : Loggable, IRegistrationBuilder
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
        /// Gets or sets a value indicating whether the service instance is disposed from outside.
        /// </summary>
        /// <value>
        /// True if externally owned, false if not.
        /// </value>
        public bool IsExternallyOwned { get; internal set; }

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
        public IRegistrationBuilder AddMetadata(string name, object? value)
        {
            (this.Metadata ??= new Dictionary<string, object?>())[name] = value;
            return this;
        }

        /// <summary>
        /// Indicates whether the created instances are disposed by an external owner.
        /// </summary>
        /// <returns>
        /// This builder.
        /// </returns>
        public IRegistrationBuilder ExternallyOwned()
        {
            this.IsExternallyOwned = true;
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

        /// <summary>
        /// Indicates the type registered as the exported service key.
        /// </summary>
        /// <param name="contractType">Type of the service.</param>
        /// <returns>
        /// A part builder allowing further configuration of the part.
        /// </returns>
        public IRegistrationBuilder As(Type contractType)
        {
            this.ContractType = contractType ?? throw new ArgumentNullException(nameof(contractType));
            return this;
        }

        /// <summary>
        /// Mark the part as being shared within the entire composition.
        /// </summary>
        /// <returns>
        /// A part builder allowing further configuration of the part.
        /// </returns>
        IRegistrationBuilder IRegistrationBuilder.Singleton()
        {
            this.Lifetime = AppServiceLifetime.Singleton;
            return this;
        }

        /// <summary>
        /// Mark the part as being shared within the scope.
        /// </summary>
        /// <returns>
        /// A part builder allowing further configuration of the part.
        /// </returns>
        IRegistrationBuilder IRegistrationBuilder.Scoped()
        {
            this.Lifetime = AppServiceLifetime.Scoped;
            return this;
        }

        /// <summary>
        /// Indicates that this service allows multiple registrations.
        /// </summary>
        /// <param name="value">True if multiple service registrations are allowed, false otherwise.</param>
        /// <returns>
        /// A part builder allowing further configuration of the part.
        /// </returns>
        IRegistrationBuilder IRegistrationBuilder.AllowMultiple(bool value)
        {
            this.AllowMultiple = value;
            return this;
        }

        /// <summary>
        /// Select which of the available constructors will be used to instantiate the part.
        /// </summary>
        /// <param name="constructorSelector">Filter that selects a single constructor.</param>
        /// <param name="parameterBuilder">The parameter builder.</param>
        /// <returns>
        /// A part builder allowing further configuration of the part.
        /// </returns>
        IRegistrationBuilder IRegistrationBuilder.SelectConstructor(
            Func<IEnumerable<ConstructorInfo>, ConstructorInfo?> constructorSelector,
            Action<ParameterInfo, IParameterBuilder>? parameterBuilder)
        {
            // TODO not supported.
            if (this.Logger.IsTraceEnabled())
            {
                this.Logger.Warn("Selecting a specific constructor is not supported ({registrationBuilder}).", this);
            }

            return this;
        }

        internal void Build()
        {
            void ConfigureService(IRegistrationBuilder b)
            {
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

                if (this.IsExternallyOwned)
                {
                    b.ExternallyOwned();
                }
            }

            if (this.InstancingStrategy != null)
            {
                this.ambientServices.RegisterService(this.ContractType, this.InstancingStrategy, ConfigureService);

                return;
            }

            throw new InvalidOperationException(string.Format(Strings.LiteRegistrationBuilder_InvalidRegistration_Exception, nameof(this.ImplementationType), nameof(this.Factory)));
        }
    }
}
