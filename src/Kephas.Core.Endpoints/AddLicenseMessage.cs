// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AddLicenseMessage.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Endpoints
{
    using Kephas.ComponentModel.DataAnnotations;
    using Kephas.Messaging;
    using Kephas.Security.Authorization;
    using Kephas.Security.Authorization.AttributedModel;

    /// <summary>
    /// Message for adding a license.
    /// </summary>
    [RequiresPermission(typeof(AppAdminPermission))]
    [DisplayInfo(Description = "Adds a license to the license folder. If a license with the same name already exists, it will be replaced and the old one will be renamed.")]
    public class AddLicenseMessage : IMessage
    {
        /// <summary>
        /// Gets or sets the license name.
        /// </summary>
        [DisplayInfo(Description = "The license name. This is also the name of the file which will be persisted.")]
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the license content.
        /// </summary>
        [DisplayInfo(Description = "The license content.")]
        public string? Content { get; set; }
    }
}
