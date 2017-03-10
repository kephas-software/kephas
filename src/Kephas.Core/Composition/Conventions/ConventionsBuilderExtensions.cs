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

    using Kephas.Composition.AttributedModel;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Logging;
    using Kephas.Reflection;
    using Kephas.Runtime;
    using Kephas.Services;

    /// <summary>
    /// Extension methods for <see cref="IConventionsBuilder"/>.
    /// </summary>
    public static class ConventionsBuilderExtensions
    {
        /// <summary>
        /// Information describing the convention registrar contract type.
        /// </summary>
        private static readonly TypeInfo ConventionRegistrarContractTypeInfo = typeof(IConventionsRegistrar).GetTypeInfo();

        /// <summary>
        /// Adds the conventions from the provided types implementing
        /// <see cref="IConventionsRegistrar" />.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="conventionTypes">The convention types.</param>
        /// <param name="parts">The parts.</param>
        /// <param name="registrationContext">Context for the registration.</param>
        /// <returns>
        /// The convention builder.
        /// </returns>
        public static IConventionsBuilder RegisterConventions(this IConventionsBuilder builder, IEnumerable<Type> conventionTypes, IEnumerable<Type> parts, IContext registrationContext)
        {
            Requires.NotNull(builder, nameof(builder));
            Contract.Requires(conventionTypes != null);

            return RegisterConventionsCore(builder, () => conventionTypes.Where(IsConventionRegistrar).Select(t => t.AsRuntimeTypeInfo()), parts, registrationContext);
        }

        /// <summary>
        /// Adds the conventions from types implementing <see cref="IConventionsRegistrar" /> found in
        /// the provided assemblies.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="assemblies">The assemblies.</param>
        /// <param name="parts">The parts.</param>
        /// <param name="registrationContext">Context for the registration.</param>
        /// <returns>
        /// The convention builder.
        /// </returns>
        public static IConventionsBuilder RegisterConventionsFrom(this IConventionsBuilder builder, IEnumerable<Assembly> assemblies, IEnumerable<Type> parts, IContext registrationContext)
        {
            Requires.NotNull(builder, nameof(builder));
            Contract.Requires(assemblies != null);
            Contract.Requires(parts != null);

            return RegisterConventionsCore(builder, () => parts.Where(IsConventionRegistrar).Select(t => t.AsRuntimeTypeInfo()), parts, registrationContext);
        }

        /// <summary>
        /// Gets the registration builder.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="registrarTypesProvider">The registrar types provider.</param>
        /// <param name="parts">The convention types.</param>
        /// <param name="registrationContext">The registration context.</param>
        /// <returns>
        /// The registration builder.
        /// </returns>
        private static IConventionsBuilder RegisterConventionsCore(this IConventionsBuilder builder, Func<IEnumerable<IRuntimeTypeInfo>> registrarTypesProvider, IEnumerable<Type> parts, IContext registrationContext)
        {
            Requires.NotNull(builder, nameof(builder));

            var partInfos = parts.Select(p => p.GetTypeInfo()).ToList();
            var registrarTypes = registrarTypesProvider();
            var registrars = registrarTypes.Select(t => (IConventionsRegistrar)t.CreateInstance()).ToList();
            var logger = typeof(ConventionsBuilderExtensions).GetLogger(registrationContext);

            // apply the convention builders
            foreach (var registrar in registrars)
            {
                if (logger.IsDebugEnabled())
                {
                    logger.Debug($"Registering conventions from '{registrar.GetType()}...");
                }

                registrar.RegisterConventions(builder, partInfos, registrationContext);
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
            return typeInfo.IsClass 
                    && !typeInfo.IsAbstract 
                    && ConventionRegistrarContractTypeInfo.IsAssignableFrom(typeInfo)
                    && typeInfo.GetCustomAttribute<ExcludeFromCompositionAttribute>() == null;
        }
    }
}