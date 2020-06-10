// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PipesSettings.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Pipes.Configuration
{
    /// <summary>
    /// Settings for communication over pipes.
    /// </summary>
    public class PipesSettings
    {
        /// <summary>
        /// Gets or sets the server hosting the root.
        /// </summary>
        public string ServerName { get; set; } = ".";

        /// <summary>
        /// Gets or sets the namespace.
        /// </summary>
        public string? Namespace { get; set; }
    }
}