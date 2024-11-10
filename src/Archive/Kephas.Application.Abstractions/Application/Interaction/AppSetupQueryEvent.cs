// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppSetupQueryEvent.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application.Interaction
{
    /// <summary>
    /// Event for querying whether setup activities can be executed.
    /// </summary>
    public class AppSetupQueryEvent
    {
        /// <summary>
        /// Gets or sets a value indicating whether the setup activities are enabled.
        /// </summary>
        public bool SetupEnabled { get; set; } = true;
    }
}
