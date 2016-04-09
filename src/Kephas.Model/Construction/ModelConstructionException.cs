// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelConstructionException.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
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
        public ModelConstructionException()
        {
        }

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