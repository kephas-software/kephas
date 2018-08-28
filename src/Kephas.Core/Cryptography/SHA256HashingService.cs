// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SHA256HashingService.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
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
    /// A null hashing service.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class SHA256HashingService : HashingServiceBase
    {
        /// <summary>
        /// Creates the hash algorithm.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>
        /// The new hash algorithm.
        /// </returns>
        protected override HashAlgorithm CreateHashAlgorithm(IContext context) => new SHA256Managed();
    }
}