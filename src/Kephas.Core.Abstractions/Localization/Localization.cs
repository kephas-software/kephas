// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Localization.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the localization class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Localization
{
    using System.Collections.Generic;
    using System.Globalization;

    /// <summary>
    /// Provides basic means for localization, typically strings.
    /// </summary>
    public class Localization : ILocalization
    {
        private CultureInfo? culture;
        private Dictionary<string, object?>? data;

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

        /// <summary>
        /// Convenience method that provides a string Indexer
        /// to the Members collection AND the strongly typed
        /// members of the object by name.
        /// // dynamic
        /// exp["Address"] = "112 nowhere lane";
        /// // strong
        /// var name = exp["StronglyTypedProperty"] as string;.
        /// </summary>
        /// <value>
        /// The <see cref="object" /> identified by the key.
        /// </value>
        /// <param name="key">The key identifying the member name.</param>
        /// <returns>The requested member value.</returns>
        public object? this[string key]
        {
            get => this.data?[key];
            set => (this.data ??= new ())[key] = value;
        }
    }
}