// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HashingSettings.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Cryptography
{
    using System.Text;

    using Kephas.Configuration;

    /// <summary>
    /// Settings for hashing.
    /// </summary>
    public class HashingSettings : ISettings
    {
        /// <summary>
        /// Gets or sets the salt for hashing.
        /// </summary>
        public string Salt { get; set; } = "r3pl@ce-ME-at-0nc3!";

        /// <summary>
        /// Gets the bytes for the hashing salt.
        /// </summary>
        /// <returns>The hashing salt as bytes.</returns>
        public byte[] GetSaltBytes()
            => Encoding.UTF8.GetBytes(this.Salt);
    }
}