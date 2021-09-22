// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SystemCompositionConventionsBuilder.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Conventions builder for MEF.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Injection.SystemComposition.Conventions
{
    using System;
    using System.Collections.Generic;
    using System.Composition.Convention;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Injection;
    using Kephas.Injection.Conventions;

    /// <summary>
    /// Conventions builder for MEF.
    /// </summary>
    public class SystemCompositionConventionsBuilder : IConventionsBuilder, ISystemCompositionConventionBuilderProvider
    {
        /// <summary>
        /// The inner conventions builder.
        /// </summary>
        private readonly ConventionBuilder innerConventionBuilder;

        /// <summary>
        /// The part builders.
        /// </summary>
        private readonly IDictionary<Type, SystemCompositionPartBuilder> partBuilders = new Dictionary<Type, SystemCompositionPartBuilder>();

        /// <summary>
        /// Initializes a new instance of the <see cref="SystemCompositionConventionsBuilder"/> class.
        /// </summary>
        public SystemCompositionConventionsBuilder()
        {
            this.innerConventionBuilder = new ConventionBuilder();
        }

        /// <summary>
        /// Gets the convention builder.
        /// </summary>
        /// <returns>
        /// The convention builder.
        /// </returns>
        public ConventionBuilder GetConventionBuilder()
        {
            return this.innerConventionBuilder;
        }

        /// <summary>
        /// Define a rule that will apply to all types that derive from (or implement) the specified type.
        /// </summary>
        /// <param name="type">The type from which matching types derive.</param>
        /// <returns>A <see cref="IPartConventionsBuilder"/> that must be used to specify the rule.</returns>
        public IPartConventionsBuilder ForTypesDerivedFrom(Type type)
        {
            Requires.NotNull(type, nameof(type));

            return new SystemCompositionPartConventionsBuilder(this.innerConventionBuilder.ForTypesDerivedFrom(type));
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
            Requires.NotNull(typePredicate, nameof(typePredicate));

            return new SystemCompositionPartConventionsBuilder(this.innerConventionBuilder.ForTypesMatching(typePredicate));
        }

        /// <summary>
        /// Define a rule that will apply to the specified type.
        /// </summary>
        /// <param name="type">The type from which matching types derive.</param>
        /// <returns>A <see cref="IPartConventionsBuilder"/> that must be used to specify the rule.</returns>
        public IPartConventionsBuilder ForType(Type type)
        {
            Requires.NotNull(type, nameof(type));

            return new SystemCompositionPartConventionsBuilder(this.innerConventionBuilder.ForType(type));
        }

        /// <summary>
        /// Defines a registration for the specified type and its singleton instance.
        /// </summary>
        /// <param name="type">The registered service type.</param>
        /// <param name="instance">The instance.</param>
        /// <returns>
        /// An IPartBuilder.
        /// </returns>
        public IPartBuilder ForInstance(Type type, object instance)
        {
            Requires.NotNull(type, nameof(type));
            Requires.NotNull(instance, nameof(instance));

            var partBuilder = new SystemCompositionPartBuilder(type, instance);
            partBuilder.Singleton();
            this.partBuilders[type] = partBuilder;
            return partBuilder;
        }

        /// <summary>
        /// Defines a registration for the specified type and its instance factory.
        /// </summary>
        /// <param name="type">The registered service type.</param>
        /// <param name="factory">The service factory.</param>
        /// <returns>A <see cref="IPartBuilder"/> to further configure the rule.</returns>
        public IPartBuilder ForInstanceFactory(Type type, Func<IInjector, object> factory)
        {
            Requires.NotNull(type, nameof(type));
            Requires.NotNull(factory, nameof(factory));

            var partBuilder = new SystemCompositionPartBuilder(type, factory);
            this.partBuilders[type] = partBuilder;

            return partBuilder;
        }

        /// <summary>
        /// Gets the part builders in this collection.
        /// </summary>
        /// <returns>
        /// An enumerator that allows foreach to be used to process the part builders in this collection.
        /// </returns>
        protected internal IEnumerable<SystemCompositionPartBuilder> GetPartBuilders()
        {
            return this.partBuilders.Values;
        }
    }
}