﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceRegistrationBuilderExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the service registration builder extensions class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Injection.Lite.Builder
{
    using System;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Injection.Builder;
    using Kephas.Services;

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
            builder = builder ?? throw new ArgumentNullException(nameof(builder));
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
            builder = builder ?? throw new ArgumentNullException(nameof(builder));

            return builder.WithType(typeof(TImplementation));
        }

        /// <summary>
        /// Sets the processing priority for the registered service.
        /// </summary>
        /// <param name="builder">The builder to act on.</param>
        /// <param name="priority">The priority value.</param>
        /// <returns>
        /// This builder.
        /// </returns>
        public static IRegistrationBuilder ProcessingPriority(this IRegistrationBuilder builder, Priority priority)
        {
            builder = builder ?? throw new ArgumentNullException(nameof(builder));

            return builder.AddMetadata(nameof(IHasProcessingPriority.ProcessingPriority), priority);
        }

        /// <summary>
        /// Sets the override priority for the registered service.
        /// </summary>
        /// <param name="builder">The builder to act on.</param>
        /// <param name="priority">The priority value.</param>
        /// <returns>
        /// This builder.
        /// </returns>
        public static IRegistrationBuilder OverridePriority(this IRegistrationBuilder builder, Priority priority)
        {
            builder = builder ?? throw new ArgumentNullException(nameof(builder));

            return builder.AddMetadata(nameof(IHasOverridePriority.OverridePriority), priority);
        }

        /// <summary>
        /// Sets the <see cref="AppServiceMetadata.IsOverride"/> metadata the registered service.
        /// </summary>
        /// <param name="builder">The builder to act on.</param>
        /// <returns>
        /// This builder.
        /// </returns>
        public static IRegistrationBuilder IsOverride(this IRegistrationBuilder builder)
        {
            builder = builder ?? throw new ArgumentNullException(nameof(builder));

            return builder.AddMetadata(nameof(AppServiceMetadata.IsOverride), true);
        }
    }
}