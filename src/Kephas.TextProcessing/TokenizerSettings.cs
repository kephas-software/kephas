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
    /// <summary>
    /// The tokenizer settings.
    /// </summary>
    public class TokenizerSettings
    {
        /// <summary>
        /// Gets or sets the word separators.
        /// </summary>
        /// <value>
        /// The word separators.
        /// </value>
        public string[] WordSeparators { get; set; } = new[]
            {
                "\"",
                "'",
                "[",
                "]",
                "?",
                ",",
                ";",
                ":",
                "/",
                "\\",
                "+",
                "=",
                "(",
                ")",
                "&",
                "*",
                "$",
                "%",
                "!",
                "|",
                "{",
                "}",
                "~",
                "`",
                "#",
                "^",
                ">",
                "<",
                " -",
                "- ",
            };

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
