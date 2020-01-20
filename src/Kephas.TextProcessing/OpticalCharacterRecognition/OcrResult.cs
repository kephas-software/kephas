// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OcrResult.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the OCR result class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.TextProcessing.OpticalCharacterRecognition
{
    using System.Text;

    using Kephas.Operations;

    /// <summary>
    /// Encapsulates the result of an OCR operation.
    /// </summary>
    public class OcrResult : OperationResult, IOcrResult
    {
        private StringBuilder sb = new StringBuilder();

        /// <summary>
        /// Initializes this object from the given from string.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <returns>
        /// An OcrResult.
        /// </returns>
        public static OcrResult FromString(string str)
        {
            return new OcrResult().AppendLine(str);
        }

        /// <summary>
        /// Appends a line.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <returns>
        /// This <see cref="OcrResult"/>.
        /// </returns>
        public OcrResult AppendLine(string str)
        {
            this.sb.AppendLine(str);

            return this;
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return this.sb.ToString();
        }
    }
}
