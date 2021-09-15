// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MefConventionsBuilder.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Conventions builder for MEF.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Injection;
using Kephas.Injection.Conventions;

namespace Kephas.Composition.Mef.Conventions
{
    using System;
    using System.Collections.Generic;
    using System.Composition.Convention;
    using Kephas.Diagnostics.Contracts;

    /// <summary>
    /// Conventions builder for MEF.
    /// </summary>
    public class MefConventionsBuilder : IConventionsBuilder, IMefConventionBuilderProvider
    {
        /// <summary>
        /// The inner conventions builder.
        /// </summary>
        private readonly ConventionBuilder innerConventionBuilder;

        /// <summary>
        /// The part builders.
        /// </summary>
        private readonly IDictionary<Type, MefPartBuilder> partBuilders = new Dictionary<Type, MefPartBuilder>();

        /// <summary>
        /// Initializes a new instance of the <see cref="MefConventionsBuilder"/> class.
        /// </summary>
        public MefConventionsBuilder()
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

            return new MefPartConventionsBuilder(this.innerConventionBuilder.ForTypesDerivedFrom(type));
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

            return new MefPartConventionsBuilder(this.innerConventionBuilder.ForTypesMatching(typePredicate));
        }

        /// <summary>
        /// Define a rule that will apply to the specified type.
        /// </summary>
        /// <param name="type">The type from which matching types derive.</param>
        /// <returns>A <see cref="IPartConventionsBuilder"/> that must be used to specify the rule.</returns>
        public IPartConventionsBuilder ForType(Type type)
        {
            Requires.NotNull(type, nameof(type));

            return new MefPartConventionsBuilder(this.innerConventionBuilder.ForType(type));
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

            var partBuilder = new MefPartBuilder(type, instance);
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

            var partBuilder = new MefPartBuilder(type, factory);
            this.partBuilders[type] = partBuilder;

            return partBuilder;
        }

        /// <summary>
        /// Gets the part builders in this collection.
        /// </summary>
        /// <returns>
        /// An enumerator that allows foreach to be used to process the part builders in this collection.
        /// </returns>
        protected internal IEnumerable<MefPartBuilder> GetPartBuilders()
        {
            return this.partBuilders.Values;
        }
    }
}