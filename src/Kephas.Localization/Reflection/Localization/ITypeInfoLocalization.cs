// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITypeInfoLocalization.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
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