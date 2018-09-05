// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITypeInfoLocalization.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the ITypeInfoLocalization interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Reflection.Localization
{
    using System.Collections.Generic;

    /// <summary>
    /// Interface for type information localization.
    /// </summary>
    public interface ITypeInfoLocalization : IElementInfoLocalization
    {
        /// <summary>
        /// Gets a dictionary of members' localizations.
        /// </summary>
        /// <value>
        /// The members' localizations.
        /// </value>
        IDictionary<string, IMemberInfoLocalization> Members { get; }
    }
}