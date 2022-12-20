// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultAppManager.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the default application manager class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

using Kephas.Application.Reflection;
using Kephas.Diagnostics;
using Kephas.Logging;
using Kephas.Resources;
using Kephas.Services;
using Kephas.Services.Behaviors;
using Kephas.Sets;
using Kephas.Threading.Tasks;

/// <summary>
/// The default application manager.
/// </summary>
[OverridePriority(Priority.Low)]
public class DefaultAppManager : Loggable, IAppManager
{
    private readonly Lazy<ICollection<Lazy<IAppLifecycleBehavior, AppServiceMetadata>>> lazyAppLifecycleBehaviorFactories;
    private readonly Lazy<ICollection<Lazy<IFeatureManager, FeatureManagerMetadata>>> lazyFeatureManagerFactories;
    private readonly Lazy<ICollection<Lazy<IFeatureLifecycleBehavior, FeatureLifecycleBehaviorMetadata>>> lazyFeatureLifecycleBehaviorFactories;

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultAppManager"/> class.
    /// </summary>
    /// <param name="appRuntime">The application runtime.</param>
    /// <param name="serviceProvider">The ambient services.</param>
    /// <param name="appLifecycleBehaviorFactories">Optional. The application lifecycle behavior factories.</param>
    /// <param name="featureManagerFactories">Optional. The feature manager factories.</param>
    /// <param name="featureLifecycleBehaviorFactories">Optional. The feature lifecycle behavior factories.</param>
    public DefaultAppManager(
        IAppRuntime appRuntime,
        IServiceProvider serviceProvider,
        IEnabledLazyEnumerable<IAppLifecycleBehavior, AppServiceMetadata>? appLifecycleBehaviorFactories = null,
        IEnabledLazyEnumerable<IFeatureManager, FeatureManagerMetadata>? featureManagerFactories = null,
        IEnabledLazyEnumerable<IFeatureLifecycleBehavior, FeatureLifecycleBehaviorMetadata>? featureLifecycleBehaviorFactories = null)
        : base(serviceProvider)
    {
        appRuntime = appRuntime ?? throw new ArgumentNullException(nameof(appRuntime));
        serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

        this.AppRuntime = appRuntime;
        this.ServiceProvider = serviceProvider;
        this.lazyAppLifecycleBehaviorFactories = new Lazy<ICollection<Lazy<IAppLifecycleBehavior, AppServiceMetadata>>>(
            () => appLifecycleBehaviorFactories == null
                ? new List<Lazy<IAppLifecycleBehavior, AppServiceMetadata>>()
                : appLifecycleBehaviorFactories.Order().ToList());

        this.lazyFeatureManagerFactories = new Lazy<ICollection<Lazy<IFeatureManager, FeatureManagerMetadata>>>(
            () => featureManagerFactories == null
                ? new List<Lazy<IFeatureManager, FeatureManagerMetadata>>()
                : this.SortEnabledFeatureManagerFactories(featureManagerFactories.ToList()));

        this.lazyFeatureLifecycleBehaviorFactories = new Lazy<ICollection<Lazy<IFeatureLifecycleBehavior, FeatureLifecycleBehaviorMetadata>>>(
            () => featureLifecycleBehaviorFactories == null
                ? new List<Lazy<IFeatureLifecycleBehavior, FeatureLifecycleBehaviorMetadata>>()
                : featureLifecycleBehaviorFactories.Order().ToList());
    }

    /// <summary>
    /// Gets the application lifecycle behavior factories.
    /// </summary>
    /// <value>
    /// The application lifecycle behavior factories.
    /// </value>
    protected internal ICollection<Lazy<IAppLifecycleBehavior, AppServiceMetadata>> AppLifecycleBehaviorFactories
        => this.lazyAppLifecycleBehaviorFactories.Value;

    /// <summary>
    /// Gets the feature manager factories.
    /// </summary>
    /// <value>
    /// The feature manager factories.
    /// </value>
    protected internal ICollection<Lazy<IFeatureManager, FeatureManagerMetadata>> FeatureManagerFactories
        => this.lazyFeatureManagerFactories.Value;

