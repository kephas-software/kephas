// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SynchronizedHostedServiceBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Extensions.Hosting;

/// <summary>
/// Base class for synchronized hosted services.
/// </summary>
public abstract class SynchronizedHostedServiceBase(IHostedServiceSynchronizer hostedServiceSynchronizer)
    : ISynchronizedHostedService
{
    /// <summary>
    /// Triggered when the application host is ready to start the service.
    /// </summary>
    /// <param name="cancellationToken">Indicates that the start process has been aborted.</param>
    /// <returns>A <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous Start operation.</returns>
    public Task StartAsync(CancellationToken cancellationToken) =>
        hostedServiceSynchronizer.EnterStartAsync(
            this,
            cancellationToken);

    /// <summary>
    /// Triggered when the application host is performing a graceful shutdown.
    /// </summary>
    /// <param name="cancellationToken">Indicates that the shutdown process should no longer be graceful.</param>
    /// <returns>A <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous Stop operation.</returns>
    public Task StopAsync(CancellationToken cancellationToken) =>
        hostedServiceSynchronizer.EnterStopAsync(
            this,
            cancellationToken);

    /// <summary>
    /// Triggered when the application host is ready to start the service, and all the dependencies completed the start procedure.
    /// </summary>
    /// <param name="cancellationToken">Indicates that the start process has been aborted.</param>
    /// <returns>A <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous Start operation.</returns>
    public abstract Task OnStartAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Triggered when the application host is performing a graceful shutdown, ahead of all the dependencies.
    /// </summary>
    /// <param name="cancellationToken">Indicates that the shutdown process should no longer be graceful.</param>
    /// <returns>A <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous Stop operation.</returns>
    public abstract Task OnStopAsync(CancellationToken cancellationToken);
}