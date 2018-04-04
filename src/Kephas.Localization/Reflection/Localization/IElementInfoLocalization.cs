// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IElementInfoLocalization.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
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
        string Name { get; }

        /// <summary>
        /// Gets the localized description.
        /// </summary>
        /// <value>
        /// The localized description.
        /// </value>
        string Description { get; }
    }
}