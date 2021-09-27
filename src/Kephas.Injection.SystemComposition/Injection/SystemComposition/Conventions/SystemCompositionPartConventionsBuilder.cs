// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SystemCompositionPartConventionsBuilder.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Conventions builder for a specific part.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Injection.SystemComposition.Conventions
{
    using System;
    using System.Collections.Generic;
    using System.Composition.Convention;
    using System.Linq;
    using System.Reflection;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Injection;
    using Kephas.Injection.AttributedModel;
    using Kephas.Injection.Conventions;
    using Kephas.Injection.SystemComposition.Resources;

    /// <summary>
    /// Conventions builder for a specific part.
    /// </summary>
    public class SystemCompositionPartConventionsBuilder : IPartBuilder
    {
        /// <summary>
        /// The inner convention builder.
        /// </summary>
        private readonly PartConventionBuilder innerConventionBuilder;

        private Type? contractType;
        private Type? serviceType;

        /// <summary>
        /// Initializes a new instance of the <see cref="SystemCompositionPartConventionsBuilder"/> class.
        /// </summary>
        /// <param name="innerConventionBuilder">The inner convention builder.</param>
        internal SystemCompositionPartConventionsBuilder(PartConventionBuilder innerConventionBuilder)
        {
            Requires.NotNull(innerConventionBuilder, nameof(innerConventionBuilder));

            this.innerConventionBuilder = innerConventionBuilder;
            this.innerConventionBuilder.Export(builder => builder.AsContractType(this.serviceType));
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
        public IPartBuilder As(Type contractType)
        {
            Requires.NotNull(contractType, nameof(contractType));

            this.serviceType = contractType;
            return this;
        }

        /// <summary>
        /// Mark the part as being shared within the entire composition.
        /// </summary>
        /// <returns>A part builder allowing further configuration of the part.</returns>
        public IPartBuilder Singleton()
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
        public IPartBuilder Scoped()
        {
            this.innerConventionBuilder.Shared(InjectionScopeNames.Default);
            return this;
        }

        /// <summary>
        /// Select which of the available constructors will be used to instantiate the part.
        /// </summary>
        /// <param name="constructorSelector">Filter that selects a single constructor.</param><param name="importConfiguration">Action configuring the parameters of the selected constructor.</param>
        /// <returns>
        /// A part builder allowing further configuration of the part.
        /// </returns>
        public IPartBuilder SelectConstructor(Func<IEnumerable<ConstructorInfo>, ConstructorInfo?> constructorSelector, Action<ParameterInfo, IImportConventionsBuilder>? importConfiguration = null)
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

            if (importConfiguration == null)
            {
                this.innerConventionBuilder.SelectConstructor(NewConstructorSelector);
            }
            else
            {
                this.innerConventionBuilder.SelectConstructor(
                    NewConstructorSelector,
                    (pi, config) => importConfiguration(pi, new SystemCompositionImportConventionsBuilder(config)));
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
        public IPartBuilder AddMetadata(string name, object? value)
        {
            this.innerConventionBuilder.AddPartMetadata(name, value);

            return this;
        }

        /// <summary>
        /// Indicates that this service allows multiple registrations.
        /// </summary>
        /// <param name="value">True if multiple service registrations are allowed, false otherwise.</param>
        /// <returns>
        /// A part builder allowing further configuration of the part.
        /// </returns>
        IPartBuilder IPartBuilder.AllowMultiple(bool value)
        {
            // not used, by default all services allow multiple instances.
            return this;
        }
    }
}