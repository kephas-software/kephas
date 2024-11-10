// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HostSettings.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application.Configuration
{
    using Kephas.Dynamic;

    /// <summary>
    /// Settings for the hosting services.
    /// </summary>
    public class HostSettings : Expando
    {
        /// <summary>
        /// Gets or sets a value indicating whether the application should run as a service (Windows service or Unix daemon).
        /// </summary>
        public bool RunAsService { get; set; }

        /// <summary>
        /// Gets or sets the urls to listen to.
        /// </summary>
        public UrlSettings[]? Urls { get; set; }
    }
}