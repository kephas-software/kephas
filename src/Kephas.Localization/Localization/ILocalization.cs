﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ILocalization.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
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
    public interface ILocalization : IDynamic
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