// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Tokenizer.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the tokenizer class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.TextProcessing
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;

    using Kephas.Configuration;
    using Kephas.Dynamic;
    using Kephas.Services;

    /// <summary>
    /// A tokenizer service.
    /// </summary>
    public class Tokenizer : ITokenizer
    {
        private readonly IContextFactory contextFactory;
        private readonly IConfiguration<TokenizerSettings>? tokenizerConfig;

        /// <summary>
        /// Initializes a new instance of the <see cref="Tokenizer"/> class.
        /// </summary>
        /// <param name="contextFactory">The context factory.</param>
        /// <param name="tokenizerConfig">Optional. The tokenizer configuration.</param>
        public Tokenizer(IContextFactory contextFactory, IConfiguration<TokenizerSettings>? tokenizerConfig = null)
        {
            this.contextFactory = contextFactory;
            this.tokenizerConfig = tokenizerConfig;
        }

        /// <summary>
        /// Splits the provided text into tokens and returns them.
        /// </summary>
        /// <param name="text">The text to be tokenized.</param>
        /// <param name="optionsConfig">Optional. The options configuration.</param>
        /// <returns>
        /// An enumeration of tokens.
        /// </returns>
        public IEnumerable<string> Tokenize(string text, Action<ITokenizerContext>? optionsConfig = null)
        {
            using (var context = this.CreateTokenizerContext(optionsConfig))
            {
                var settings = this.tokenizerConfig?.GetSettings() ?? new TokenizerSettings();

                var sb = new StringBuilder(text);
                foreach (var separator in settings.WordSeparators ?? new string[0])
                {
                    sb.Replace(separator, " ");
                }

                IEnumerable<string> tokens = sb.ToString()
                    .Split(new[] { ' ', '\r', '\n', '\t' }, StringSplitOptions.RemoveEmptyEntries)
                    .Where(t => !settings.Languages.Any(l =>
                        {
                            var culture = CultureInfo.GetCultureInfo(l.Name);
                            return l.NoiseWords.Any(w => string.Compare(w, t, true, culture) == 0);
                        }));

                if (context.Transformation != null)
                {
                    tokens = tokens.Select(t => context.Transformation(t));
                }

                return tokens;
            }
        }

        /// <summary>
        /// Creates tokenizer context.
        /// </summary>
        /// <param name="optionsConfig">Optional. The options configuration.</param>
        /// <returns>
        /// The new tokenizer context.
        /// </returns>
        protected virtual ITokenizerContext CreateTokenizerContext(Action<ITokenizerContext>? optionsConfig = null)
        {
            return this.contextFactory.CreateContext<TokenizerContext>().Merge(optionsConfig);
        }
    }
}
