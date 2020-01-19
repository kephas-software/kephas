// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISearchContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the ISearchContext interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Plugins
{
    using Kephas.Services;

    /// <summary>
    /// Interface for search plugin context.
    /// </summary>
    public interface ISearchContext : IContext
    {
        /// <summary>
        /// Gets or sets the search term.
        /// </summary>
        /// <value>
        /// The search term.
        /// </value>
        string SearchTerm { get; set; }

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
    }
}
