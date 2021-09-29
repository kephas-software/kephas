// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InjectionBuildContextExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Testing.Injection
{
    using System.Collections.Generic;
    using System.Linq;
    using Kephas.Injection.Hosting;
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
            var providers = context.AppServiceInfosProviders?.ToList() ?? new List<IAppServiceInfosProvider>();
            providers.Add(provider);
            context.AppServiceInfosProviders = providers;
            return context;
        }
    }
}