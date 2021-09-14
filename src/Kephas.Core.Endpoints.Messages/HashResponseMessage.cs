// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HashResponseMessage.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Endpoints
{
    using Kephas.Messaging;

    /// <summary>
    /// A hash response message.
    /// </summary>
    public class HashResponseMessage : IMessage
    {
        /// <summary>
        /// Gets or sets the hash.
        /// </summary>
        /// <value>
        /// The hash.
        /// </value>
        public string? Hash { get; set; }
    }
}