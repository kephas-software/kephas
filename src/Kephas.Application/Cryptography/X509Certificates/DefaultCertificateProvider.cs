// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultCertificateProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application.Cryptography.X509Certificates
{
    using System;
    using System.IO;
    using System.Security;
    using System.Security.Cryptography.X509Certificates;

    using Kephas.Application;
    using Kephas.Application.Configuration;
    using Kephas.Configuration;
    using Kephas.Cryptography;
    using Kephas.Cryptography.X509Certificates;
    using Kephas.Logging;
    using Kephas.Services;

    /// <summary>
    /// The default certificate provider.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class DefaultCertificateProvider : Loggable, ICertificateProvider
    {
        private readonly IAppRuntime appRuntime;
        private readonly IConfiguration<SystemSettings> systemConfiguration;
        private readonly IEncryptionService encryptionService;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultCertificateProvider"/> class.
        /// </summary>
        /// <param name="appRuntime">The application runtime.</param>
        /// <param name="systemConfiguration">The system configuration.</param>
        /// <param name="encryptionService">The encryption service.</param>
        /// <param name="logManager">Optional. The log manager.</param>
        public DefaultCertificateProvider(
            IAppRuntime appRuntime,
            IConfiguration<SystemSettings> systemConfiguration,
            IEncryptionService encryptionService,
            ILogManager? logManager = null)
            : base(logManager)
        {
            this.appRuntime = appRuntime;
            this.systemConfiguration = systemConfiguration;
            this.encryptionService = encryptionService;
        }

        /// <summary>
        /// Tries to get the certificate with the provided name.
        /// </summary>
        /// <param name="name">The certificate name.</param>
        /// <param name="context">Optional. The context.</param>
        /// <returns>The certificate.</returns>
        public virtual X509Certificate2? TryGetCertificate(string name, IContext? context = null)
        {
            var certificates = this.systemConfiguration.Settings.Certificates;
            if (certificates == null || !certificates.TryGetValue(name, out var certificateInfo))
            {
                return null;
            }

            if (!string.IsNullOrEmpty(certificateInfo.FilePath))
            {
                var filePath = Path.Combine(this.appRuntime.GetAppLocation(), certificateInfo.FilePath!);
                var password = certificateInfo.Password ??
                               this.encryptionService.Decrypt(certificateInfo.EncryptedPassword!);
                return new X509Certificate2(filePath, password);
            }

            var (find, itemName, itemValue) = this.GetFindOptions(certificateInfo);

            if (find == null)
            {
                throw new SecurityException(
                    $"Only certificates loaded from a file or from store '{certificateInfo.StoreLocation}/{certificateInfo.StoreName}' is supported.");
            }

            using var store = new X509Store(certificateInfo.StoreName, certificateInfo.StoreLocation);
            store.Open(OpenFlags.ReadOnly);
            var cert = find(store);
            if (cert == null)
            {
                var validString = certificateInfo.ValidOnly ? "valid " : string.Empty;
                this.Logger.Warn(
                    $"No {validString}certificate with {itemName} '{{{itemName}}}' could be found in {{store}}.",
                    itemValue,
                    $"{certificateInfo.StoreLocation}/{certificateInfo.StoreName}");
            }

            return cert;
        }

        private (Func<X509Store, X509Certificate2?>? find, string name, string value) GetFindOptions(
            CertificateSettings certificateInfo)
        {
            if (!string.IsNullOrEmpty(certificateInfo.Thumbprint))
            {
                return (store => this.FindByThumbprint(store, certificateInfo), "thumbprint",
                    certificateInfo.Thumbprint!);
            }

            if (!string.IsNullOrEmpty(certificateInfo.SubjectDistinguishedName))
            {
                return (store => this.FindBySubjectDistinguishedName(store, certificateInfo),
                    "subjectDistinguishedName", certificateInfo.SubjectDistinguishedName!);
            }

            if (!string.IsNullOrEmpty(certificateInfo.SubjectName))
            {
                return (store => this.FindBySubjectName(store, certificateInfo), "subjectName",
                    certificateInfo.SubjectName!);
            }

            return (null, string.Empty, string.Empty);
        }

        private X509Certificate2? FindBySubjectName(X509Store store, CertificateSettings certificateInfo)
        {
            var certs = store.Certificates.Find(
                X509FindType.FindBySubjectName,
                certificateInfo.SubjectName!,
                certificateInfo.ValidOnly);
            return certs.Count == 0 ? null : certs[0];
        }

        private X509Certificate2? FindByThumbprint(X509Store store, CertificateSettings certificateInfo)
        {
            var certs = store.Certificates.Find(
                X509FindType.FindByThumbprint,
                certificateInfo.Thumbprint!,
                certificateInfo.ValidOnly);
            return certs.Count == 0 ? null : certs[0];
        }

        private X509Certificate2? FindBySubjectDistinguishedName(X509Store store, CertificateSettings certificateInfo)
        {
            var certs = store.Certificates.Find(
                X509FindType.FindBySubjectDistinguishedName,
                certificateInfo.SubjectDistinguishedName!,
                certificateInfo.ValidOnly);
            return certs.Count == 0 ? null : certs[0];
        }
    }
}