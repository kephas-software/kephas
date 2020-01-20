// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IOcrProcessor.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IOcrProcessor interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.TextProcessing.OpticalCharacterRecognition
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Services;

    /// <summary>
    /// Interface for OCR processor.
    /// </summary>
    [SingletonAppServiceContract]
    public interface IOcrProcessor
    {
        /// <summary>
        /// Process the image stream asynchronously.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="optionsConfig">Optional. The options configuration.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the OCR result.
        /// </returns>
        Task<IOcrResult> ProcessAsync(Stream image, Action<IOcrContext> optionsConfig = null, CancellationToken cancellationToken = default);
    }
}
