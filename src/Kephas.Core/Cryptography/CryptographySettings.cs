// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CryptographySettings.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Cryptography
{
    using System.Text;

    /// <summary>
    /// Settings for cryptography.
    /// </summary>
    public class CryptographySettings
    {
        /// <summary>
        /// Gets or sets the salt for hashing.
        /// </summary>
        public string HashingSalt { get; set; } = "r3pl@ce-ME-at-0nc3!";
    }

    /// <summary>
    /// Extension methods for <see cref="CryptographySettings"/>.
    /// </summary>
    public static class CryptographySettingsExtensions
    {
        /// <summary>
        /// Gets the bytes for the hashing salt.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <returns>The hashing salt as bytes.</returns>
        public static byte[] GetHashingSaltBytes(this CryptographySettings settings)
            => Encoding.UTF8.GetBytes(settings.HashingSalt);
    }
}