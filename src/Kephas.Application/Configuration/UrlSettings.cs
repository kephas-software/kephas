// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UrlSettings.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application.Configuration
{
    using System;

    using Kephas.Cryptography.X509Certificates;
    using Kephas.Dynamic;

    /// <summary>
    /// Settings for the endpoint URL.
    /// </summary>
    public class UrlSettings : Expando
    {
        private readonly string anyIPToken = Guid.NewGuid().ToString("n");

        /// <summary>
        /// Initializes a new instance of the <see cref="UrlSettings"/> class.
        /// </summary>
        public UrlSettings()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UrlSettings"/> class.
        /// </summary>
        /// <param name="settings">The serialized settings in form of {url}[;{certificate}].</param>
        public UrlSettings(string settings)
        {
            this.Parse(settings);
        }

        /// <summary>
        /// Gets or sets the endpoint URL template.
        /// </summary>
        public string? Url { get; set; }

        /// <summary>
        /// Gets or sets the name of the certificate used for this endpoint. Typically, the certificate is retrieved using the
        /// <see cref="ICertificateProvider.TryGetCertificate"/> method, passing this name as the parameter.
        /// </summary>
        public string? Certificate { get; set; }

        /// <summary>
        /// Gets a value indicating whether the <see cref="Url"/> is a wildcard for any IP address.
        /// </summary>
        public bool IsAnyIP => this.GetUri()?.Host == this.anyIPToken;

        /// <summary>
        /// Gets a value indicating whether the <see cref="Url"/> has a HTTPS scheme.
        /// </summary>
        public bool IsHttps => Uri.UriSchemeHttps.Equals(this.GetUri()?.Scheme, StringComparison.OrdinalIgnoreCase);

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return string.IsNullOrEmpty(this.Certificate)
                ? this.Url ?? string.Empty
                : $"{this.Url};{this.Certificate}";
        }

        /// <summary>
        /// Parses the settings into this instance.
        /// </summary>
        /// <param name="settings">The serialized settings in form of {url}[;{certificate}].</param>
        protected virtual void Parse(string settings)
        {
            var splits = settings.Split(';');
            this.Url = splits[0];
            this.Certificate = splits.Length > 1 ? splits[1] : null;
        }

        private Uri? GetUri()
            => string.IsNullOrEmpty(this.Url) ? null : new Uri(this.Url.Replace("*", this.anyIPToken));
    }
}