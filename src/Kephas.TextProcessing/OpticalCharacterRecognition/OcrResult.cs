// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OcrResult.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.TextProcessing.OpticalCharacterRecognition
{
    using System;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// An OCR result.
    /// </summary>
    public class OcrResult
    {
        /// <summary>
        /// Gets or sets the read operation status.
        /// </summary>
        /// <value>
        /// The read operation status.
        /// </value>
        public virtual string? Status { get; set; }

        /// <summary>
        /// Gets or sets the language.
        /// </summary>
        /// <value>
        /// The language.
        /// </value>
        public virtual string? Language { get; set; }

        /// <summary>
        /// Gets or sets the regions.
        /// </summary>
        /// <value>
        /// The regions.
        /// </value>
        public virtual OcrPage[] Pages { get; set; } = Array.Empty<OcrPage>();

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var line in this.Pages.SelectMany(r => r.Lines))
            {
                sb.AppendLine(string.Join(" ", line.Words.Select(w => w.Text)));
            }

            return sb.ToString();
        }
    }
}