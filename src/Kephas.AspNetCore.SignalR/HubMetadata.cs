// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HubMetadata.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore.SignalR
{
    using System.Collections.Generic;
    using Kephas.Collections;
    using Kephas.Services;

    /// <summary>
    /// Metadata class for hubs.
    /// </summary>
    public class HubMetadata : AppServiceMetadata
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HubMetadata"/> class.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        public HubMetadata(IDictionary<string, object?>? metadata)
            : base(metadata)
        {
            if (metadata == null)
            {
                return;
            }

            this.Pattern = (string?)metadata.TryGetValue(nameof(this.Pattern)) ?? string.Empty;
        }

        /// <summary>
        /// Gets the hub pattern.
        /// </summary>
        public string Pattern { get; } = string.Empty;
    }
}