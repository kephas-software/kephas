// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SystemCompositionTypeRegistrationBuilder.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Conventions builder for a specific part.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Injection.SystemComposition.Builder
{
    using System;
    using System.Collections.Generic;
    using System.Composition.Convention;
    using System.Composition.Hosting;
    using System.Linq;
    using System.Reflection;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Injection;
    using Kephas.Injection.AttributedModel;
    using Kephas.Injection.Builder;
    using Kephas.Injection.SystemComposition.Resources;
    using Kephas.Logging;

    /// <summary>
    /// Conventions builder for a specific part.
    /// </summary>
    public class SystemCompositionTypeRegistrationBuilder : Loggable, ISystemCompositionRegistrationBuilder
    {
        private readonly PartConventionBuilder innerConventionBuilder;
        private readonly Type serviceType;
        private Type? contractType;
        private IDictionary<string, object?>? metadata;

        /// <summary>
        /// Initializes a new instance of the <see cref="SystemCompositionTypeRegistrationBuilder"/> class.
        /// </summary>
        /// <param name="innerConventionBuilder">The inner convention builder.</param>
        /// <param name="serviceType">The service type.</param>
        /// <param name="logManager">Optional. The log manager.</param>
        internal SystemCompositionTypeRegistrationBuilder(
            PartConventionBuilder innerConventionBuilder,
            Type serviceType,
            ILogManager? logManager = null)
            : base(logManager)
        {
            this.innerConventionBuilder = innerConventionBuilder ?? throw new ArgumentNullException(nameof(innerConventionBuilder));
            this.serviceType = serviceType;
        }

        /// <summary>
        /// Indicates the declared service type. Typically this is the same as the contract type, but
        /// this may get overwritten, for example when declaring generic type services for collecting
        /// metadata.
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
            this.innerConventionBuilder.Shared();
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
            this.innerConventionBuilder.Shared(InjectionScopeNames.Default);
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
        public IRegistrationBuilder SelectConstructor(
            Func<IEnumerable<ConstructorInfo>, ConstructorInfo?> constructorSelector,
            Action<ParameterInfo, IParameterBuilder>? parameterBuilder = null)
        {
            Requires.NotNull(constructorSelector, nameof(constructorSelector));

            ConstructorInfo NewConstructorSelector(IEnumerable<ConstructorInfo> ctorInfos)
            {
                var ctor = constructorSelector(ctorInfos);

                if (ctor == null)
                {
                    var constructorsList = ctorInfos.Where(c => !c.IsStatic && c.IsPublic).ToList();

                    if (constructorsList.Count == 0)
                    {
                        throw new InjectionException(string.Format(Strings.SystemCompositionPartConventionsBuilder_MissingCompositionConstructor, ctorInfos.FirstOrDefault()?.DeclaringType?.ToString() ?? "<unknown>", this.contractType));
                    }

                    if (constructorsList.Count == 1)
                    {
                        return constructorsList[0];
                    }

                    var sortedConstructors = constructorsList.ToDictionary(c => c, c => c.GetParameters().Length).OrderByDescending(kv => kv.Value).ToList();
                    if (sortedConstructors[0].Value == sortedConstructors[1].Value)
                    {
                        throw new InjectionException(string.Format(Strings.SystemCompositionPartConventionsBuilder_AmbiguousCompositionConstructor, constructorsList.First().DeclaringType, typeof(InjectConstructorAttribute)));
                    }

                    return sortedConstructors[0].Key;
                }

                return ctor;
            }

            if (parameterBuilder == null)
            {
                this.innerConventionBuilder.SelectConstructor(NewConstructorSelector);
            }
            else
            {
                this.innerConventionBuilder.SelectConstructor(
                    NewConstructorSelector,
                    (pi, config) => parameterBuilder(pi, new SystemCompositionParameterBuilder(config)));
            }

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
            (this.metadata ??= new Dictionary<string, object?>())[name] = value;

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
            this.Logger.Warn($"{nameof(SystemCompositionTypeRegistrationBuilder)} does not support externally owned instances, as it creates them.");
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
                throw new InvalidOperationException("The contract type is not set");
            }

            configuration.WithPart(this.serviceType);
            this.innerConventionBuilder.Export(builder =>
            {
                builder.AsContractType(this.contractType);
                if (this.metadata != null)
                {
                    foreach (var (name, value) in this.metadata)
                    {
                        builder.AddMetadata(name, value);
                    }
                }
            });
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
            // not used, by default all services allow multiple instances.
            return this;
        }
    }
}