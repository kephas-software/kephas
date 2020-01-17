// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISearchPluginContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the ISearchPluginContext interface.
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
    }
}
