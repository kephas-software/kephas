// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IApp.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application;

using Kephas.Services.Builder;
using Kephas.Threading.Tasks;

/// <summary>
/// Abstraction for an app.
/// </summary>
public interface IApp : IAsyncDisposable
{
    /// <summary>
    /// Gets the application services builder.
    /// </summary>
    /// <value>
    /// The application services builder.
    /// </value>
    public IAppServiceCollectionBuilder ServicesBuilder { get; }

    /// <summary>
    /// Gets the ambient services.
    /// </summary>
    /// <value>
    /// The ambient services.
    /// </value>
    IAmbientServices AmbientServices => this.ServicesBuilder.AmbientServices;

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
    /// <param name="cancellationToken">Optional. The cancellation token.</param>
    /// <returns>
    /// The asynchronous result that yields the <see cref="IAppContext"/>.
    /// </returns>
    Task<AppRunResult> RunAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Shuts down the application asynchronously.
    /// </summary>
    /// <param name="cancellationToken">Optional. The cancellation token.</param>
    /// <returns>
    /// An asynchronous result.
    /// </returns>
    Task ShutdownAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Extension methods for <see cref="IApp"/>.
/// </summary>
public static class AppExtensions
{
    /// <summary>
    /// Bootstraps the application asynchronously.
    /// </summary>
    /// <param name="app">The application.</param>
    /// <param name="errorCode">The error code to return if the running fails.</param>
    /// <returns>An async result.</returns>
    public static async Task<int> RunAsync(this IApp app, int errorCode)
    {
        try
        {
            await app.RunAsync(null, default).PreserveThreadContext();
            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return errorCode;
        }
    }
}