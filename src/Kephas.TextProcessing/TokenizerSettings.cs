// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TokenizerSettings.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the tokenizer settings class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.TextProcessing
{
    using Kephas.Configuration;

    /// <summary>
    /// The tokenizer settings.
    /// </summary>
    public class TokenizerSettings : ISettings
    {
        /// <summary>
        /// Gets or sets the word separators.
        /// </summary>
        /// <value>
        /// The word separators.
        /// </value>
        public string WordSeparators { get; set; } = " \t\r\n\"'[]?,;:/\\+=()&*$%!|{}~`#^><";

        /// <summary>
        /// Gets or sets the word block separators.
        /// </summary>
        /// <remarks>They are blocks of characters which count only as a block when considering the word separation.</remarks>
        /// <value>
        /// The word block separators.
        /// </value>
        public string[] WordBlockSeparators { get; set; } = { " -", "- " };

        /// <summary>
        /// Gets or sets the minimum length of a word.
        /// </summary>
        public int WordMinLength { get; set; } = 3;

        /// <summary>
        /// Gets or sets the list of language settings.
        /// </summary>
        /// <value>
        /// The languages.
        /// </value>
        public LanguageSettings[] Languages { get; set; } = new[]
        {
            new LanguageSettings
            {
                Name = "en",
                NoiseWords = new[]
                {
                    "the",
                    "re",
                    "and",
                    "or",
                    "not",
                    "no",
                    "yes",
                    "if",
                    "when",
                    "where",
                    "there",
                },
            },
            new LanguageSettings
            {
                Name = "de",
                NoiseWords = new[]
                {
                    "der",
                    "die",
                    "das",
                    "fw",
                    "und",
                    "oder",
                    "nicht",
                    "nein",
                    "ja",
                    "wenn",
                    "wann",
                    "wo",
                    "da",
                },
            },
        };
    }
}
