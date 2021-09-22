// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConventionsBuilderExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Extension methods for <see cref="IConventionsBuilder" />.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Kephas.Collections;
using Kephas.Diagnostics.Contracts;
using Kephas.Injection.Hosting;
using Kephas.Logging;

namespace Kephas.Injection.Conventions
{
    /// <summary>
    /// Extension methods for <see cref="IConventionsBuilder"/>.
    /// </summary>
    public static class ConventionsBuilderExtensions
    {
        /// <summary>
        /// Defines a registration for the specified type and its singleton instance.
        /// </summary>
        /// <typeparam name="TService">Type of the registered service.</typeparam>
        /// <param name="conventionsBuilder">The conventionsBuilder to act on.</param>
        /// <param name="instance">The singleton instance.</param>
        public static void ForInstance<TService>(this IConventionsBuilder conventionsBuilder, TService instance)
        {
            Requires.NotNull(conventionsBuilder, nameof(conventionsBuilder));

            conventionsBuilder.ForInstance(typeof(TService), instance);
        }

        /// <summary>
        /// Defines a registration for the specified type and its instance factory.
        /// </summary>
        /// <typeparam name="TService">Type of the registered service.</typeparam>
        /// <param name="conventionsBuilder">The conventionsBuilder to act on.</param>
        /// <param name="factory">The instance factory.</param>
        /// <returns>
        /// A <see cref="IPartBuilder"/> to further configure the rule.
        /// </returns>
        public static IPartBuilder ForInstanceFactory<TService>(this IConventionsBuilder conventionsBuilder, Func<IInjector, TService> factory)
        {
            Requires.NotNull(conventionsBuilder, nameof(conventionsBuilder));

            return conventionsBuilder.ForInstanceFactory(typeof(TService), c => factory(c));
        }

        /// <summary>
        /// Adds the conventions from the provided types implementing
        /// <see cref="IConventionsRegistrar" />.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="parts">The parts.</param>
        /// <param name="buildContext">Context for the registration.</param>
        /// <returns>
        /// The convention builder.
        /// </returns>
        public static IConventionsBuilder RegisterConventions(this IConventionsBuilder builder, IList<Type> parts, IInjectionBuildContext buildContext)
        {
            Requires.NotNull(builder, nameof(builder));
            Requires.NotNull(parts, nameof(parts));
            Requires.NotNull(buildContext, nameof(buildContext));

            return RegisterConventionsCore(builder, parts, buildContext);
        }

        /// <summary>
        /// Adds the conventions from types implementing <see cref="IConventionsRegistrar" /> found in
        /// the provided assemblies.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="assemblies">The assemblies.</param>
        /// <param name="parts">The parts.</param>
        /// <param name="buildContext">Context for the registration.</param>
        /// <returns>
        /// The convention builder.
        /// </returns>
        public static IConventionsBuilder RegisterConventionsFrom(this IConventionsBuilder builder, IEnumerable<Assembly> assemblies, IList<Type> parts, IInjectionBuildContext buildContext)
        {
            Requires.NotNull(builder, nameof(builder));
            Requires.NotNull(assemblies, nameof(assemblies));
            Requires.NotNull(parts, nameof(parts));
            Requires.NotNull(buildContext, nameof(buildContext));

            return RegisterConventionsCore(builder, parts, buildContext);
        }

        /// <summary>
        /// Gets a value indicating whether the type is part candidate.
        /// </summary>
        /// <param name="potentialCandidate">The potential candidate type.</param>
        /// <returns>
        /// True if the type is a part candidate, false if not.
        /// </returns>
        internal static bool IsPartCandidate(this Type potentialCandidate)
        {
            return potentialCandidate.IsInterface
                || (potentialCandidate.IsClass && !(potentialCandidate.IsAbstract && potentialCandidate.IsSealed));
        }

        /// <summary>
        /// Gets the registration builder.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="parts">The convention types.</param>
        /// <param name="buildContext">The registration context.</param>
        /// <returns>
        /// The registration builder.
        /// </returns>
        private static IConventionsBuilder RegisterConventionsCore(this IConventionsBuilder builder, IList<Type> parts, IInjectionBuildContext buildContext)
        {
            Requires.NotNull(builder, nameof(builder));

            if (buildContext.Parts != null)
            {
                parts.AddRange(buildContext.Parts.Where(IsPartCandidate));
            }

            var conventionRegistrars = buildContext.AmbientServices.GetService<IEnumerable<IConventionsRegistrar>>();
            var registrars = conventionRegistrars?.ToList() ?? new List<IConventionsRegistrar>();
            if (buildContext.Registrars != null)
            {
                registrars.AddRange(buildContext.Registrars);
            }

            var logger = typeof(ConventionsBuilderExtensions).GetLogger(buildContext);

            // apply the convention builders
            foreach (var registrar in registrars)
            {
                if (logger.IsDebugEnabled())
                {
                    logger.Debug("Registering conventions from '{conventionsRegistrar}...", registrar.GetType());
                }

                registrar.RegisterConventions(builder, parts, buildContext);
            }

            return builder;
        }
    }
}