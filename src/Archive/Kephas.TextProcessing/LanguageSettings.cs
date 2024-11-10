// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LanguageSettings.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the language settings class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.TextProcessing
{
    /// <summary>
    /// A language settings.
    /// </summary>
    public class LanguageSettings
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the noise words.
        /// </summary>
        /// <value>
        /// The noise words.
        /// </value>
        public string[] NoiseWords { get; set; }
    }
}
