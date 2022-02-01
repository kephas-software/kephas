// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TenantSettingsProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Configuration.Providers;

using Kephas.Application;
using Kephas.Logging;
using Kephas.Services;
using Kephas.Threading;
using Kephas.Threading.Tasks;

/// <summary>
/// Provider for <see cref="TenantSettings"/>.
/// </summary>
[ProcessingPriority(Priority.High)]
[SettingsType(typeof(TenantSettings))]
public class TenantSettingsProvider : Loggable, ISettingsProvider
{
    private readonly IAppArgs appArgs;
    private readonly Lazy<IConfiguration<TenantsManagementSettings>> managementConfiguration;

    /// <summary>
    /// Initializes a new instance of the <see cref="TenantSettingsProvider"/> class.
    /// </summary>
    /// <param name="appArgs">The application arguments.</param>
    /// <param name="managementConfiguration">The management configuration.</param>
    /// <param name="logManager">Optional. The log manager.</param>
    public TenantSettingsProvider(
        IAppArgs appArgs,
        Lazy<IConfiguration<TenantsManagementSettings>> managementConfiguration,
        ILogManager? logManager = null)
        : base(logManager)
    {
        this.appArgs = appArgs;
        this.managementConfiguration = managementConfiguration;
    }

    /// <summary>
    /// Gets the settings with the provided type.
    /// </summary>
    /// <param name="settingsType">Type of the settings.</param>
    /// <param name="context">The context.</param>
    /// <returns>
    /// The settings.
    /// </returns>
    public object? GetSettings(Type settingsType, IContext? context)
    {
        var tenantId = this.appArgs.Tenant();
        if (string.IsNullOrEmpty(tenantId))
        {
            return new TenantSettings();
        }

        var managementSettings = this.managementConfiguration.Value.GetSettings(context);
        var tenantSettings = managementSettings.Tenants.FirstOrDefault(t => string.Equals(t.TenantId, tenantId))
            ?? new TenantSettings();
        return tenantSettings;
    }

    /// <summary>
    /// Updates the settings asynchronously.
    /// </summary>
    /// <param name="settings">The settings to be updated.</param>
    /// <param name="context">The context.</param>
    /// <param name="cancellationToken">Optional. The cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task UpdateSettingsAsync(object settings, IContext? context, CancellationToken cancellationToken = default)
    {
        var tenantSettings = (TenantSettings)settings;
        if (string.IsNullOrEmpty(tenantSettings.TenantId))
        {
            throw new InvalidOperationException("Cannot update settings with an empty tenant ID.");
        }

        var tenantId = this.appArgs.Tenant();
        if (string.IsNullOrEmpty(tenantId))
        {
            this.Logger.Warn("Updating tenant settings for '{targetTenant}' from the management application.", tenantSettings.TenantId);
        }
        else if (!tenantSettings.TenantId.Equals(tenantId, StringComparison.OrdinalIgnoreCase))
        {
            this.Logger.Error("Cannot update settings for tenant '{targetTenant}' from yours: '{tenant}'.", tenantSettings.TenantId, tenantId);
            throw new InvalidOperationException($"Cannot update settings for tenant '{tenantSettings.TenantId}' from yours: '{tenantId}'.");
        }

        using var updateLock = new Lock();
        await updateLock.EnterAsync(async () =>
        {
            var managementSettings = this.managementConfiguration.Value.GetSettings(context);
            var existingSettings = managementSettings.Tenants.FirstOrDefault(t =>
                tenantSettings.TenantId.Equals(t.TenantId, StringComparison.OrdinalIgnoreCase));
            if (existingSettings == null)
            {
                managementSettings.Tenants.Add(tenantSettings);
            }
            else
            {
                var index = managementSettings.Tenants.IndexOf(existingSettings);
                managementSettings.Tenants[index] = tenantSettings;
            }

            await this.managementConfiguration.Value.UpdateSettingsAsync(managementSettings, context, cancellationToken)
                .PreserveThreadContext();
        }).PreserveThreadContext();
    }
}