// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SearchContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the search context class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#nullable enable

namespace Kephas.Plugins
{
    using Kephas.Application;
    using Kephas.Composition;
    using Kephas.Services;

    /// <summary>
    /// A search context.
    /// </summary>
    public class SearchContext : Context, ISearchContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SearchContext"/> class.
        /// </summary>
        /// <param name="compositionContext">Context for the composition.</param>
        public SearchContext(ICompositionContext compositionContext)
            : base(compositionContext)
        {
        }

        /// <summary>
        /// Gets or sets the search term.
        /// </summary>
        /// <value>
        /// The search term.
        /// </value>
        public string? SearchTerm { get; set; }

        /// <inheritdoc/>
        public AppIdentity? PluginIdentity { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the prerelease versions should be included.
        /// </summary>
        /// <value>
        /// True to include prerelease versions, false to ignore them.
        /// </value>
        public bool IncludePrerelease { get; set; } = false;

        /// <summary>
        /// Gets or sets the number of packages to skip.
        /// </summary>
        /// <value>
        /// The number of packages to skip.
        /// </value>
        public int Skip { get; set; } = 0;

        /// <summary>
        /// Gets or sets the number of packages to take.
        /// </summary>
        /// <value>
        /// The number of packages to take.
        /// </value>
        public int Take { get; set; } = 20;
    }
}
