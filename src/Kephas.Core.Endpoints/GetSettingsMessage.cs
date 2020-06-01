// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetSettingsMessage.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Endpoints
{
    using Kephas.ComponentModel.DataAnnotations;
    using Kephas.Messaging;
    using Kephas.Messaging.Messages;

    /// <summary>
    /// A get settings message.
    /// </summary>
    [DisplayInfo(Description = "Gets the provided settings.")]
    public class GetSettingsMessage : IMessage
    {
        /// <summary>
        /// Gets or sets the settings type.
        /// </summary>
        [DisplayInfo(Description = "The name of the settings type to retrieve. The 'Settings' ending may be left out.")]
        public string? SettingsType { get; set; }
    }

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