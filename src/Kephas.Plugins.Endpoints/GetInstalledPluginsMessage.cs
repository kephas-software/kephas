// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetInstalledPluginsMessage.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the get installed plugins message class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Plugins.Endpoints
{
    using Kephas.Application;
    using Kephas.ComponentModel.DataAnnotations;
    using Kephas.Messaging;
    using Kephas.Messaging.Messages;
    using Kephas.Plugins;

    /// <summary>
    /// A get installed plugins message.
    /// </summary>
    [TypeDisplay(Description = "Gets the installed plugins.")]
    public class GetInstalledPluginsMessage : IMessage
    {
    }

    /// <summary>
    /// A get installed plugins response message.
    /// </summary>
    public class GetInstalledPluginsResponseMessage : ResponseMessage
    {
        /// <summary>
        /// Gets or sets the plugins.
        /// </summary>
        /// <value>
        /// The plugins.
        /// </value>
        public PluginData[] Plugins { get; set; }
    }

    /// <summary>
    /// A plugin data.
    /// </summary>
    public class PluginData
    {
        /// <summary>
        /// Gets or sets the full pathname of the plugin folder.
        /// </summary>
        /// <value>
        /// The full pathname of the plugin folder.
        /// </value>
        public string Location { get; set; }

        /// <summary>
        /// Gets or sets the state.
        /// </summary>
        /// <value>
        /// The state.
        /// </value>
        public PluginState State { get; set; }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        /// <value>
        /// The version.
        /// </value>
        public string Version { get; set; }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return $"{new AppIdentity(this.Id, this.Version)} ({this.State} in {this.Location})";
        }
    }
}
