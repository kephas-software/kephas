﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICertificateProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Cryptography.X509Certificates
{
    using System.Security.Cryptography.X509Certificates;

    using Kephas.Services;

    /// <summary>
    /// Certificate provider service.
    /// </summary>
    [SingletonAppServiceContract]
    public interface ICertificateProvider
    {
        /// <summary>
        /// Tries to get the certificate with the provided name.
        /// </summary>
        /// <param name="name">The certificate name.</param>
        /// <param name="context">Optional. The context.</param>
        /// <returns>The certificate.</returns>
        X509Certificate2? TryGetCertificate(string name, IContext? context = null);
    }
}