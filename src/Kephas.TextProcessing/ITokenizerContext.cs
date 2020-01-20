// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITokenizerContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the ITokenizerContext interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.TextProcessing
{
    using System;

    using Kephas.Services;

    /// <summary>
    /// Interface for tokenizer context.
    /// </summary>
    public interface ITokenizerContext : IContext
    {
        /// <summary>
        /// Gets or sets the transformation.
        /// </summary>
        /// <value>
        /// A function delegate that yields a string.
        /// </value>
        public Func<string, string> Transformation { get; set; }
    }
}
