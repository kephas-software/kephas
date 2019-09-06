// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RegistrationBuilder.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the registration builder class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Lite.Conventions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Kephas.Composition.Conventions;
    using Kephas.Logging;
    using Kephas.Resources;
    using Kephas.Services;

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
        /// Gets or sets the factory.
        /// </summary>
        /// <value>
        /// A function delegate that yields an object.
        /// </value>
        public Func<ICompositionContext, object> Factory { get; set; }

        /// <summary>
        /// Gets or sets the type of the implementation.
        /// </summary>
        /// <value>
        /// The type of the implementation.
        /// </value>
        public Type ImplementationType { get; set; }

        /// <summary>
        /// Gets or sets the implementation type predicate.
        /// </summary>
        /// <value>
        /// The implementation type predicate.
        /// </value>
        public Predicate<Type> ImplementationTypePredicate { get; set; }

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
        public Action<Type, IExportConventionsBuilder> ExportConfiguration { get; set; }

        /// <summary>
        /// Gets a value indicating whether we allow multiple.
        /// </summary>
        /// <value>
        /// True if allow multiple, false if not.
        /// </value>
        public bool AllowMultiple { get; private set; }

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
            this.Logger.Debug($"Metadata {name} is automatically added for {this}.");
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
            this.Logger.Debug($"Metadata {name} is automatically added for {this}.");
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
            this.ServiceType = contractType;
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
                                       ?? (this.ImplementationTypePredicate != null ? "type predicate" :
                                           this.Factory != null ? "factory" : "unknown");
            return $"{this.ServiceType}/{this.Lifetime}/{implementationString}";
        }


        internal void Build(IEnumerable<Type> parts)
        {
            void ConfigureSingleService(IServiceRegistrationBuilder b)
            {
                if (this.ImplementationType != null)
                {
                    b.WithType(this.ImplementationType);
                }
                else if (this.Factory != null)
                {
                    b.WithFactory(this.Factory);
                }

                if (this.Lifetime == AppServiceLifetime.Singleton)
                {
                    b.AsSingleton();
                }
                else if (this.Lifetime == AppServiceLifetime.Transient)
                {
                    b.AsTransient();
                }
                else if (this.Lifetime == AppServiceLifetime.Scoped)
                {
                    throw new NotSupportedException("Scoped services not supported");
                }

                if (this.AllowMultiple)
                {
                    b.AllowMultiple();
                }
            }

            if (this.ImplementationType != null || this.Factory != null)
            {
                this.ExportConfiguration?.Invoke(this.ImplementationType, this);
                this.ambientServices.Register(this.ServiceType, ConfigureSingleService);

                return;
            }

            if (this.ImplementationTypePredicate != null)
            {
                var isGenericTypeDefinition = this.ServiceType?.IsGenericTypeDefinition ?? false;
                var genericServiceType = this.ServiceType;
                this.AllowMultiple = !isGenericTypeDefinition;

                foreach (var type in parts.Where(t => this.ImplementationTypePredicate(t)))
                {
                    this.ImplementationType = type;
                    if (isGenericTypeDefinition && !type.IsGenericTypeDefinition)
                    {
                        var constructedServiceType = type.GetInterfaces().FirstOrDefault(i => i.GetGenericTypeDefinition() == genericServiceType);
                        if (constructedServiceType == null)
                        {
                            throw new InvalidOperationException(string.Format(Strings.LiteRegistrationBuilder_CannotIdentifyConstructedServiceType_Exception, genericServiceType, type));
                        }

                        this.ExportConfiguration?.Invoke(constructedServiceType, this);
                    }
                    else
                    {
                        this.ExportConfiguration?.Invoke(type, this);
                    }

                    this.ambientServices.Register(this.ServiceType, ConfigureSingleService);
                }

                return;
            }

            throw new InvalidOperationException(string.Format(Strings.LiteRegistrationBuilder_InvalidRegistration_Exception, nameof(this.ImplementationType), nameof(this.ImplementationTypePredicate), nameof(this.Factory)));
        }
    }
}
