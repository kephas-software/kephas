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
    using Kephas.Services;

    /// <summary>
    /// A hashing context.
    /// </summary>
    public class HashingContext : Context, IHashingContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HashingContext"/> class.
        /// </summary>
        /// <param name="serviceProvider">The injector.</param>
        /// <param name="isThreadSafe">Optional. True if this object is thread safe.</param>
        public HashingContext(IServiceProvider serviceProvider, bool isThreadSafe = false)
            : base(serviceProvider, isThreadSafe)
        {
        }

        /// <summary>
        /// Gets or sets the salt.
        /// </summary>
        /// <value>
        /// The salt.
        /// </value>
        public byte[]? Salt { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the service should use the default salt.
        /// </summary>
        /// <value>
        /// True if use the default salt, false if not.
        /// </value>
        public bool UseDefaultSalt { get; set; }
    }
}