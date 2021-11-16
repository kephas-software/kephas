// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PythonSettings.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scripting.Python
{
    using Kephas.Configuration;

    /// <summary>
    /// Provides the settings for the Python engine.
    /// </summary>
    public class PythonSettings : ISettings
    {
        /// <summary>
        /// Gets or sets the search paths.
        /// </summary>
        public string[]? SearchPaths { get; set; }

        /// <summary>
        /// Gets or sets the global modules loaded by default
        /// </summary>
        public string[]? GlobalModules { get; set; }
    }
}