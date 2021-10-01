// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DynamicDisplayInfo.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Reflection.Dynamic
{
    using Kephas.Dynamic;

    /// <summary>
    /// Display information for dynamic elements.
    /// </summary>
    public class DynamicDisplayInfo : Expando, IDisplayInfo
    {
        /// <summary>
        /// Gets or sets the localized name.
        /// </summary>
        /// <returns>
        /// The localized name.
        /// </returns>
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the localized short name.
        /// </summary>
        /// <returns>
        /// The localized short name.
        /// </returns>
        public string? ShortName { get; set; }

        /// <summary>
        /// Gets or sets the localized description.
        /// </summary>
        /// <returns>
        /// The localized description.
        /// </returns>
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the localized prompt.
        /// </summary>
        /// <returns>
        /// The localized prompt.
        /// </returns>
        public string? Prompt { get; set; }

        /// <summary>
        /// Gets the localized name.
        /// </summary>
        /// <returns>
        /// The localized name.
        /// </returns>
        public string? GetName() => this.Name ?? string.Empty;

        /// <summary>
        /// Gets the localized description.
        /// </summary>
        /// <returns>
        /// The localized description.
        /// </returns>
        public string? GetDescription() => this.Description ?? string.Empty;

        /// <summary>
        /// Gets the localized prompt.
        /// </summary>
        /// <returns>
        /// The localized prompt.
        /// </returns>
        public string? GetPrompt() => this.Prompt ?? string.Empty;

        /// <summary>
        /// Gets the localized short name.
        /// </summary>
        /// <returns>
        /// The localized short name.
        /// </returns>
        public string? GetShortName() => this.ShortName ?? string.Empty;
    }
}