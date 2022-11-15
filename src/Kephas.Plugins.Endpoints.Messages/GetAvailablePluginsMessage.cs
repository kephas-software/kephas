// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetAvailablePluginsMessage.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the get available plugins message class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Plugins.Endpoints
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using Kephas.Application;
    using Kephas.ComponentModel.DataAnnotations;
    using Kephas.Messaging;
    using Kephas.Messaging.Messages;

    /// <summary>
    /// A get available plugins message.
    /// </summary>
    [DisplayInfo(Description = "Gets the available plugins. If a plugin ID is provided, the available versions are retrieved.")]
    public class GetAvailablePluginsMessage : IMessage
    {
        /// <summary>
        /// Gets or sets a value indicating whether prerelease versions should be included or not.
        /// </summary>
        /// <value>
        /// True to include prerelease versions, false otherwise.
        /// </value>
        [Display(ShortName = "pre", Description = "Optional. Value indicating whether prerelease versions should be included or not.")]
        public bool IncludePrerelease { get; set; } = false;

        /// <summary>
        /// Gets or sets the plugin ID.
        /// </summary>
        /// <value>
        /// The plugin ID.
        /// </value>
        [Display(Description = "Optional. Value indicating the plugin ID. If the ID is provided, the available versions are retrieved.")]
        public string? Id { get; set; }

        /// <summary>
        /// Gets or sets the number of packages to skip.
        /// </summary>
        /// <value>
        /// The number of packages to skip.
        /// </value>
        [Display(Description = "Optional. Value indicating the number of packages to skip (default: 0).")]
        public int Skip { get; set; } = 0;

        /// <summary>
        /// Gets or sets the number of packages to take.
        /// </summary>
        /// <value>
        /// The number of packages to take.
        /// </value>
        [Display(Description = "Optional. Value indicating the number of packages to take (default: 20).")]
        public int Take { get; set; } = 20;

        /// <summary>
        /// Gets or sets the search term.
        /// </summary>
        /// <value>
        /// The search term.
        /// </value>
        [Display(ShortName = "term", Description = "Optional. The search term.")]
        public string? SearchTerm { get; set; }
    }

    /// <summary>
    /// A get available plugins response message.
    /// </summary>
    public class GetAvailablePluginsResponseMessage : ResponseMessage
    {
        /// <summary>
        /// Gets or sets the plugins.
        /// </summary>
        /// <value>
        /// The plugins.
        /// </value>
        public IDictionary<string, string>? Plugins { get; set; }
    }
}
