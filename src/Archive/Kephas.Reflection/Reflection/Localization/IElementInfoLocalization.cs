// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IElementInfoLocalization.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IElementInfoLocalization interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Reflection.Localization
{
    using Kephas.Localization;

    /// <summary>
    /// Interface for element information localization.
    /// </summary>
    public interface IElementInfoLocalization : ILocalization
    {
        /// <summary>
        /// Gets the localized name.
        /// </summary>
        /// <value>
        /// The localized name.
        /// </value>
        string? Name { get; }

        /// <summary>
        /// Gets the localized description.
        /// </summary>
        /// <value>
        /// The localized description.
        /// </value>
        string? Description { get; }

        /// <summary>
        /// Gets the localized short name.
        /// </summary>
        /// <remarks>
        /// The short name can be used for example in column headers.
        /// </remarks>
        /// <value>
        /// The localized short name.
        /// </value>
        string? ShortName { get; }

        /// <summary>
        /// Gets the localized value that will be used to set the watermark for prompts in the UI.
        /// </summary>
        /// <value>
        /// The localized value that will be used to set the watermark for prompts in the UI.
        /// </value>
        string? Prompt { get; }
    }
}