// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConventionsBuilderExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Extension methods for <see cref="IConventionsBuilder" />.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Injection.Conventions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Injection.Hosting;
    using Kephas.Logging;

    /// <summary>
    /// Extension methods for <see cref="IConventionsBuilder"/>.
    /// </summary>
    public static class ConventionsBuilderExtensions
    {
        /// <summary>
        /// Adds the conventions from the provided types implementing
        /// <see cref="IConventionsRegistrar" />.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="buildContext">Context for the registration.</param>
        /// <returns>
        /// The convention builder.
        /// </returns>
        public static IConventionsBuilder RegisterConventions(this IConventionsBuilder builder, IInjectionBuildContext buildContext)
        {
            Requires.NotNull(builder, nameof(builder));
            Requires.NotNull(buildContext, nameof(buildContext));

            return RegisterConventionsCore(builder, buildContext);
        }

        /// <summary>
        /// Adds the conventions from types implementing <see cref="IConventionsRegistrar" /> found in
        /// the provided assemblies.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="assemblies">The assemblies.</param>
        /// <param name="buildContext">Context for the registration.</param>
        /// <returns>
        /// The convention builder.
        /// </returns>
        public static IConventionsBuilder RegisterConventionsFrom(this IConventionsBuilder builder, IEnumerable<Assembly> assemblies, IInjectionBuildContext buildContext)
        {
            Requires.NotNull(builder, nameof(builder));
            Requires.NotNull(assemblies, nameof(assemblies));
            Requires.NotNull(buildContext, nameof(buildContext));

            return RegisterConventionsCore(builder, buildContext);
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
        /// <param name="buildContext">The registration context.</param>
        /// <returns>
        /// The registration builder.
        /// </returns>
        private static IConventionsBuilder RegisterConventionsCore(this IConventionsBuilder builder, IInjectionBuildContext buildContext)
        {
            Requires.NotNull(builder, nameof(builder));

            var conventionRegistrars = buildContext.AmbientServices.GetService<IEnumerable<IConventionsRegistrar>>();
            var registrars = conventionRegistrars?.ToList() ?? new List<IConventionsRegistrar>();

            var logger = typeof(ConventionsBuilderExtensions).GetLogger(buildContext);

            // apply the convention builders
            foreach (var registrar in registrars)
            {
                if (logger.IsDebugEnabled())
                {
                    logger.Debug("Registering conventions from '{conventionsRegistrar}...", registrar.GetType());
                }

                registrar.RegisterConventions(builder, buildContext);
            }

            return builder;
        }
    }
}