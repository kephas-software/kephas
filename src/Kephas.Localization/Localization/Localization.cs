// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Localization.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the localization class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Localization
{
    using System.Globalization;

    using Kephas.Dynamic;

    /// <summary>
    /// Provides basic means for localization, typically strings.
    /// </summary>
    public class Localization : Expando, ILocalization
    {
        /// <summary>
        /// The culture.
        /// </summary>
        private CultureInfo culture;

        /// <summary>
        /// Gets or sets the culture.
        /// </summary>
        /// <value>
        /// The culture.
        /// </value>
        public CultureInfo Culture
        {
            get => this.culture ?? CultureInfo.CurrentCulture;
            set => this.culture = value;
        }
    }
}