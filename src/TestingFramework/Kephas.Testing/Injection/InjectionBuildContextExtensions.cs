﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InjectionBuildContextExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Testing.Injection
{
    using Kephas.Injection.Builder;
    using Kephas.Services;

    /// <summary>
    /// Extension methods for <see cref="IInjectionBuildContext"/>.
    /// </summary>
    public static class InjectionBuildContextExtensions
    {
        /// <summary>
        /// Adds a new <see cref="IAppServiceInfosProvider"/> to the <see cref="IInjectionBuildContext.AppServiceInfosProviders"/>.
        /// </summary>
        /// <typeparam name="TContext">The context type.</typeparam>
        /// <param name="context">The context.</param>
        /// <param name="provider">The provider.</param>
        /// <returns>This <see cref="IInjectionBuildContext"/> instance.</returns>
        public static TContext WithAppServiceInfosProvider<TContext>(this TContext context, IAppServiceInfosProvider provider)
            where TContext : IInjectionBuildContext
        {
            context.AppServiceInfosProviders.Add(provider);
            return context;
        }
    }
}