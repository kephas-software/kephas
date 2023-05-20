// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMessageProcessingContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Contract for contexts when processing messages.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging
{
    using Kephas.Services;

    /// <summary>
    /// Contract for contexts when processing messages.
    /// </summary>
    public interface IMessagingContext : IContext
    {
        /// <summary>
        /// Gets the message to process.
        /// </summary>
        IMessageBase Message { get; }
    }
}