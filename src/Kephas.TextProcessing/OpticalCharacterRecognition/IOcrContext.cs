// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IOcrContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IOcrContext interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.TextProcessing.OpticalCharacterRecognition
{
    using Kephas.Services;

    /// <summary>
    /// An OCR context.
    /// </summary>
    public interface IOcrContext : IContext
    {
        /// <summary>
        /// Gets or sets the language.
        /// </summary>
        /// <value>
        /// The language.
        /// </value>
        public string Language { get; set; }

        /// <summary>
        /// Gets or sets the type of the media.
        /// </summary>
        /// <value>
        /// The type of the media.
        /// </value>
        public string MediaType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the orientation should be auto detected.
        /// </summary>
        /// <value>
        /// True if the orientation should be auto detected, false otherwise.
        /// </value>
        public bool? DetectOrientation { get; set; }
    }
}
