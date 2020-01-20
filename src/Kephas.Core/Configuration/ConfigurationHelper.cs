// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigurationHelper.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the configuration helper class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Configuration
{
    using System.Runtime.CompilerServices;

    using Kephas.Application;

    /// <summary>
    /// The configuration helper class.
    /// </summary>
    public static class ConfigurationHelper
    {
        /// <summary>
        /// The default configuration folder.
        /// </summary>
        public const string DefaultConfigFolder = "Config";

        /// <summary>
        /// Gets or sets the pathname of the configuration folder.
        /// </summary>
        /// <remarks>
        /// If this value is a relative path, it is used relative to the application location as well as all the plugins.
        /// </remarks>
        /// <value>
        /// The pathname of the configuration folder.
        /// </value>
        public static string ConfigFolder { get; set; } = DefaultConfigFolder;

        /// <summary>
        /// Gets the full path of the application configuration folder.
        /// </summary>
        /// <param name="appRuntime">The app runtime to act on.</param>
        /// <returns>
        /// The full path of the application configuration folder.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetAppConfigFullPath(this IAppRuntime appRuntime) => appRuntime?.GetFullPath(ConfigFolder);
    }
}
