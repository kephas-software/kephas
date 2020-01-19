// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PluginState.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the plugin state class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Plugins
{
    /// <summary>
    /// Values that represent plugin states.
    /// </summary>
    /// <seealso/>
    public enum PluginState
    {
        /// <summary>
        /// The plugin is not ready yet for any operation, probably file structure being created.
        /// </summary>
        None,

        /// <summary>
        /// The plugin has been expanded in all folders, it awaits initialization.
        /// </summary>
        PendingInitialization,

        /// <summary>
        /// The plugin is completely installed and enabled.
        /// </summary>
        Enabled,

        /// <summary>
        /// The plugin is installed, but has been disabled.
        /// </summary>
        Disabled,

        /// <summary>
        /// The plugin specific data has been uninstalled, it awaits final removal of folders.
        /// </summary>
        PendingUninitialization,

        /// <summary>
        /// The plugin installation is corrupt.
        /// </summary>
        Corrupt,
    }
}
