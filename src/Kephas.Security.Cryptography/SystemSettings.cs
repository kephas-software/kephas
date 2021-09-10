// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SystemSettings.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application.Configuration
{
    using System;
    using System.Collections.Generic;

    using Kephas.Application.Cryptography.X509Certificates;
    using Kephas.Dynamic;

    /// <summary>
    /// Settings for the application runtime.
    /// </summary>
    public class SystemSettings : Expando
    {

        /// <summary>
        /// Gets the certificate information.
        /// </summary>
        public IDictionary<string, CertificateSettings>? Certificates { get; } = new Dictionary<string, CertificateSettings>(StringComparer.InvariantCultureIgnoreCase);
    }
}
