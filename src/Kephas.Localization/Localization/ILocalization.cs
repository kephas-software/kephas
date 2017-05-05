// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ILocalization.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the ILocalization interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Localization
{
    using System.Globalization;

    using Kephas.Dynamic;

    /// <summary>
    /// Interface for localization.
    /// </summary>
    public interface ILocalization : IExpando
    {
        /// <summary>
        /// Gets the culture.
        /// </summary>
        /// <value>
        /// The culture.
        /// </value>
        CultureInfo Culture { get; }
    }
}