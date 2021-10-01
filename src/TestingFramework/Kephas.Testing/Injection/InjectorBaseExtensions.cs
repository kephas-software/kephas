// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InjectorBaseExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Testing.Injection
{
    using System;
    using System.Collections.Generic;
    using Kephas.Injection.Builder;
    using Kephas.Services;

    /// <summary>
    /// Extension methods for <see cref="InjectorBuilderBase{TBuilder}"/>.
    /// </summary>
    public static class InjectorBaseExtensions
    {
        /// <summary>
        /// Adds the composition parts.
        /// </summary>
        /// <typeparam name="TBuilder">The builder type.</typeparam>
        /// <param name="builder">The builder.</param>
        /// <param name="parts">The parts.</param>
        /// <returns>
        /// This builder.
        /// </returns>
        /// <remarks>
        /// Can be used multiple times, the provided parts are added to the existing ones.
        /// </remarks>
        public static TBuilder WithParts<TBuilder>(this TBuilder builder, IEnumerable<Type> parts)
            where TBuilder : InjectorBuilderBase<TBuilder>
        {
            parts = parts ?? throw new ArgumentNullException(nameof(parts));

            return builder.WithAppServiceInfosProvider(new PartsAppServiceInfosProvider(parts));
        }

        /// <summary>
        /// Adds the <see cref="IAppServiceInfosProvider"/>.
        /// </summary>
        /// <remarks>
        /// Can be used multiple times, the providers are added to the existing ones.
        /// </remarks>
        /// <typeparam name="TBuilder">The builder type.</typeparam>
        /// <param name="builder">The builder.</param>
        /// <param name="appServiceInfosProvider">The <see cref="IAppServiceInfosProvider"/>.</param>
        /// <returns>
        /// This builder.
        /// </returns>
        public static TBuilder WithAppServiceInfosProvider<TBuilder>(this TBuilder builder, IAppServiceInfosProvider appServiceInfosProvider)
            where TBuilder : InjectorBuilderBase<TBuilder>
        {
            appServiceInfosProvider = appServiceInfosProvider ?? throw new ArgumentNullException(nameof(appServiceInfosProvider));

            builder.BuildContext.AppServiceInfosProviders.Add(appServiceInfosProvider);

            return builder;
        }
    }
}