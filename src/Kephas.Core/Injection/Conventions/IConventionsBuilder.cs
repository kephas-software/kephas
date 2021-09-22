// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IConventionsBuilder.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Contract for conventions builder.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Injection.Conventions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Injection.Hosting;
    using Kephas.Logging;

    /// <summary>
    /// Contract for conventions builder.
    /// </summary>
    public interface IConventionsBuilder
    {
        /// <summary>
        /// Define a rule that will apply to the specified type.
        /// </summary>
        /// <param name="type">The type from which matching types derive.</param>
        /// <returns>A <see cref="IPartConventionsBuilder"/> that must be used to specify the rule.</returns>
        IPartConventionsBuilder ForType(Type type);

        /// <summary>
        /// Defines a registration for the specified type and its singleton instance.
        /// </summary>
        /// <param name="type">The registered service type.</param>
        /// <param name="instance">The instance.</param>
        /// <returns>A <see cref="IPartBuilder"/> to further configure the rule.</returns>
        IPartBuilder ForInstance(Type type, object instance);

        /// <summary>
        /// Defines a registration for the specified type and its instance factory.
        /// </summary>
        /// <param name="type">The registered service type.</param>
        /// <param name="factory">The service factory.</param>
        /// <returns>A <see cref="IPartBuilder"/> to further configure the rule.</returns>
        IPartBuilder ForInstanceFactory(Type type, Func<IInjector, object> factory);

        /// <summary>
        /// Adds the conventions from the provided types implementing
        /// <see cref="IConventionsRegistrar" />.
        /// </summary>
        /// <param name="buildContext">Context for the registration.</param>
        /// <returns>
        /// The convention builder.
        /// </returns>
        public IConventionsBuilder RegisterConventions(IInjectionBuildContext buildContext)
        {
            Requires.NotNull(buildContext, nameof(buildContext));

            var builder = this;
            var conventionRegistrars = buildContext.AmbientServices.GetService<IEnumerable<IConventionsRegistrar>>();
            var registrars = conventionRegistrars?.ToList() ?? new List<IConventionsRegistrar>();

            var logger = this.GetType().GetLogger(buildContext);

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