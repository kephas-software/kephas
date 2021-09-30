// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IInjectorBuilder.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IInjectorBuilder interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Injection.Hosting
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Kephas.Injection.Conventions;
    using Kephas.Logging;

    /// <summary>
    /// Contract for injection builders.
    /// </summary>
    public interface IInjectorBuilder
    {
        /// <summary>
        /// Creates the injector.
        /// </summary>
        /// <returns>The newly created injector.</returns>
        IInjector Build();

        /// <summary>
        /// Define a rule that will apply to the specified type.
        /// </summary>
        /// <param name="type">The type from which matching types derive.</param>
        /// <returns>A <see cref="IPartBuilder"/> that must be used to specify the rule.</returns>
        IPartBuilder ForType(Type type);

        /// <summary>
        /// Defines a registration for the specified type and its singleton instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns>A <see cref="IPartBuilder"/> to further configure the rule.</returns>
        IPartBuilder ForInstance(object instance);

        /// <summary>
        /// Defines a registration for the specified type and its instance factory.
        /// </summary>
        /// <param name="type">The registered service type.</param>
        /// <param name="factory">The service factory.</param>
        /// <returns>A <see cref="IPartBuilder"/> to further configure the rule.</returns>
        IPartBuilder ForFactory(Type type, Func<IInjector, object> factory);
    }
}