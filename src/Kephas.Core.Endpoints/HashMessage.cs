// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HashMessage.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the hash message class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Endpoints
{
    using System.ComponentModel.DataAnnotations;

    using Kephas.ComponentModel.DataAnnotations;
    using Kephas.Messaging;

    /// <summary>
    /// A hash message.
    /// </summary>
    [DisplayInfo(Description = "Hashes the provided value, using an optional salt.")]
    public class HashMessage : IMessage
    {
        /// <summary>
        /// Gets or sets the value to hash.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        [Display(Description = "The value to be hashed.")]
        public string? Value { get; set; }

        /// <summary>
        /// Gets or sets the salt.
        /// </summary>
        /// <value>
        /// The salt.
        /// </value>
        [Display(Description = "Optional. The salt used in hash. If not provided, a default salt will be used if UseDefaultSalt is set to true.")]
        public string? Salt { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this object use default salt.
        /// </summary>
        /// <value>
        /// True if use default salt, false if not.
        /// </value>
        [Display(Description = "Optional. A value indicating to use the default salt in hash if none provided.")]
        public bool UseDefaultSalt { get; set; }
    }

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