// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HostedServiceSynchronizer.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Concurrent;
using Kephas.DependencyInjection;
using Kephas.Extensions.Diagnostics;
using Kephas.Reflection;
using Microsoft.Extensions.Logging;

namespace Kephas.Extensions.Hosting;

/// <summary>
/// The default implementation of the <see cref="IHostedServiceSynchronizer"/>.
/// </summary>
/// <param name="timeProvider">The time provider.</param>
/// <param name="lazyHostedServices">The enumeration of lazy hosted services.</param>
/// <param name="logger">The logger.</param>
public class HostedServiceSynchronizer(
    TimeProvider timeProvider,
    ILazyEnumerable<ISynchronizedHostedService, SynchronizedHostedServiceMetadata> lazyHostedServices,
    ILogger<HostedServiceSynchronizer> logger)
    : IHostedServiceSynchronizer
{
    private readonly ConcurrentDictionary<Type, int> completedOnStart = new();
    private readonly ConcurrentDictionary<Type, int> completedOnStop = new();
    private readonly ConcurrentDictionary<Type, HostedServiceAction> startQueue = new();
    private readonly ConcurrentDictionary<Type, HostedServiceAction> stopQueue = new();
    private IDictionary<Type, HashSet<Type>>? dependencyOnStartMap;
    private IDictionary<Type, HashSet<Type>>? dependencyOnStopMap;

    /// <summary>
    /// Enters the start procedure of the <paramref name="hostedService"/>.
    /// </summary>
    /// <param name="hostedService">The hosted service instance.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The asynchronous result.</returns>
    public Task EnterStartAsync(
        ISynchronizedHostedService hostedService,
        CancellationToken cancellationToken = default) =>
        EnterActionAsync(GetHostedServiceStartAction(hostedService, cancellationToken), completedOnStart, startQueue, cancellationToken);

    /// <summary>
    /// Enters the stop procedure of the <paramref name="hostedService"/>.
    /// </summary>
    /// <param name="hostedService">The hosted service instance.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The asynchronous result.</returns>
    public Task EnterStopAsync(
        ISynchronizedHostedService hostedService,
        CancellationToken cancellationToken = default) =>
        EnterActionAsync(GetHostedServiceStopAction(hostedService, cancellationToken), completedOnStop, stopQueue, cancellationToken);

    private async Task EnterActionAsync(
        HostedServiceAction hostedServiceAction,
        ConcurrentDictionary<Type, int> completedOnAction,
        ConcurrentDictionary<Type, HostedServiceAction> queue,
        CancellationToken cancellationToken = default)
    {
        hostedServiceAction.NotifyCompleted(completedOnAction.Keys);
        if (hostedServiceAction.CanProceed())
        {
            cancellationToken.ThrowIfCancellationRequested();
            await hostedServiceAction.Proceed(actionCompleted => Next(completedOnAction, queue, actionCompleted));
        }
        else
        {
            queue.AddOrUpdate(
                hostedServiceAction.Type,
                hostedServiceAction,
                (type, _) => throw new InvalidOperationException($"Service {type} was already added."));
        }
    }
    
    private IDictionary<Type, HashSet<Type>> DependencyOnStartMap =>
        dependencyOnStartMap ??= BuildDependencyOnStartMap();
    
    private IDictionary<Type, HashSet<Type>> DependencyOnStopMap =>
        dependencyOnStopMap ??= BuildDependencyOnStopMap();

    private IDictionary<Type, HashSet<Type>> BuildDependencyOnStartMap()
    {
        var requireMap = GetTokenMap(GetRequiredTokens);
        var grantMap = GetTokenMap(GetGrantedTokens);
        var typeRequireMap = requireMap
            .SelectMany(
                kv => kv.Value.Select(t =>
                    KeyValuePair.Create(
                        t,
                        grantMap.TryGetValue(kv.Key, out var dependencies) ? dependencies : new List<Type>())))
            .ToList();
        var dependencyMap = typeRequireMap
            .GroupBy(kv => kv.Key)
            .Select(g => KeyValuePair.Create(g.Key, new HashSet<Type>(g.SelectMany(kv => kv.Value))))
#if NET6_0
            .ToDictionary(kv => kv.Key, kv => kv.Value);
#else
            .ToDictionary();
#endif

        ValidateSatifiedRequires(typeRequireMap);
        return ValidateDependencyMap(dependencyMap);
    }

    private void ValidateSatifiedRequires(IEnumerable<KeyValuePair<Type,IList<Type>>> typeRequireMap)
    {
        var missingRequires = typeRequireMap
            .Where(kv => kv.Value.Count == 0)
            .Select(kv => kv.Key)
            .ToList();
        if (missingRequires.Count > 0)
        {
            throw new ServiceException(
                $"The following hosted services require tokens which are not granted by any other registered hosted services: {string.Join(", ", missingRequires.Select(t => t.FullName))}.");
        }
    }

    private IDictionary<Type, HashSet<Type>> BuildDependencyOnStopMap() =>
        DependencyOnStartMap
            .SelectMany(kv => kv.Value.Select(t => KeyValuePair.Create(t, kv.Key)))
            .GroupBy(kv => kv.Key)
            .Select(g => KeyValuePair.Create(g.Key, new HashSet<Type>(g.Select(kv => kv.Value))))
#if NET6_0
            .ToDictionary(kv => kv.Key, kv => kv.Value);
#else
            .ToDictionary();
#endif

    private async Task Next(
        ConcurrentDictionary<Type, int> completed,
        ConcurrentDictionary<Type, HostedServiceAction> queue,
        Type newCompleted)
    {
        completed.AddOrUpdate(newCompleted, _ => 1, (_, i) => i + 1);

        var toProceed = new List<Type>();
        do
        {
            foreach (var type in toProceed)
            {
                if (queue.TryRemove(type, out var action))
                {
                    await action.Proceed(newActionGrants => Next(completed, queue, newActionGrants));
                }
            }
            
            toProceed.Clear();
            foreach(var (type, action) in queue)
            {
                action.NotifyCompleted(completed.Keys);
                if (action.CanProceed())
                {
                    toProceed.Add(type);
                }
            }
        } while(toProceed.Count > 0);
    }

    private HostedServiceAction GetHostedServiceStartAction(ISynchronizedHostedService hostedService, CancellationToken cancellationToken)
    {
        var hostedServiceType = hostedService?.GetType() ?? throw new ArgumentNullException(nameof(hostedService));
        return new HostedServiceAction(
            hostedServiceType,
            DependencyOnStartMap.TryGetValue(hostedServiceType, out var dependencies)
                ? dependencies.ToList()
                : [],
            () => Profiler.ProfileTimeInfoAsync(
                () => hostedService.OnStartAsync(cancellationToken),
                logger,
                nameof(hostedService.OnStartAsync)),
            nameof(hostedService.OnStartAsync),
            logger);
    }

    private HostedServiceAction GetHostedServiceStopAction(ISynchronizedHostedService hostedService, CancellationToken cancellationToken)
    {
        var hostedServiceType = hostedService?.GetType() ?? throw new ArgumentNullException(nameof(hostedService));
        return new HostedServiceAction(
            hostedServiceType,
            DependencyOnStopMap.TryGetValue(hostedServiceType, out var dependencies)
                ? dependencies.ToList()
                : [],
            () => Profiler.ProfileTimeInfoAsync(
                () => hostedService.OnStopAsync(cancellationToken),
                logger,
                nameof(hostedService.OnStopAsync)),
            nameof(hostedService.OnStopAsync),
            logger);
    }

    private IDictionary<string, IList<Type>> GetTokenMap(Func<Type, IList<string>> collect)
    {
        var result = new Dictionary<string, IList<Type>>();
        foreach (var lazy in lazyHostedServices)
        {
            var type = lazy.Metadata.ImplementationType;
            if (type is null)
            {
                continue;
            }

            foreach (string token in collect(type))
            {
                if (result.TryGetValue(token, out var types))
                {
                    types.Add(type);
                }
                else
                {
                    result.Add(token, new List<Type> { type });
                }
            }
        }

        return result;
    }

    private IList<string> GetRequiredTokens(Type hostedServiceType) =>
        lazyHostedServices
            .Where(s => s.Metadata.ImplementationType == hostedServiceType)
            .SelectMany(s => s.Metadata.RequireSyncTokens.Select(t => t?.ToLower()))
            .Where(t => t is not null and not "")
            .Distinct()
            .ToList()!;

    private IList<string> GetGrantedTokens(Type hostedServiceType) =>
        lazyHostedServices
            .Where(s => s.Metadata.ImplementationType == hostedServiceType)
            .SelectMany(s => s.Metadata.GrantSyncTokens.Select(t => t?.ToLower()))
            .Where(t => t is not null and not "")
            .Distinct()
            .ToList()!;

    private IDictionary<Type,HashSet<Type>> ValidateDependencyMap(IDictionary<Type,HashSet<Type>> dependencyMap)
    {
        foreach (var kv in dependencyMap)
        {
            ValidateDependencyMap([kv.Key], dependencyMap);
        }
        
        return dependencyMap;
    }
    
    private void ValidateDependencyMap(List<Type> path, IDictionary<Type,HashSet<Type>> dependencyMap)
    {
        var lastItem = path[^1];
        if (!dependencyMap.TryGetValue(lastItem, out var dependencies))
        {
            return;
        }

        foreach (var dependency in dependencies)
        {
            if (path.Contains(dependency))
            {
                throw new CircularDependencyException($"A circular dependency has been identified among following services: {string.Join(" > ", path.Union([dependency]).Select(t => t.FullName))}");
            }

            ValidateDependencyMap(path.Union([dependency]).ToList(), dependencyMap);
        }
    }
}