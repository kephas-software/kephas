// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IInjectionBuildContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IInjectionBuildContext interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Injection.Hosting
{
    using System.Collections.Generic;
    using System.Linq;

    using Kephas.Services;

    /// <summary>
    /// Contract interface for <see cref="IInjectorBuilder"/> contexts.
    /// </summary>
    public interface IInjectionBuildContext : IContext
    {
        /// <summary>
        /// Gets or sets the application service information providers.
        /// </summary>
        /// <value>
        /// The application service information providers.
        /// </value>
        IEnumerable<IAppServiceInfosProvider>? AppServiceInfosProviders { get; set; }
    }

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