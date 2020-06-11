// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StartupSettings.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application.Configuration
{
    using System.Collections.Generic;

    /// <summary>
    /// Settings for the application startup.
    /// </summary>
    public class StartupSettings
    {
        /// <summary>
        /// Gets or sets the settings for the application instances.
        /// </summary>
        IDictionary<string, AppInstanceSettings> Instances { get; set; } = new Dictionary<string, AppInstanceSettings>();
    }

    /// <summary>
    /// Settings for the application instances.
    /// </summary>
    public class AppInstanceSettings
    {
    }
}
