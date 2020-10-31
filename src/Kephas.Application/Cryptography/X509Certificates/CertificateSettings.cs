// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CertificateSettings.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application.Cryptography.X509Certificates
{
    using System.Security.Cryptography.X509Certificates;

    using Kephas.Dynamic;

    /// <summary>
    /// Gets the certificate settings.
    /// </summary>
    public class CertificateSettings : Expando
    {
        /// <summary>
        /// Gets or sets the subject name.
        /// </summary>
        public string? SubjectName { get; set; }

        /// <summary>
        /// Gets or sets the subject distinguished name.
        /// </summary>
        public string? SubjectDistinguishedName { get; set; }

        /// <summary>
        /// Gets or sets the store name.
        /// </summary>
        public StoreName StoreName { get; set; } = StoreName.My;

        /// <summary>
        /// Gets or sets the store location.
        /// </summary>
        public StoreLocation StoreLocation { get; set; } = StoreLocation.CurrentUser;

        /// <summary>
        /// Gets or sets the thumbprint.
        /// </summary>
        public string? Thumbprint { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether only valid certificates should be considered. Default is true.
        /// </summary>
        public bool ValidOnly { get; set; } = true;

        /// <summary>
        /// Gets or sets the path to the certificate file.
        /// </summary>
        public string? FilePath { get; set; }

        /// <summary>
        /// Gets or sets the cleat text password.
        /// </summary>
        public string? Password { get; set; }

        /// <summary>
        /// Gets or sets the encrypted password.
        /// </summary>
        public string? EncryptedPassword { get; set; }
    }
}
