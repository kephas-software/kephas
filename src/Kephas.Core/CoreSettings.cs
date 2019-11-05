// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CoreSettings.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the core settings class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas
{
    using Kephas.Threading.Tasks;

    /// <summary>
    /// The Kephas core settings.
    /// </summary>
    public class CoreSettings
    {
        /// <summary>
        /// Gets or sets the task settings.
        /// </summary>
        /// <value>
        /// The task settings.
        /// </value>
        public TaskSettings Task { get; set; } = new TaskSettings();
    }
}
