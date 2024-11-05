// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MediatorService.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using MediatR;

namespace Kephas.Extensions.Mediator;

/// <summary>
/// Replacement for the default <see cref="IMediator"/> service supporting polymorphic handlers and pipeline behaviors.
/// </summary>
public class MediatorService : MediatR.Mediator
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MediatorService"/> class.
    /// </summary>
    /// <param name="serviceProvider">The mediator service provider.</param>
    public MediatorService(IMediatorServiceProvider serviceProvider)
        : base(serviceProvider)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MediatorService"/> class.
    /// </summary>
    /// <param name="serviceProvider">The mediator service provider.</param>
    /// <param name="publisher">The notification publisher.</param>
    public MediatorService(IMediatorServiceProvider serviceProvider, INotificationPublisher publisher)
        : base(serviceProvider, publisher)
    {
    }
}