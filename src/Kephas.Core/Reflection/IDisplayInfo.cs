// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDisplayInfo.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IDisplayInfo interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Reflection
{
    /// <summary>
    /// Interface for display information.
    /// </summary>
    public interface IDisplayInfo
    {
        /// <summary>
        /// Gets the localized name.
        /// </summary>
        /// <returns>
        /// The localized name.
        /// </returns>
        string GetName();

        /// <summary>
        /// Gets the localized description.
        /// </summary>
        /// <returns>
        /// The localized description.
        /// </returns>
        string GetDescription();

        /// <summary>
        /// Gets the localized prompt.
        /// </summary>
        /// <returns>
        /// The localized prompt.
        /// </returns>
        string GetPrompt();

        /// <summary>
        /// Gets the localized short name.
        /// </summary>
        /// <returns>
        /// The localized short name.
        /// </returns>
        string GetShortName();
    }
}