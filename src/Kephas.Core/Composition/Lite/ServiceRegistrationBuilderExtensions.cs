// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceRegistrationBuilderExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the service registration builder extensions class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Lite
{
    using System;
    using Kephas.Diagnostics.Contracts;

    /// <summary>
    /// Extensions for the service registration builder.
    /// </summary>
    public static class ServiceRegistrationBuilderExtensions
    {
        /// <summary>
        /// Registers the service with the provided factory.
        /// </summary>
        /// <param name="builder">The builder to act on.</param>
        /// <param name="factory">The factory.</param>
        /// <returns>
        /// This builder.
        /// </returns>
        public static IServiceRegistrationBuilder WithFactory(this IServiceRegistrationBuilder builder, Func<object> factory)
        {
            Requires.NotNull(builder, nameof(builder));
            Requires.NotNull(factory, nameof(factory));

            return builder.WithFactory(ctx => factory());
        }

        /// <summary>
        /// Registers the service with the provided implementation type.
        /// </summary>
        /// <typeparam name="TImplementation">Type of the implementation.</typeparam>
        /// <param name="builder">The builder to act on.</param>
        /// <returns>
        /// This builder.
        /// </returns>
        public static IServiceRegistrationBuilder WithType<TImplementation>(this IServiceRegistrationBuilder builder)
        {
            Requires.NotNull(builder, nameof(builder));

            return builder.WithType(typeof(TImplementation));
        }

        /// <summary>
        /// Sets the registration type to a super type of the service type.
        /// </summary>
        /// <remarks>
        /// The registration type is the key to find the service. The registered service type is a
        /// subtype providing additional information, typically metadata.
        /// </remarks>
        /// <typeparam name="TContract">The type of registration.</typeparam>
        /// <param name="builder">The builder to act on.</param>
        /// <returns>
        /// This builder.
        /// </returns>
        public static IServiceRegistrationBuilder RegisterAs<TContract>(this IServiceRegistrationBuilder builder)
        {
            Requires.NotNull(builder, nameof(builder));

            return builder.RegisterAs(typeof(TContract));
        }
    }
}