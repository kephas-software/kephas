// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SwitchApp.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application;

using Kephas.Services.Builder;
using Kephas.Threading.Tasks;

/// <summary>
/// App used for switching among multiple registered apps.
/// </summary>
public class SwitchApp : AppBase
{
    private readonly IList<AppEntry> appEntries = new List<AppEntry>();

    /// <summary>
    /// Initializes a new instance of the <see cref="SwitchApp"/> class.
    /// </summary>
    /// <param name="appArgs">The application arguments.</param>
    public SwitchApp(IAppArgs appArgs)
        : base(appArgs)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SwitchApp"/> class.
    /// </summary>
    /// <param name="appArgs">The application arguments.</param>
    public SwitchApp(IEnumerable<string> appArgs)
        : this(new AppArgs(appArgs))
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
    public virtual SwitchApp AddApp(Func<IAppServiceCollectionBuilder, IAppArgs, IApp> appFactory, Func<IAppServiceCollectionBuilder, IAppArgs, bool>? condition = null)
    {
        this.appEntries.Add(new AppEntry(condition, appFactory));

        return this;
    }

    /// <summary>
    /// Runs the application asynchronously.
    /// </summary>
    /// <param name="cancellationToken">Optional. The cancellation token.</param>
    /// <returns>
    /// The asynchronous result that yields the <see cref="IAppContext"/>.
    /// </returns>
    public override async Task<AppRunResult> RunAsync(CancellationToken cancellationToken = default)
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
            if (appEntry.Condition?.Invoke(this.ServicesBuilder, this.AppArgs) ?? true)
            {
                this.RunningApp = appEntry.AppFactory(this.ServicesBuilder, this.AppArgs);
                return await this.RunningApp.RunAsync(cancellationToken).PreserveThreadContext();
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

    /// <summary>
    /// Configures the services.
    /// </summary>
    /// <param name="servicesBuilder">The service builder.</param>
    protected override void ConfigureServices(IAppServiceCollectionBuilder servicesBuilder)
    {
    }

    /// <summary>
    /// This is the last step in the app's configuration, when all the services are set up
    /// and the container is built. For inheritors, this is the last place where services can
    /// be added before calling. By default, it only builds the Lite container, but any other container adapter
    /// can be used, like Autofac or System.Composition.
    /// </summary>
    /// <remarks>
    /// Override this method to initialize the startup services, like log manager and configuration manager.
    /// </remarks>
    /// <param name="servicesBuilder">The services builder.</param>
    /// <returns>The service provider.</returns>
    protected override IServiceProvider BuildServiceProvider(IAppServiceCollectionBuilder servicesBuilder)
    {
        if (this.RunningApp is not { IsRunning: true })
        {
            throw new InvalidOperationException("The selected app is not running yet.");
        }

        return this.RunningApp.ServiceProvider!;
    }

    private record AppEntry(
        Func<IAppServiceCollectionBuilder, IAppArgs, bool>? Condition,
        Func<IAppServiceCollectionBuilder, IAppArgs, IApp> AppFactory);
}