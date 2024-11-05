// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NotificationHandlerMetadata.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.DependencyInjection.Metadata;
using MediatR;

namespace Kephas.Extensions.Mediator;

/// <summary>
/// Metadata for <see cref="INotificationHandler{TNotification}"/>.
/// </summary>
public record NotificationMetadata : ServiceMetadata
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NotificationMetadata"/> class.
    /// </summary>
    /// <param name="metadata">The metadata.</param>
    public NotificationMetadata(IDictionary<string, object?>? metadata)
        : base(metadata)
    {
        if (metadata == null)
        {
            return;
        }

        NotificationType = (Type?)TryGetValue(metadata, nameof(NotificationType));
    }

    /// <summary>
    /// Gets the notification type.
    /// </summary>
    public Type? NotificationType { get; }
}