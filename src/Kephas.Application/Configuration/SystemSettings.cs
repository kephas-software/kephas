// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SystemSettings.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application.Configuration
{
    using System.Collections.Generic;

    using Kephas.Dynamic;

    /// <summary>
    /// Settings for the application runtime.
    /// </summary>
    public class SystemSettings : Expando
    {
        /// <summary>
        /// Gets or sets the settings for the application instances.
        /// </summary>
        public IDictionary<string, AppSettings> Instances { get; set; } = new Dictionary<string, AppSettings>();
    }
}
