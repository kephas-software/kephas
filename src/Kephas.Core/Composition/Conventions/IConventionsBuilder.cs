// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IConventionsBuilder.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Contract for conventions builder.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Conventions
{
    using System;

    /// <summary>
    /// Contract for conventions builder.
    /// </summary>
    public interface IConventionsBuilder
    {
        /// <summary>
        /// Define a rule that will apply to all types that derive from (or implement) the specified type.
        /// </summary>
        /// <param name="type">The type from which matching types derive.</param>
        /// <returns>A <see cref="IPartConventionsBuilder"/> that must be used to specify the rule.</returns>
        IPartConventionsBuilder ForTypesDerivedFrom(Type type);

        /// <summary>
        /// Define a rule that will apply to all types that derive from (or implement) the specified type.
        /// </summary>
        /// <param name="typePredicate">The type predicate.</param>
        /// <returns>
        /// A <see cref="IPartConventionsBuilder" /> that must be used to specify the rule.
        /// </returns>
        IPartConventionsBuilder ForTypesMatching(Predicate<Type> typePredicate);

        /// <summary>
        /// Define a rule that will apply to the specified type.
        /// </summary>
        /// <param name="type">The type from which matching types derive.</param>
        /// <returns>A <see cref="IPartConventionsBuilder"/> that must be used to specify the rule.</returns>
        IPartConventionsBuilder ForType(Type type);
    }
}