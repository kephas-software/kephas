// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OcrLine.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.TextProcessing.OpticalCharacterRecognition
{
    /// <summary>
    /// A line within a scanned document's region.
    /// </summary>
    public class OcrLine
    {
        /// <summary>
        /// Gets or sets the bounding box.
        /// </summary>
        /// <value>
        /// The bounding box.
        /// </value>
        public string BoundingBox { get; set; }

        /// <summary>
        /// Gets or sets the words.
        /// </summary>
        /// <value>
        /// The words.
        /// </value>
        public OcrWord[] Words { get; set; }
    }
}