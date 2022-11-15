// --------------------------------------------------------------------------------------------------------------------
// <copyright file="KeySettings.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore.IdentityServer4.Configuration
{
    using Kephas.Cryptography.X509Certificates;
    using Kephas.Dynamic;

    /// <summary>
    /// The key settings.
    /// </summary>
    /// <seealso cref="Expando" />
    public class KeySettings : Expando
    {
        /// <summary>
        /// The temporary key type.
        /// </summary>
        public const string TemporaryType = "Temporary";

        /// <summary>
        /// The default key type.
        /// </summary>
        public const string DefaultType = "Default";

        /// <summary>
        /// Gets or sets the key type.
        /// </summary>
        /// <value>
        /// The key type.
        /// </value>
        public string? Type { get; set; }

        /// <summary>
        /// Gets or sets the file path of the development key.
        /// </summary>
        /// <value>
        /// The file path.
        /// </value>
        public string? FilePath { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the development key is persisted.
        /// </summary>
        /// <value>
        ///   <c>true</c> if persisted; otherwise, <c>false</c>.
        /// </value>
        public bool? Persisted { get; set; } = true;

        /// <summary>
        /// Gets or sets the certificate retrieved with the <see cref="ICertificateProvider"/>.
        /// </summary>
        /// <value>
        /// The certificate.
        /// </value>
        public string? Certificate { get; set; }
    }
}
