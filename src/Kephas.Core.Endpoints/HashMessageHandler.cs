// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HashMessageHandler.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the hash message handler class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Endpoints
{
    using System;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Cryptography;
    using Kephas.Messaging;

    /// <summary>
    /// A hash message handler.
    /// </summary>
    public class HashMessageHandler : MessageHandlerBase<HashMessage, HashResponseMessage>
    {
        /// <summary>
        /// The hashing service.
        /// </summary>
        private readonly IHashingService hashingService;

        /// <summary>
        /// Initializes a new instance of the <see cref="HashMessageHandler"/> class.
        /// </summary>
        /// <param name="hashingService">The hashing service.</param>
        public HashMessageHandler(IHashingService hashingService)
        {
            this.hashingService = hashingService;
        }

        /// <summary>
        /// Processes the provided message asynchronously and returns a response promise.
        /// </summary>
        /// <param name="message">The message to be handled.</param>
        /// <param name="context">The processing context.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The response promise.</returns>
        public override async Task<HashResponseMessage> ProcessAsync(HashMessage message, IMessagingContext context, CancellationToken token)
        {
            if (string.IsNullOrEmpty(message.Value)) throw new ArgumentException("Value must not be null or empty.", nameof(message.Value));

            await Task.Yield();

            var salt = string.IsNullOrEmpty(message.Salt)
                        ? null
                        : Encoding.UTF8.GetBytes(message.Salt);
            var hash = this.hashingService.Hash(message.Value, ctx => (salt == null ? ctx : ctx.Salt(salt)).UseDefaultSalt(message.UseDefaultSalt));

            var response = new HashResponseMessage
            {
                Hash = Convert.ToBase64String(hash),
            };
            return response;
        }
    }
}