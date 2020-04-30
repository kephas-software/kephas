// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OcrResult.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.TextProcessing.OpticalCharacterRecognition
{
    /// <summary>
    /// An OCR result.
    /// </summary>
    public class OcrResult
    {
        /// <summary>
        /// Gets or sets the text angle.
        /// </summary>
        /// <value>
        /// The text angle.
        /// </value>
        public double? TextAngle { get; set; }

        /// <summary>
        /// Gets or sets the orientation.
        /// </summary>
        /// <value>
        /// The orientation.
        /// </value>
        public string Orientation { get; set; }

        /// <summary>
        /// Gets or sets the language.
        /// </summary>
        /// <value>
        /// The language.
        /// </value>
        public string Language { get; set; }

        /// <summary>
        /// Gets or sets the regions.
        /// </summary>
        /// <value>
        /// The regions.
        /// </value>
        public OcrRegion[] Regions { get; set; }
    }
}