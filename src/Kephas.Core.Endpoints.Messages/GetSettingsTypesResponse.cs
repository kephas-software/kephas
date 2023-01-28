// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetSettingsTypesResponse.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Endpoints
{
    using System;

    using Kephas.Messaging.Messages;
    using Kephas.Reflection;

    /// <summary>
    /// Response to the <see cref="GetSettingsTypesMessage"/>.
    /// </summary>
    public class GetSettingsTypesResponse : Response
    {
        /// <summary>
        /// Gets or sets the settings types.
        /// </summary>
        public ITypeInfo[] SettingsTypes { get; set; } = Array.Empty<ITypeInfo>();
    }
}