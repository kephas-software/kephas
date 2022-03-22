// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IApp.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application;

using Kephas.Operations;

/// <summary>
/// Abstraction for an app.
/// </summary>
public interface IApp : IAsyncDisposable
{
    /// <summary>
    /// Gets the ambient services.
    /// </summary>
    /// <value>
    /// The ambient services.
    /// </value>
    IAmbientServices AmbientServices { get; }

    /// <summary>
    /// Gets a context for the application.
    /// </summary>
    /// <value>
    /// The application context.
    /// </value>
    IAppContext? AppContext { get; }

    /// <summary>
    /// Gets a value indicating whether the application is running.
    /// </summary>
    bool IsRunning { get; }

    /// <summary>
    /// Gets a value indicating whether the application is shutting down.
    /// </summary>
    bool IsShuttingDown { get; }

    /// <summary>
    /// Runs the application asynchronously.
    /// </summary>
    /// <param name="mainCallback">
    /// Optional. The callback for the main function.
    /// </param>
    /// <param name="cancellationToken">Optional. The cancellation token.</param>
    /// <returns>
    /// The asynchronous result that yields the <see cref="IAppContext"/>.
    /// </returns>
    Task<AppRunResult> RunAsync(
        Func<IAppArgs, Task<(IOperationResult result, AppShutdownInstruction instruction)>>? mainCallback = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Shuts down the application asynchronously.
    /// </summary>
    /// <param name="cancellationToken">Optional. The cancellation token.</param>
    /// <returns>
    /// An asynchronous result.
    /// </returns>
    Task ShutdownAsync(CancellationToken cancellationToken = default);
}