    /// <summary>
    /// Gets the feature lifecycle behavior factories.
    /// </summary>
    /// <value>
    /// The feature lifecycle behavior factories.
    /// </value>
    protected internal ICollection<Lazy<IFeatureLifecycleBehavior, FeatureLifecycleBehaviorMetadata>> FeatureLifecycleBehaviorFactories
        => this.lazyFeatureLifecycleBehaviorFactories.Value;

    /// <summary>
    /// Gets the application runtime.
    /// </summary>
    protected IAppRuntime AppRuntime { get; }

    /// <summary>
    /// Gets the injector.
    /// </summary>
    /// <value>
    /// The injector.
    /// </value>
    protected IServiceProvider ServiceProvider { get; }

    /// <summary>
    /// Initializes the application asynchronously.
    /// </summary>
    /// <param name="appContext">Context for the application.</param>
    /// <param name="cancellationToken">Optional. The cancellation token.</param>
    /// <returns>
    /// A Task.
    /// </returns>
    public virtual async Task InitializeAsync(IAppContext? appContext, CancellationToken cancellationToken = default)
    {
        appContext = appContext ?? throw new ArgumentNullException(nameof(appContext));

        try
        {
            await Profiler.WithInfoStopwatchAsync(
                async () =>
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var orderedBehaviors = this.AppLifecycleBehaviorFactories
                        .ToList();

                    await this.BeforeAppInitializeAsync(orderedBehaviors, appContext, cancellationToken).PreserveThreadContext();
                    cancellationToken.ThrowIfCancellationRequested();

                    // set the features in the app manifest.
                    var features = this.FeatureManagerFactories
                        .Select(f => f.Metadata.FeatureInfo!)
                        .ToList();
                    this.SetAppRuntimeFeatures(features);

                    await this.InitializeFeaturesAsync(appContext, cancellationToken).PreserveThreadContext();
                    cancellationToken.ThrowIfCancellationRequested();

                    await this.AfterAppInitializeAsync(orderedBehaviors, appContext, cancellationToken).PreserveThreadContext();
                    cancellationToken.ThrowIfCancellationRequested();
                },
                this.Logger).PreserveThreadContext();
        }
        catch (OperationCanceledException)
        {
            this.Logger.Error(AbstractionStrings.DefaultAppManager_InitializeCanceled_Exception, DateTimeOffset.Now);
            throw;
        }
        catch (Exception ex)
        {
            this.Logger.Error(ex, AbstractionStrings.DefaultAppManager_InitializeFaulted_Exception, DateTimeOffset.Now);
            throw;
        }
    }

    /// <summary>
    /// Finalizes the application asynchronously.
    /// </summary>
    /// <param name="appContext">Context for the application.</param>
    /// <param name="cancellationToken">Optional. The cancellation token.</param>
    /// <returns>
    /// An asynchronous result.
    /// </returns>
    public virtual async Task FinalizeAsync(IAppContext? appContext, CancellationToken cancellationToken = default)
    {
        appContext = appContext ?? throw new ArgumentNullException(nameof(appContext));

        try
        {
            await Profiler.WithInfoStopwatchAsync(
                async () =>
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var reversedOrderBehaviors = this.AppLifecycleBehaviorFactories
                        .Reverse()
                        .ToList();

                    await this.BeforeAppFinalizeAsync(reversedOrderBehaviors, appContext, cancellationToken).PreserveThreadContext();
                    cancellationToken.ThrowIfCancellationRequested();

                    await this.FinalizeFeaturesAsync(appContext, cancellationToken).PreserveThreadContext();
                    cancellationToken.ThrowIfCancellationRequested();

                    await this.AfterAppFinalizeAsync(reversedOrderBehaviors, appContext, cancellationToken).PreserveThreadContext();
                    cancellationToken.ThrowIfCancellationRequested();
                },
                this.Logger).PreserveThreadContext();
        }
        catch (OperationCanceledException)
        {
            this.Logger.Error(AbstractionStrings.DefaultAppManager_FinalizeCanceled_Exception, DateTimeOffset.Now);
            throw;
        }
        catch (Exception ex)
        {
            this.Logger.Error(ex, AbstractionStrings.DefaultAppManager_FinalizeFaulted_Exception, DateTimeOffset.Now);
            throw;
        }
    }

    /// <summary>
    /// Overridable method called before actually initializing the application.
    /// </summary>
    /// <param name="behaviors">The behaviors.</param>
    /// <param name="appContext">Context for the application.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// A Task.
    /// </returns>
    protected virtual async Task BeforeAppInitializeAsync(
        IEnumerable<Lazy<IAppLifecycleBehavior, AppServiceMetadata>> behaviors,
        IAppContext appContext,
        CancellationToken cancellationToken)
    {
        foreach (var behavior in behaviors)
        {
            await Profiler.WithTraceStopwatchAsync(
                () => behavior.Value.BeforeAppInitializeAsync(appContext, cancellationToken),
                this.Logger,
                this.GetBehaviorInfo(behavior.Metadata)).PreserveThreadContext();
        }
    }

    /// <summary>
    /// Overridable method called after the application was initialized.
    /// </summary>
    /// <param name="behaviors">The behaviors.</param>
    /// <param name="appContext">Context for the application.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// A Task.
    /// </returns>
    protected virtual async Task AfterAppInitializeAsync(
        IEnumerable<Lazy<IAppLifecycleBehavior, AppServiceMetadata>> behaviors,
        IAppContext appContext,
        CancellationToken cancellationToken)
    {
        foreach (var behavior in behaviors)
        {
            await Profiler.WithTraceStopwatchAsync(
                () => behavior.Value.AfterAppInitializeAsync(appContext, cancellationToken),
                this.Logger,
                this.GetBehaviorInfo(behavior.Metadata)).PreserveThreadContext();
        }
    }

    /// <summary>
    /// Invokes the initialization of feature managers asynchronously.
    /// </summary>
    /// <param name="appContext">Context for the application.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// A Task.
    /// </returns>
    protected virtual async Task InitializeFeaturesAsync(IAppContext appContext, CancellationToken cancellationToken)
    {
        var orderedFeatureManagers = this.FeatureManagerFactories
            .ToList();

        var orderedBehaviors = this.FeatureLifecycleBehaviorFactories
            .ToList();

        cancellationToken.ThrowIfCancellationRequested();

        foreach (var featureManagerFactory in orderedFeatureManagers)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var featureManager = featureManagerFactory.Value;
            var featureManagerMetadata = featureManagerFactory.Metadata;

            var (featureIdentifier, featureType) = this.GetFeatureInfo(featureManager, featureManagerMetadata);
            try
            {
                await Profiler.WithInfoStopwatchAsync(
                    async () =>
                    {
                        await this.BeforeFeatureInitializeAsync(orderedBehaviors, appContext, featureManagerMetadata, cancellationToken).PreserveThreadContext();
                        await this.InitializeFeatureAsync(featureManager, appContext, cancellationToken).PreserveThreadContext();
                        await this.AfterFeatureInitializeAsync(orderedBehaviors, appContext, featureManagerMetadata, cancellationToken).PreserveThreadContext();
                    },
                    this.Logger,
                    "Initialize feature " + featureIdentifier).PreserveThreadContext();
            }
            catch (OperationCanceledException cex)
            {
                this.Logger.Error(cex, "{feature} was canceled during initialization. The current operation will be interrupted.", featureIdentifier);
                throw;
            }
            catch (Exception ex)
            {
                var isRequiredFeature = featureManagerMetadata?.FeatureInfo.IsRequired ?? false;
                var featureKind = isRequiredFeature ? "required" : "optional";
                this.Logger.Error(ex, "{feature} ({featureKind}) failed to initialize. See the inner exception for more details.", featureIdentifier, featureKind);

                // interrupt the initialization if a required feature manager failed to initialize.
                if (isRequiredFeature)
                {
                    throw;
                }

                // remove the failed feature from the app manifest.
                this.RemoveFailedAppRuntimeFeature(featureManagerMetadata.FeatureInfo);
            }

            cancellationToken.ThrowIfCancellationRequested();
        }
    }

    /// <summary>
    /// Initializes the feature asynchronously.
    /// </summary>
    /// <param name="featureManager">Manager for feature.</param>
    /// <param name="appContext">Context for the application.</param>
    /// <param name="cancellationToken">The cancellation token (optional).</param>
    /// <returns>
    /// The asynchronous result.
    /// </returns>
    protected virtual Task InitializeFeatureAsync(
        IFeatureManager featureManager,
        IAppContext appContext,
        CancellationToken cancellationToken)
    {
        return featureManager.InitializeAsync(appContext, cancellationToken);
    }

    /// <summary>
    /// Executes the before initialization behaviors.
    /// </summary>
    /// <param name="behaviors">The behaviors.</param>
    /// <param name="appContext">Context for the application.</param>
    /// <param name="appServiceMetadata">The application service metadata.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// A Task.
    /// </returns>
    protected virtual async Task BeforeFeatureInitializeAsync(
        ICollection<Lazy<IFeatureLifecycleBehavior, FeatureLifecycleBehaviorMetadata>> behaviors,
        IAppContext appContext,
        FeatureManagerMetadata appServiceMetadata,
        CancellationToken cancellationToken)
    {
        foreach (var behavior in behaviors)
        {
            var featureRef = behavior.Metadata.Target;
            if (featureRef == null || featureRef.IsMatch(appServiceMetadata.FeatureInfo))
            {
                await Profiler.WithTraceStopwatchAsync(
                    () => behavior.Value.BeforeInitializeAsync(appContext, appServiceMetadata, cancellationToken),
                    this.Logger,
                    this.GetBehaviorInfo(behavior.Metadata)).PreserveThreadContext();
            }
        }
    }

    /// <summary>
    /// Executes the after initialization behaviors.
    /// </summary>
    /// <param name="behaviors">The behaviors.</param>
    /// <param name="appContext">Context for the application.</param>
    /// <param name="appServiceMetadata">The application service metadata.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// A Task.
    /// </returns>
    protected virtual async Task AfterFeatureInitializeAsync(
        ICollection<Lazy<IFeatureLifecycleBehavior, FeatureLifecycleBehaviorMetadata>> behaviors,
        IAppContext appContext,
        FeatureManagerMetadata appServiceMetadata,
        CancellationToken cancellationToken)
    {
        foreach (var behavior in behaviors)
        {
            var featureRef = behavior.Metadata.Target;
            if (featureRef == null || featureRef.IsMatch(appServiceMetadata.FeatureInfo))
            {
                await Profiler.WithTraceStopwatchAsync(
                    () => behavior.Value.AfterInitializeAsync(appContext, appServiceMetadata, cancellationToken),
                    this.Logger,
                    this.GetBehaviorInfo(behavior.Metadata)).PreserveThreadContext();
            }
        }
    }

    /// <summary>
    /// Overridable method called before actually finalizing the application.
    /// </summary>
    /// <param name="behaviors">The behaviors.</param>
    /// <param name="appContext">Context for the application.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// A Task.
    /// </returns>
    protected virtual async Task BeforeAppFinalizeAsync(
        IEnumerable<Lazy<IAppLifecycleBehavior, AppServiceMetadata>> behaviors,
        IAppContext appContext,
        CancellationToken cancellationToken)
    {
        foreach (var behavior in behaviors)
        {
            await Profiler.WithTraceStopwatchAsync(
                () => behavior.Value.BeforeAppFinalizeAsync(appContext, cancellationToken),
                this.Logger,
                this.GetBehaviorInfo(behavior.Metadata)).PreserveThreadContext();
        }
    }

    /// <summary>
    /// Overridable method called after the application was finalized.
    /// </summary>
    /// <param name="behaviors">The behaviors.</param>
    /// <param name="appContext">Context for the application.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// A Task.
    /// </returns>
    protected virtual async Task AfterAppFinalizeAsync(
        IEnumerable<Lazy<IAppLifecycleBehavior, AppServiceMetadata>> behaviors,
        IAppContext appContext,
        CancellationToken cancellationToken)
    {
        foreach (var behavior in behaviors)
        {
            await Profiler.WithTraceStopwatchAsync(
                () => behavior.Value.AfterAppFinalizeAsync(appContext, cancellationToken),
                this.Logger,
                this.GetBehaviorInfo(behavior.Metadata)).PreserveThreadContext();
        }
    }

    /// <summary>
    /// Invokes the finalization of feature managers asynchronously.
    /// </summary>
    /// <param name="appContext">Context for the application.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// A Task.
    /// </returns>
    protected virtual async Task FinalizeFeaturesAsync(IAppContext appContext, CancellationToken cancellationToken)
    {
        // the feature manager are in the right order for initializing, now reverse that order
        var reversedOrderedFeatureManagers = this.FeatureManagerFactories
            .Reverse()
            .ToList();

        var reverseOrderedBehaviors = this.FeatureLifecycleBehaviorFactories
            .Reverse()
            .ToList();

        cancellationToken.ThrowIfCancellationRequested();

        foreach (var featureManagerFactory in reversedOrderedFeatureManagers)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var featureManager = featureManagerFactory.Value;
            var featureManagerMetadata = featureManagerFactory.Metadata;

            var (featureIdentifier, featureType) = this.GetFeatureInfo(featureManager, featureManagerMetadata);
            try
            {
                await Profiler.WithInfoStopwatchAsync(
                    async () =>
                    {
                        await this.BeforeFeatureFinalizeAsync(reverseOrderedBehaviors, appContext, featureManagerMetadata, cancellationToken).PreserveThreadContext();
                        await this.FinalizeFeatureAsync(featureManager, appContext, cancellationToken).PreserveThreadContext();
                        await this.AfterFeatureFinalizeAsync(reverseOrderedBehaviors, appContext, featureManagerMetadata, cancellationToken).PreserveThreadContext();
                    },
                    this.Logger,
                    "Finalize feature " + featureIdentifier).PreserveThreadContext();
            }
            catch (OperationCanceledException cex)
            {
                this.Logger.Error(cex, "{feature} was canceled during finalization. The current operation will be interrupted.", featureIdentifier);
                throw;
            }
            catch (Exception ex)
            {
                var isRequiredFeature = featureManagerMetadata?.FeatureInfo.IsRequired ?? false;
                var featureKind = isRequiredFeature ? "required" : "optional";
                this.Logger.Error(ex, "{feature} ({featureKind}) failed to finalize. See the inner exception for more details.", featureIdentifier, featureKind);

                // interrupt the finalization if a required feature manager failed to finalize.
                if (isRequiredFeature)
                {
                    throw;
                }
            }

            cancellationToken.ThrowIfCancellationRequested();
        }
    }

    /// <summary>
    /// Finalizes the feature asynchronously.
    /// </summary>
    /// <param name="featureManager">Manager for feature.</param>
    /// <param name="appContext">Context for the application.</param>
    /// <param name="cancellationToken">The cancellation token (optional).</param>
    /// <returns>
    /// The asynchronous result.
    /// </returns>
    protected virtual Task FinalizeFeatureAsync(
        IFeatureManager featureManager,
        IAppContext appContext,
        CancellationToken cancellationToken)
    {
        return featureManager.FinalizeAsync(appContext, cancellationToken);
    }

    /// <summary>
    /// Executes the before finalization behaviors.
    /// </summary>
    /// <param name="behaviors">The behaviors.</param>
    /// <param name="appContext">Context for the application.</param>
    /// <param name="appServiceMetadata">The application service metadata.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// A Task.
    /// </returns>
    protected virtual async Task BeforeFeatureFinalizeAsync(
        ICollection<Lazy<IFeatureLifecycleBehavior, FeatureLifecycleBehaviorMetadata>> behaviors,
        IAppContext appContext,
        FeatureManagerMetadata appServiceMetadata,
        CancellationToken cancellationToken)
    {
        foreach (var behavior in behaviors)
        {
            var featureRef = behavior.Metadata.Target;
            if (featureRef == null || featureRef.IsMatch(appServiceMetadata.FeatureInfo))
            {
                await Profiler.WithTraceStopwatchAsync(
                    () => behavior.Value.BeforeFinalizeAsync(appContext, appServiceMetadata, cancellationToken),
                    this.Logger,
                    this.GetBehaviorInfo(behavior.Metadata)).PreserveThreadContext();
            }
        }
    }

    /// <summary>
    /// Executes the after finalization behaviors.
    /// </summary>
    /// <param name="behaviors">The behaviors.</param>
    /// <param name="appContext">Context for the application.</param>
    /// <param name="appServiceMetadata">The application service metadata.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// A Task.
    /// </returns>
    protected virtual async Task AfterFeatureFinalizeAsync(
        ICollection<Lazy<IFeatureLifecycleBehavior, FeatureLifecycleBehaviorMetadata>> behaviors,
        IAppContext appContext,
        FeatureManagerMetadata appServiceMetadata,
        CancellationToken cancellationToken)
    {
        foreach (var behavior in behaviors)
        {
            var featureRef = behavior.Metadata.Target;
            if (featureRef == null || featureRef.IsMatch(appServiceMetadata.FeatureInfo))
            {
                await Profiler.WithTraceStopwatchAsync(
                    () => behavior.Value.AfterFinalizeAsync(appContext, appServiceMetadata, cancellationToken),
                    this.Logger,
                    this.GetBehaviorInfo(behavior.Metadata)).PreserveThreadContext();
            }
        }
    }

    /// <summary>
    /// Sets the features in the application runtime.
    /// </summary>
    /// <param name="features">The features.</param>
    protected virtual void SetAppRuntimeFeatures(IList<FeatureInfo> features)
    {
        this.AppRuntime.SetFeatures(features);
    }

    /// <summary>
    /// Removes the failed feature from the application runtime.
    /// </summary>
    /// <param name="featureInfo">Information describing the feature.</param>
    protected virtual void RemoveFailedAppRuntimeFeature(FeatureInfo featureInfo)
    {
        var featureInfoList = this.AppRuntime.GetFeatures() as IList<FeatureInfo>;
        featureInfoList?.Remove(featureInfo);
    }

    /// <summary>
    /// Sorts the feature manager factories.
    /// </summary>
    /// <param name="featureManagerFactories">The feature manager factories.</param>
    /// <returns>
    /// The sorted feature manager factories.
    /// </returns>
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Reviewed. Suppression is OK here.")]
    protected virtual ICollection<Lazy<IFeatureManager, FeatureManagerMetadata>> SortEnabledFeatureManagerFactories(
        ICollection<Lazy<IFeatureManager, FeatureManagerMetadata>> featureManagerFactories)
    {
        var partialOrderedSet = new PartialOrderedSet<Lazy<IFeatureManager, FeatureManagerMetadata>>(
            featureManagerFactories,
            this.CompareFeatureManagers);
        return partialOrderedSet.ToList();
    }

    /// <summary>
    /// Compares two feature managers regarding to their processing priority.
    /// </summary>
    /// <param name="fm1">The first fm.</param>
    /// <param name="fm2">The second fm.</param>
    /// <returns>
    /// <c>null</c> if the two are not comparable, -1 if fm1 must be processed before fm2, 1 if fm2 must be processed before fm1.
    /// </returns>
    private int? CompareFeatureManagers(Lazy<IFeatureManager, FeatureManagerMetadata> fm1, Lazy<IFeatureManager, FeatureManagerMetadata> fm2)
    {
        var fm1Info = fm1.Metadata.FeatureInfo!;
        var fm2Info = fm2.Metadata.FeatureInfo!;
        if (fm2Info.Dependencies.Contains(fm1Info.Name))
        {
            return -1;
        }

        if (fm1Info.Dependencies.Contains(fm2Info.Name))
        {
            return 1;
        }

        // Note: do not perform arithmetic operations because this will probably end
        // in overflows, taking into consideration that we can have int.MinValue and int.MaxValue values
        // and therefore in bad comparisons.
        if (fm1.Metadata.ProcessingPriority < fm2.Metadata.ProcessingPriority)
        {
            return -1;
        }

        if (fm1.Metadata.ProcessingPriority > fm2.Metadata.ProcessingPriority)
        {
            return 1;
        }

        return null;
    }

    private (string featureIdentifier, Type featureType) GetFeatureInfo(IFeatureManager featureManager, FeatureManagerMetadata featureManagerMetadata)
    {
        var featureType = featureManager.GetType();
        var featureIdentifier = $"'{featureManagerMetadata.FeatureInfo!.Name}' ({featureType}, #{featureManagerMetadata.ProcessingPriority})";
        return (featureIdentifier, featureType);
    }

    private string GetBehaviorInfo<TMetadata>(TMetadata behaviorMetadata, [CallerMemberName] string operation = "operation")
        where TMetadata : notnull, AppServiceMetadata
    {
        return $"{operation} -> {behaviorMetadata.ServiceType}. (#{behaviorMetadata.ProcessingPriority})";
    }
}