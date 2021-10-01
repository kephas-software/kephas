// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SystemCompositionRegistrationBuilder.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the MEF part builder class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Injection.SystemComposition.Builder
{
    using System;
    using System.Collections.Generic;
    using System.Composition.Hosting;
    using System.Reflection;
    using Kephas.Injection;
    using Kephas.Injection.Builder;
    using Kephas.Injection.SystemComposition.ExportProviders;

    /// <summary>
    /// A MEF part builder.
    /// </summary>
    public class SystemCompositionRegistrationBuilder : ISystemCompositionRegistrationBuilder
    {
        private readonly Func<IInjector, object>? instanceFactory;
        private readonly object? instance;
        private Type? contractType;

        /// <summary>
        /// Initializes a new instance of the <see cref="SystemCompositionRegistrationBuilder"/> class.
        /// </summary>
        /// <param name="instance">The instance.</param>
        public SystemCompositionRegistrationBuilder(object instance)
        {
            this.instance = instance ?? throw new ArgumentNullException(nameof(instance));
            this.IsSingleton = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SystemCompositionRegistrationBuilder"/> class.
        /// </summary>
        /// <param name="instanceFactory">The instance factory.</param>
        public SystemCompositionRegistrationBuilder(Func<IInjector, object> instanceFactory)
        {
            this.instanceFactory = instanceFactory ?? throw new ArgumentNullException(nameof(instanceFactory));
        }

        private bool IsSingleton { get; set; }

        private bool IsScoped { get; set; }

        private IDictionary<string, object?>? Metadata { get; set; }

        /// <summary>
        /// Indicates the type registered as the exported service key.
        /// </summary>
        /// <param name="contractType">Type of the service.</param>
        /// <returns>
        /// A part builder allowing further configuration of the part.
        /// </returns>
        public IRegistrationBuilder As(Type contractType)
        {
            this.contractType = contractType ?? throw new ArgumentNullException(nameof(contractType));
            return this;
        }

        /// <summary>
        /// Mark the part as being shared within the entire composition.
        /// </summary>
        /// <returns>A part builder allowing further configuration of the part.</returns>
        public IRegistrationBuilder Singleton()
        {
            this.IsSingleton = true;
            this.IsScoped = false;
            return this;
        }

        /// <summary>
        /// Mark the part as being shared within the scope.
        /// </summary>
        /// <returns>
        /// A part builder allowing further configuration of the part.
        /// </returns>
        public IRegistrationBuilder Scoped()
        {
            this.IsSingleton = false;
            this.IsScoped = true;
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
            // this is not used
            return this;
        }

        /// <summary>
        /// Select which of the available constructors will be used to instantiate the part.
        /// </summary>
        /// <param name="constructorSelector">Filter that selects a single constructor.</param><param name="importConfiguration">Action configuring the parameters of the selected constructor.</param>
        /// <returns>
        /// A part builder allowing further configuration of the part.
        /// </returns>
        public IRegistrationBuilder SelectConstructor(Func<IEnumerable<ConstructorInfo>, ConstructorInfo?> constructorSelector, Action<ParameterInfo, IImportConventionsBuilder>? importConfiguration = null)
        {
            // simple part builders do not need a constructor.
            return this;
        }

        /// <summary>
        /// Adds metadata to the export.
        /// </summary>
        /// <param name="name">The name of the metadata item.</param>
        /// <param name="value">The metadata value.</param>
        /// <returns>
        /// A part builder allowing further configuration.
        /// </returns>
        public IRegistrationBuilder AddMetadata(string name, object? value)
        {
            (this.Metadata ??= new Dictionary<string, object?>())[name] = value;

            return this;
        }

        /// <summary>
        /// Sets the container up using the configuration.
        /// </summary>
        /// <param name="configuration">The container configuration.</param>
        public void Build(ContainerConfiguration configuration)
        {
            if (this.contractType == null)
            {
                throw new InjectionException($"Contract type not set.");
            }

            configuration.WithProvider(this.instance != null
                ? new FactoryExportDescriptorProvider(
                    this.contractType,
                    () => this.instance,
                    this.Metadata)
                : new FactoryExportDescriptorProvider(
                    this.contractType,
                    ctx => this.instanceFactory!(ctx),
                    this.IsSingleton || this.IsScoped,
                    this.Metadata));
        }
    }
}