// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SecuritySettings.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Security.Cryptography
{
    using System;
    using System.Collections.Generic;

    using Kephas.Application.Cryptography.X509Certificates;
    using Kephas.Dynamic;

    /// <summary>
    /// Settings for the security components.
    /// </summary>
    public class SecuritySettings : Expando
    {
        /// <summary>
        /// Gets the certificate information.
        /// </summary>
        public IDictionary<string, CertificateSettings>? Certificates { get; } = new Dictionary<string, CertificateSettings>(StringComparer.InvariantCultureIgnoreCase);
    }
}
