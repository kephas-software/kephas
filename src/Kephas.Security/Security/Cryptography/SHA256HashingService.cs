// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SHA256HashingService.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the null hashing service class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Cryptography
{
    using System.Security.Cryptography;

    using Kephas.Services;

    /// <summary>
    /// A SHA-256 hashing service.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class Sha256HashingService : HashingServiceBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Sha256HashingService"/> class.
        /// </summary>
        /// <param name="injectableFactory">The injectable factory.</param>
        public Sha256HashingService(IInjectableFactory injectableFactory)
            : base(injectableFactory)
        {
        }

        /// <summary>
        /// Creates the hash algorithm.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>
        /// The new hash algorithm.
        /// </returns>
        protected override HashAlgorithm CreateHashAlgorithm(IHashingContext? context = null) => SHA256.Create();
    }
}