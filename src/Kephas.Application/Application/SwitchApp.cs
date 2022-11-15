// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SwitchApp.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application;

using Kephas.Operations;
using Kephas.Threading.Tasks;

/// <summary>
/// App used for switching among multiple registered apps.
/// </summary>
public class SwitchApp : AppBase<AmbientServices>
{
    private readonly IList<AppEntry> appEntries = new List<AppEntry>();

    /// <summary>
    /// Initializes a new instance of the <see cref="SwitchApp"/> class.
    /// </summary>
    /// <param name="appArgs">The application arguments.</param>
    public SwitchApp(IAppArgs appArgs)
        : this(new AmbientServices(), appArgs)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SwitchApp"/> class.
    /// </summary>
    /// <param name="appArgs">The application arguments.</param>
    public SwitchApp(IEnumerable<string> appArgs)
        : this(new AmbientServices(), new AppArgs(appArgs))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SwitchApp"/> class.
    /// </summary>
    /// <param name="ambientServices">The ambient services.</param>
    /// <param name="appArgs">The application arguments.</param>
    public SwitchApp(IAmbientServices ambientServices, IEnumerable<string> appArgs)
        : this(ambientServices, new AppArgs(appArgs))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SwitchApp"/> class.
    /// </summary>
    /// <param name="ambientServices">The ambient services.</param>
    /// <param name="appArgs">The application arguments.</param>
    public SwitchApp(IAmbientServices ambientServices, IAppArgs appArgs)
        : base(ambientServices, appArgs)
    {
    }

    /// <summary>
    /// Gets a context for the application.
    /// </summary>
    /// <value>
    /// The application context.
    /// </value>
    public override IAppContext? AppContext => this.RunningApp?.AppContext;

    /// <summary>
    /// Gets a value indicating whether the application is running.
    /// </summary>
    public override bool IsRunning => this.RunningApp?.IsRunning ?? false;

    /// <summary>
    /// Gets a value indicating whether the application is shutting down.
    /// </summary>
    public override bool IsShuttingDown => this.RunningApp?.IsShuttingDown ?? false;

    /// <summary>
    /// Gets the running application.
    /// </summary>
    protected IApp? RunningApp { get; private set; }

    /// <summary>
    /// Adds an app indicating also when the app is enabled.
    /// </summary>
    /// <param name="appFactory">The <see cref="IApp"/> factory.</param>
    /// <param name="condition">Optional. Function indicating when the application should be considered.</param>
    /// <returns>This instance.</returns>
    public virtual SwitchApp AddApp(Func<IAmbientServices, IAppArgs, IApp> appFactory, Func<IAmbientServices, IAppArgs, bool>? condition = null)
    {
        this.appEntries.Add(new AppEntry(condition, appFactory));

        return this;
    }

    /// <summary>
    /// Runs the application asynchronously.
    /// </summary>
    /// <param name="mainCallback">
    ///     Optional. The callback for the main function.
    ///     If not provided, the service implementing <see cref="IAppMainLoop"/> will be invoked,
    ///     otherwise the application will end.
    /// </param>
    /// <param name="cancellationToken">Optional. The cancellation token.</param>
    /// <returns>
    /// The asynchronous result that yields the <see cref="IAppContext"/>.
    /// </returns>
    public override async Task<AppRunResult> RunAsync(Func<IAppArgs, Task<(IOperationResult result, AppShutdownInstruction instruction)>>? mainCallback = null, CancellationToken cancellationToken = default)
    {
        if (this.RunningApp?.IsRunning ?? false)
        {
            throw new InvalidOperationException("The application is already running.");
        }

        if (this.RunningApp is not null)
        {
            await this.RunningApp.DisposeAsync();
            this.RunningApp = null;
        }

        foreach (var appEntry in this.appEntries)
        {
            if (appEntry.Condition?.Invoke(this.AmbientServices, this.AppArgs) ?? true)
            {
                this.RunningApp = appEntry.AppFactory(this.AmbientServices, this.AppArgs);
                return await this.RunningApp.RunAsync(mainCallback, cancellationToken).PreserveThreadContext();
            }
        }

        return new AppRunResult(null, AppShutdownInstruction.Shutdown);
    }

    /// <summary>
    /// Shuts down the application asynchronously.
    /// </summary>
    /// <param name="cancellationToken">Optional. The cancellation token.</param>
    /// <returns>
    /// An asynchronous result.
    /// </returns>
    public override async Task ShutdownAsync(CancellationToken cancellationToken = default)
    {
        if (this.RunningApp is not { IsRunning: true })
        {
            return;
        }

        await this.RunningApp.ShutdownAsync(cancellationToken).PreserveThreadContext();
    }

    private record AppEntry(
        Func<IAmbientServices, IAppArgs, bool>? Condition,
        Func<IAmbientServices, IAppArgs, IApp> AppFactory);
}