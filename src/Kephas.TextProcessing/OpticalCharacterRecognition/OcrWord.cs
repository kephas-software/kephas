// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OcrWord.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.TextProcessing.OpticalCharacterRecognition
{
    /// <summary>
    /// A word within a scanned document's line.
    /// </summary>
    public class OcrWord
    {
        /// <summary>
        /// Gets or sets the bounding box.
        /// </summary>
        /// <value>
        /// The bounding box.
        /// </value>
        public virtual int[]? BoundingBox { get; set; }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>
        /// The text.
        /// </value>
        public virtual string Text { get; set; }

        /// <summary>
        /// Gets or sets the confidence of the recognized text.
        /// </summary>
        /// <value>
        /// The text.
        /// </value>
        public virtual string? Confidence { get; set; }
    }
}