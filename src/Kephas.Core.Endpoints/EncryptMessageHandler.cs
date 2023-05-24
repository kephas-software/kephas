// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EncryptMessageHandler.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the encrypt message handler class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Endpoints
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Cryptography;
    using Kephas.Messaging;

    /// <summary>
    /// An encrypt message handler.
    /// </summary>
    public class EncryptMessageHandler : IMessageHandler<EncryptMessage, EncryptResponse>
    {
        private readonly IEncryptionService encryptionService;

        /// <summary>
        /// Initializes a new instance of the <see cref="EncryptMessageHandler"/> class.
        /// </summary>
        /// <param name="encryptionService">The encryption service.</param>
        public EncryptMessageHandler(IEncryptionService encryptionService)
        {
            this.encryptionService = encryptionService;
        }

        /// <summary>
        /// Processes the provided message asynchronously and returns a response promise.
        /// </summary>
        /// <param name="message">The message to be handled.</param>
        /// <param name="context">The processing context.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The response promise.</returns>
        public Task<EncryptResponse> ProcessAsync(EncryptMessage message, IMessagingContext context, CancellationToken token)
        {
            var key = string.IsNullOrWhiteSpace(message.Key) ? null : Convert.FromBase64String(message.Key);
            var encrypted = encryptionService.Encrypt(message.Value, ctx => ctx.Key(key));
            return Task.FromResult(new EncryptResponse { Encrypted = encrypted });
        }
    }
}