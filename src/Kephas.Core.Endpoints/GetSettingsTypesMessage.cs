// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetSettingsTypesMessage.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Endpoints
{
    using System;

    using Kephas.ComponentModel.DataAnnotations;
    using Kephas.Messaging;
    using Kephas.Messaging.Messages;
    using Kephas.Reflection;

    /// <summary>
    /// Message for getting the setting types.
    /// </summary>
    [DisplayInfo(Description = "Gets the registered settings types.")]
    public class GetSettingsTypesMessage : IMessage
    {
    }

    /// <summary>
    /// Response to the <see cref="GetSettingsTypesMessage"/>.
    /// </summary>
    public class GetSettingsTypesResponseMessage : ResponseMessage
    {
        /// <summary>
        /// Gets or sets the settings types.
        /// </summary>
        public ITypeInfo[] SettingsTypes { get; set; } = Array.Empty<ITypeInfo>();
    }
}