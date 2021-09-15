// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LiteConventionsBuilder.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the lite conventions builder class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Kephas.Injection.Conventions;

namespace Kephas.Injection.Lite.Conventions
{
    /// <summary>
    /// A lightweight conventions builder.
    /// </summary>
    internal class LiteConventionsBuilder : IConventionsBuilder
    {
        public const string LiteCompositionKey = "__LiteComposition";

        private readonly IAmbientServices ambientServices;

        private IList<LiteRegistrationBuilder> descriptorBuilders = new List<LiteRegistrationBuilder>();

        /// <summary>
        /// Initializes a new instance of the <see cref="LiteConventionsBuilder"/> class.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        public LiteConventionsBuilder(IAmbientServices ambientServices)
        {
            this.ambientServices = ambientServices;
            this.ambientServices[LiteCompositionKey] = true;
        }

        /// <summary>
        /// Defines a registration for the specified type and its singleton instance.
        /// </summary>
        /// <param name="type">The registered service type.</param>
        /// <param name="instance">The instance.</param>
        /// <returns>
        /// A <see cref="IPartBuilder"/> to further configure the rule.
        /// </returns>
        public IPartBuilder ForInstance(Type type, object instance)
        {
            var descriptorBuilder = new LiteRegistrationBuilder(this.ambientServices)
            {
                ServiceType = type,
                Instance = instance,
            };
            this.descriptorBuilders.Add(descriptorBuilder);
            return new LitePartBuilder(descriptorBuilder);
        }

        /// <summary>
        /// Defines a registration for the specified type and its instance factory.
        /// </summary>
        /// <param name="type">The registered service type.</param>
        /// <param name="factory">The service factory.</param>
        /// <returns>
        /// A <see cref="IPartBuilder"/> to further configure the rule.
        /// </returns>
        public IPartBuilder ForInstanceFactory(Type type, Func<IInjector, object> factory)
        {
            var descriptorBuilder = new LiteRegistrationBuilder(this.ambientServices)
            {
                ServiceType = type,
                Factory = factory,
            };
            this.descriptorBuilders.Add(descriptorBuilder);
            return new LitePartBuilder(descriptorBuilder);
        }

        /// <summary>
        /// Define a rule that will apply to the specified type.
        /// </summary>
        /// <param name="type">The type from which matching types derive.</param>
        /// <returns>
        /// A <see cref="IPartConventionsBuilder"/> that must be used to specify the rule.
        /// </returns>
        public IPartConventionsBuilder ForType(Type type)
        {
            var descriptorBuilder = new LiteRegistrationBuilder(this.ambientServices)
            {
                ImplementationType = type,
            };
            this.descriptorBuilders.Add(descriptorBuilder);
            return new LitePartConventionsBuilder(this.ambientServices.LogManager, descriptorBuilder);
        }

        /// <summary>
        /// Define a rule that will apply to all types that derive from (or implement) the specified type.
        /// </summary>
        /// <param name="type">The type from which matching types derive.</param>
        /// <returns>
        /// A <see cref="IPartConventionsBuilder"/> that must be used to specify the rule.
        /// </returns>
        public IPartConventionsBuilder ForTypesDerivedFrom(Type type)
        {
            return this.ForTypesMatching(t => t.IsClass && !t.IsAbstract && !ReferenceEquals(type, t) && type.IsAssignableFrom(t));
        }

        /// <summary>
        /// Define a rule that will apply to all types that derive from (or implement) the specified type.
        /// </summary>
        /// <param name="typePredicate">The type predicate.</param>
        /// <returns>
        /// A <see cref="IPartConventionsBuilder" /> that must be used to specify the rule.
        /// </returns>
        public IPartConventionsBuilder ForTypesMatching(Predicate<Type> typePredicate)
        {
            var descriptorBuilder = new LiteRegistrationBuilder(this.ambientServices)
            {
                ImplementationTypePredicate = typePredicate,
            };
            this.descriptorBuilders.Add(descriptorBuilder);
            return new LitePartConventionsBuilder(this.ambientServices.LogManager, descriptorBuilder);
        }

        /// <summary>
        /// Builds the container using the given parts.
        /// </summary>
        /// <param name="parts">The parts.</param>
        public void Build(IEnumerable<Type> parts)
        {
            foreach (var descriptorBuilder in this.descriptorBuilders)
            {
                descriptorBuilder.Build(parts);
            }
        }
    }
}
