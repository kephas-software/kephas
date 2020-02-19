// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EncryptMessage.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the encrypt message class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Endpoints
{
    using System.ComponentModel.DataAnnotations;

    using Kephas.ComponentModel.DataAnnotations;
    using Kephas.Messaging;

    /// <summary>
    /// An encrypt message.
    /// </summary>
    [TypeDisplay(Description = "Encrypts the provided value, using an optional key.")]
    public class EncryptMessage : IMessage
    {
        /// <summary>
        /// Gets or sets the encrypted value.
        /// </summary>
        /// <value>
        /// The encrypted value.
        /// </value>
        [Display(Description = "The value to be encrypted.")]
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>
        /// The key.
        /// </value>
        [Display(Description = "Optional. The key used in encryption. If not provided, a default key will be used.")]
        public string Key { get; set; }
    }

    /// <summary>
    /// An encrypt response message.
    /// </summary>
    public class EncryptResponseMessage
    {
        /// <summary>
        /// Gets or sets the encrypted value.
        /// </summary>
        /// <value>
        /// The encrypted value.
        /// </value>
        public string Encrypted { get; set; }
    }
}