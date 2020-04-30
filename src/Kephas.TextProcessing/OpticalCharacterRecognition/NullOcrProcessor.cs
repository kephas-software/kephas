// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NullOcrProcessor.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the null OCR processor class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.TextProcessing.OpticalCharacterRecognition
{
    using System;
    using System.IO;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Operations;
    using Kephas.Services;

    /// <summary>
    /// A null OCR processor.
    /// </summary>
    [OverridePriority(Priority.Lowest)]
    public class NullOcrProcessor : IOcrProcessor
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
        public Task<IOperationResult<OcrResult>> ProcessAsync(Stream image, Action<IOcrContext>? optionsConfig = null, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IOperationResult<OcrResult>>(new OcrResult().ToOperationResult());
        }
    }
}
