// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NotificationHandlerMetadata.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.DependencyInjection.Metadata;
using MediatR;

namespace Kephas.Extensions.Mediator
{
    /// <summary>
    /// Metadata for <see cref="INotificationHandler{TNotification}"/>.
    /// </summary>
    public class NotificationHandlerMetadata : ServiceMetadata
    {
        /// <summary>
        /// Gets or sets the notification type.
        /// </summary>
        public Type? NotificationType { get; set; }
    }
}