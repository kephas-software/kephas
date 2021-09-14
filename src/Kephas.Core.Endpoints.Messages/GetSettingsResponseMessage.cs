// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetSettingsResponseMessage.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Endpoints
{
    using Kephas.Messaging.Messages;

    /// <summary>
    /// Response to the <see cref="GetSettingsMessage"/>.
    /// </summary>
    public class GetSettingsResponseMessage : ResponseMessage
    {
        /// <summary>
        /// Gets or sets the settings.
        /// </summary>
        public object? Settings { get; set; }
    }
}