// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMemberInfoLocalization.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IMemberInfoLocalization interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Reflection.Localization
{
    /// <summary>
    /// Interface for property information localization.
    /// </summary>
    public interface IMemberInfoLocalization : IElementInfoLocalization
    {
        /// <summary>
        /// Gets the localized short name.
        /// </summary>
        /// <remarks>
        /// The short name can be used for example in column headers.
        /// </remarks>
        /// <value>
        /// The localized short name.
        /// </value>
        string ShortName { get; }

        /// <summary>
        /// Gets the localized value that will be used to set the watermark for prompts in the UI.
        /// </summary>
        /// <value>
        /// The localized value that will be used to set the watermark for prompts in the UI.
        /// </value>
        string Prompt { get; }
    }
}