// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OcrRegion.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.TextProcessing.OpticalCharacterRecognition
{
    using System;

    /// <summary>
    /// A region within a scanned document.
    /// </summary>
    public class OcrRegion
    {
        /// <summary>
        /// Gets or sets the bounding box.
        /// </summary>
        /// <value>
        /// The bounding box.
        /// </value>
        public string BoundingBox { get; set; }

        /// <summary>
        /// Gets or sets the lines.
        /// </summary>
        /// <value>
        /// The lines.
        /// </value>
        public OcrLine[] Lines { get; set; } = Array.Empty<OcrLine>();
    }
}