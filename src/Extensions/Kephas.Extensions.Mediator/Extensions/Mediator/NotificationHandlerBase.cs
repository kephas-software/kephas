// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NotificationHandlerBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using MediatR;

namespace Kephas.Extensions.Mediator;

/// <summary>
/// Marker interface for notification handlers implementing <see cref="INotificationHandler{TNotification}"/>.
/// </summary>
/// <remarks>
/// Handlers implementing this interface benefit from <see cref="NotificationHandlerMetadata"/>.
/// </remarks>
public interface INotificationHandlerBase : IDeclaredHandlerService;

/// <summary>
/// Base class for stream request handlers that return a response.
/// </summary>
/// <typeparam name="TNotification">The notification type.</typeparam>
public abstract class NotificationHandlerBase<TNotification> : INotificationHandler<TNotification>, INotificationHandlerBase
    where TNotification : INotification
{
    /// <summary>Handles a notification</summary>
    /// <param name="notification">The notification</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public abstract Task Handle(TNotification notification, CancellationToken cancellationToken);
}