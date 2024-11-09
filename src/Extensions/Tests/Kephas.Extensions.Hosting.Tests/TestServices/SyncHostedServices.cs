// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SyncHostedServices.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Kephas.Extensions.Tests.TestServices;

public abstract class TestSynchronizedHostedService(IHostedServiceSynchronizer hostedServiceSynchronizer, string name, ILogger? logger = null)
    : SynchronizedHostedServiceBase(hostedServiceSynchronizer)
{
    public override Task OnStartAsync(CancellationToken cancellationToken)
    {
        logger?.LogInformation($"{name} started");
        return Task.CompletedTask;
    }

    public override Task OnStopAsync(CancellationToken cancellationToken)
    {
        logger?.LogInformation($"{name} stopped");
        return Task.CompletedTask;
    }
}

[RequireSyncTokens("B")]
[GrantSyncTokens("A")]
public class AneedsBHostedService(IHostedServiceSynchronizer hostedServiceSynchronizer, ILogger? logger = null)
    : TestSynchronizedHostedService(hostedServiceSynchronizer, "A", logger);

[RequireSyncTokens("C")]
[GrantSyncTokens("B")]
public class BneedsCHostedService(IHostedServiceSynchronizer hostedServiceSynchronizer, ILogger? logger = null)
    : TestSynchronizedHostedService(hostedServiceSynchronizer, "B", logger);

[RequireSyncTokens("A")]
[GrantSyncTokens("C")]
public class CneedsAHostedService(IHostedServiceSynchronizer hostedServiceSynchronizer, ILogger? logger = null)
    : TestSynchronizedHostedService(hostedServiceSynchronizer, "C", logger);

[GrantSyncTokens("C")]
public class CneedsNoneHostedService(IHostedServiceSynchronizer hostedServiceSynchronizer, ILogger? logger = null)
    : TestSynchronizedHostedService(hostedServiceSynchronizer, "CNone", logger);

[RequireSyncTokens("Missing")]
[GrantSyncTokens("X")]
public class XneedsMissingHostedService(IHostedServiceSynchronizer hostedServiceSynchronizer, ILogger? logger = null)
    : TestSynchronizedHostedService(hostedServiceSynchronizer, "X", logger);

public class NoNeedsHostedService(IHostedServiceSynchronizer hostedServiceSynchronizer, ILogger? logger = null)
    : TestSynchronizedHostedService(hostedServiceSynchronizer, "NoNeeds", logger);
