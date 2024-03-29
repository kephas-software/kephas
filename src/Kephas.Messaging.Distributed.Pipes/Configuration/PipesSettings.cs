﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PipesSettings.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Pipes.Configuration
{
    using System;

    using Kephas.Configuration;
    using Kephas.Dynamic;

    /// <summary>
    /// Settings for communication over pipes.
    /// </summary>
    public class PipesSettings : Expando, ISettings
    {
        /// <summary>
        /// Gets or sets the connection timeout.
        /// </summary>
        public TimeSpan ConnectionTimeout { get; set; } = TimeSpan.FromSeconds(10);

        /// <summary>
        /// Gets or sets the server hosting the root.
        /// </summary>
        public string ServerName { get; set; } = ".";

        /// <summary>
        /// Gets or sets the namespace.
        /// </summary>
        public string? Namespace { get; set; }
    }
}