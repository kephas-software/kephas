// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EncryptResponse.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Endpoints
{
    /// <summary>
    /// An encrypt response message.
    /// </summary>
    public class EncryptResponse
    {
        /// <summary>
        /// Gets or sets the encrypted value.
        /// </summary>
        /// <value>
        /// The encrypted value.
        /// </value>
        public string? Encrypted { get; set; }
    }
}