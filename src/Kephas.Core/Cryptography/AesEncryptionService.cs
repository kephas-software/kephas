// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AesEncryptionService.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the aes encryption service class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Cryptography
{
    using System.Security.Cryptography;

    using Kephas.Services;

    /// <summary>
    /// The AES encryption service.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class AesEncryptionService : SymmetricEncryptionServiceBase<AesManaged>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AesEncryptionService"/> class.
        /// </summary>
        /// <param name="contextFactory">The context factory.</param>
        public AesEncryptionService(IContextFactory contextFactory)
            : base(contextFactory, ctx => new AesManaged())
        {
        }
    }
}