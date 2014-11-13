// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConventionsBuilderExtensions.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Extension methods for <see cref="IConventionsBuilder" />.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Conventions
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Extension methods for <see cref="IConventionsBuilder"/>.
    /// </summary>
    public static class ConventionsBuilderExtensions
    {
        /// <summary>
        /// Adds the conventions from the provided types implementing <see cref="IConventionsRegistrar"/>.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="parts">The parts.</param>
        /// <returns>
        /// The convention builder.
        /// </returns>
        public static IConventionsBuilder WithConventions(this IConventionsBuilder builder, IEnumerable<Type> parts)
        {
            Contract.Requires(builder != null);
            Contract.Requires(parts != null);

            return WithConventionsCore(builder, () => parts.Where(IsConventionRegistrar));
        }

        /// <summary>
        /// Adds the conventions from the provided types implementing <see cref="IConventionsRegistrar"/>.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="parts">The parts.</param>
        /// <returns>
        /// The convention builder.
        /// </returns>
        public static IConventionsBuilder WithConventions(this IConventionsBuilder builder, params Type[] parts)
        {
            Contract.Requires(builder != null);
            Contract.Requires(parts != null);

            return WithConventionsCore(builder, () => parts.Where(IsConventionRegistrar));
        }

        /// <summary>
        /// Adds the conventions from types implementing <see cref="IConventionsRegistrar"/> found in the provided assemblies.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="assemblies">The assemblies.</param>
        /// <returns>
        /// The convention builder.
        /// </returns>
        public static IConventionsBuilder WithConventionsFrom(this IConventionsBuilder builder, IEnumerable<Assembly> assemblies)
        {
            Contract.Requires(builder != null);
            Contract.Requires(assemblies != null);

            return WithConventionsCore(builder, () => assemblies.SelectMany(a => a.ExportedTypes.Where(IsConventionRegistrar)));
        }

        /// <summary>
        /// Adds the conventions from types implementing <see cref="IConventionsRegistrar"/> found in the provided assemblies.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="assemblies">The assemblies.</param>
        /// <returns>
        /// The convention builder.
        /// </returns>
        public static IConventionsBuilder WithConventionsFrom(this IConventionsBuilder builder, params Assembly[] assemblies)
        {
            Contract.Requires(builder != null);
            Contract.Requires(assemblies != null);

            return WithConventionsCore(builder, () => assemblies.SelectMany(a => a.ExportedTypes.Where(IsConventionRegistrar)));
        }

        /// <summary>
        /// Gets the registration builder.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="registrarTypesProvider">The registrar types provider.</param>
        /// <returns>
        /// The registration builder.
        /// </returns>
        private static IConventionsBuilder WithConventionsCore(this IConventionsBuilder builder, Func<IEnumerable<Type>> registrarTypesProvider)
        {
            Contract.Requires(builder != null);

            var registrarTypes = registrarTypesProvider();
            var registrars = registrarTypes.Select(t => (IConventionsRegistrar)Activator.CreateInstance(t)).ToList();

            // apply the convention builders
            foreach (var registrar in registrars)
            {
                registrar.RegisterConventions(builder);
            }

            return builder;
        }

        /// <summary>
        /// Determines whether the specified type is an instantiable convention registrar.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns><c>true</c> if the specified type is an instantiable convention registrar, otherwise <c>false</c>.</returns>
        private static bool IsConventionRegistrar(Type type)
        {
            var typeInfo = type.GetTypeInfo();
            var conventionTypeInfo = typeof(IConventionsRegistrar).GetTypeInfo();
            return typeInfo.IsClass && !typeInfo.IsAbstract && conventionTypeInfo.IsAssignableFrom(typeInfo);
        }
    }
}