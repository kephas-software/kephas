// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OcrContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the OCR context class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Injection;

namespace Kephas.TextProcessing.OpticalCharacterRecognition
{
    using Kephas.Services;

    /// <summary>
    /// An OCR context.
    /// </summary>
    public class OcrContext : Context, IOcrContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OcrContext"/> class.
        /// </summary>
        /// <param name="injector">Context for the composition.</param>
        /// <param name="isThreadSafe">Optional. True if is thread safe, false if not.</param>
        public OcrContext(IInjector injector, bool isThreadSafe = false)
            : base(injector, isThreadSafe)
        {
        }

        /// <summary>
        /// Gets or sets the language.
        /// </summary>
        /// <value>
        /// The language.
        /// </value>
        public string Language { get; set; } = "unk";

        /// <summary>
        /// Gets or sets a value indicating whether the orientation should be auto detected.
        /// </summary>
        /// <value>
        /// True if the orientation should be auto detected, false otherwise.
        /// </value>
        public bool? DetectOrientation { get; set; } = true;

        /// <summary>
        /// Gets or sets the type of the media.
        /// </summary>
        /// <value>
        /// The type of the media.
        /// </value>
        public string MediaType { get; set; } = "image/tiff";
    }
}
