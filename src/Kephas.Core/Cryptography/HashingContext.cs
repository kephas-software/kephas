// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HashingContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the hashing context class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Cryptography
{
    using Kephas.Composition;
    using Kephas.Services;

    /// <summary>
    /// A hashing context.
    /// </summary>
    public class HashingContext : Context, IHashingContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HashingContext"/> class.
        /// </summary>
        /// <param name="compositionContext">Optional. Context for the composition.</param>
        /// <param name="isThreadSafe">Optional. True if this object is thread safe.</param>
        public HashingContext(ICompositionContext compositionContext, bool isThreadSafe = false)
            : base(compositionContext, isThreadSafe)
        {
        }

        /// <summary>
        /// Gets or sets the salt.
        /// </summary>
        /// <value>
        /// The salt.
        /// </value>
        public byte[] Salt { get; set; }
    }
}