// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelConstructionException.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the model construction exception class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Construction
{
    using System;

    /// <summary>
    /// Exception for signalling errors during model construction.
    /// </summary>
    public class ModelConstructionException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModelConstructionException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public ModelConstructionException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ModelConstructionException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner exception.</param>
        public ModelConstructionException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}