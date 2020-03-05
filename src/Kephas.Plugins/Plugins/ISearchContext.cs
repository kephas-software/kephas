// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISearchContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the ISearchContext interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#nullable enable

namespace Kephas.Plugins
{
    using Kephas.Application;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Services;

    /// <summary>
    /// Interface for search plugin context.
    /// </summary>
    public interface ISearchContext : IContext
    {
        /// <summary>
        /// Gets or sets the plugin identity.
        /// </summary>
        /// <value>
        /// The plugin identity.
        /// </value>
        AppIdentity? PluginIdentity { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the prerelease versions should be included.
        /// </summary>
        /// <value>
        /// True to include prerelease versions, false to ignore them.
        /// </value>
        bool IncludePrerelease { get; set; }

        /// <summary>
        /// Gets or sets the number of packages to skip.
        /// </summary>
        /// <value>
        /// The number of packages to skip.
        /// </value>
        int Skip { get; set; }

        /// <summary>
        /// Gets or sets the number of packages to take.
        /// </summary>
        /// <value>
        /// The number of packages to take.
        /// </value>
        int Take { get; set; }

        /// <summary>
        /// Gets or sets the search term.
        /// </summary>
        /// <value>
        /// The search term.
        /// </value>
        string? SearchTerm { get; set; }
    }

    /// <summary>
    /// A search context extensions.
    /// </summary>
    public static class SearchContextExtensions
    {
        /// <summary>
        /// Sets the search term.
        /// </summary>
        /// <typeparam name="TContext">Actual type of the search operation context.</typeparam>
        /// <param name="searchContext">The search context.</param>
        /// <param name="pluginIdentity">The plugin identity.</param>
        /// <returns>
        /// This <paramref name="searchContext"/>.
        /// </returns>
        public static TContext PluginIdentity<TContext>(this TContext searchContext, AppIdentity? pluginIdentity)
            where TContext : class, ISearchContext
        {
            Requires.NotNull(searchContext, nameof(searchContext));

            searchContext.PluginIdentity = pluginIdentity;

            return searchContext;
        }

        /// <summary>
        /// Sets the search term.
        /// </summary>
        /// <typeparam name="TContext">Actual type of the search operation context.</typeparam>
        /// <param name="searchContext">The search context.</param>
        /// <param name="searchTerm">The search term.</param>
        /// <returns>
        /// This <paramref name="searchContext"/>.
        /// </returns>
        public static TContext SearchTerm<TContext>(this TContext searchContext, string searchTerm)
            where TContext : class, ISearchContext
        {
            Requires.NotNull(searchContext, nameof(searchContext));

            searchContext.SearchTerm = searchTerm;

            return searchContext;
        }

        /// <summary>
        /// Sets a value indicating whether to include prerelease versions in the results.
        /// </summary>
        /// <typeparam name="TContext">Actual type of the search operation context.</typeparam>
        /// <param name="searchContext">The search context.</param>
        /// <param name="includePrerelease">True to include prerelease versions, false otherwise.</param>
        /// <returns>
        /// This <paramref name="searchContext"/>.
        /// </returns>
        public static TContext IncludePrerelease<TContext>(this TContext searchContext, bool includePrerelease)
            where TContext : class, ISearchContext
        {
            Requires.NotNull(searchContext, nameof(searchContext));

            searchContext.IncludePrerelease = includePrerelease;

            return searchContext;
        }

        /// <summary>
        /// Sets the number of results to skip.
        /// </summary>
        /// <typeparam name="TContext">Actual type of the search operation context.</typeparam>
        /// <param name="searchContext">The search context.</param>
        /// <param name="skip">The number of results to skip.</param>
        /// <returns>
        /// This <paramref name="searchContext"/>.
        /// </returns>
        public static TContext Skip<TContext>(this TContext searchContext, int skip)
            where TContext : class, ISearchContext
        {
            Requires.NotNull(searchContext, nameof(searchContext));

            searchContext.Skip = skip;

            return searchContext;
        }

        /// <summary>
        /// Sets the number of results to take.
        /// </summary>
        /// <typeparam name="TContext">Actual type of the search operation context.</typeparam>
        /// <param name="searchContext">The search context.</param>
        /// <param name="take">The number of results to take.</param>
        /// <returns>
        /// This <paramref name="searchContext"/>.
        /// </returns>
        public static TContext Take<TContext>(this TContext searchContext, int take)
            where TContext : class, ISearchContext
        {
            Requires.NotNull(searchContext, nameof(searchContext));

            searchContext.Take = take;

            return searchContext;
        }
    }
}
