// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OcrPage.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.TextProcessing.OpticalCharacterRecognition
{
    using System;

    using Kephas.Dynamic;

    /// <summary>
    /// A page within a scanned document.
    /// </summary>
    public class OcrPage : Expando
    {
        /// <summary>
        /// Gets or sets the page number.
        /// </summary>
        /// <value>
        /// The page number.
        /// </value>
        public virtual int? Page { get; set; }

        /// <summary>
        /// Gets or sets the bounding box.
        /// </summary>
        /// <value>
        /// The bounding box.
        /// </value>
        public virtual int[]? BoundingBox { get; set; }

        /// <summary>
        /// Gets or sets the lines.
        /// </summary>
        /// <value>
        /// The lines.
        /// </value>
        public virtual OcrLine[] Lines { get; set; } = Array.Empty<OcrLine>();
    }
}