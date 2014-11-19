// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MefConventionsBuilder.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Conventions builder for MEF.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Mef.Conventions
{
    using System;
    using System.Composition.Convention;

    using Kephas.Composition.Conventions;

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
            return new MefPartConventionsBuilder(this.innerConventionBuilder.ForTypesMatching(typePredicate));
        }

        /// <summary>
        /// Define a rule that will apply to the specified type.
        /// </summary>
        /// <param name="type">The type from which matching types derive.</param>
        /// <returns>A <see cref="IPartConventionsBuilder"/> that must be used to specify the rule.</returns>
        public IPartConventionsBuilder ForType(Type type)
        {
            return new MefPartConventionsBuilder(this.innerConventionBuilder.ForType(type));
        }
    }
}