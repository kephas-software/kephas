// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IHostedServiceSynchronizer.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.DependencyInjection;

namespace Kephas.Extensions.Hosting;

/// <summary>
/// Synchronizer for hosted services.
/// </summary>
[SingletonService]
public interface IHostedServiceSynchronizer
{
    /// <summary>
    /// Enters the start procedure of the <paramref name="hostedService"/>.
    /// </summary>
    /// <param name="hostedService">The hosted service instance.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The asynchronous result.</returns>
    Task EnterStartAsync(ISynchronizedHostedService hostedService, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Enters the stop procedure of the <paramref name="hostedService"/>.
    /// </summary>
    /// <param name="hostedService">The hosted service instance.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The asynchronous result.</returns>
    Task EnterStopAsync(ISynchronizedHostedService hostedService, CancellationToken cancellationToken = default);
}