// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultAppManager.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the default application manager class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application.Composition;
    using Kephas.Composition;
    using Kephas.Diagnostics;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Logging;
    using Kephas.Resources;
    using Kephas.Services;
    using Kephas.Services.Behavior;
    using Kephas.Services.Composition;
    using Kephas.Sets;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// The default application manager.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class DefaultAppManager : IAppManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultAppManager"/> class.
        /// </summary>
        /// <param name="appManifest">The application manifest.</param>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="serviceBehaviorProvider">The service behavior provider.</param>
        /// <param name="appLifecycleBehaviorFactories">The application lifecycle behavior factories.</param>
        /// <param name="featureManagerFactories">The feature manager factories.</param>
        /// <param name="featureLifecycleBehaviorFactories">The feature lifecycle behavior factories.</param>
        public DefaultAppManager(
            IAppManifest appManifest,
            IAmbientServices ambientServices,
            IServiceBehaviorProvider serviceBehaviorProvider,
            ICollection<IExportFactory<IAppLifecycleBehavior, AppServiceMetadata>> appLifecycleBehaviorFactories,
            ICollection<IExportFactory<IFeatureManager, FeatureManagerMetadata>> featureManagerFactories,
            ICollection<IExportFactory<IFeatureLifecycleBehavior, AppServiceMetadata>> featureLifecycleBehaviorFactories)
        {
            Requires.NotNull(appManifest, nameof(appManifest));
            Requires.NotNull(ambientServices, nameof(ambientServices));
            Requires.NotNull(serviceBehaviorProvider, nameof(serviceBehaviorProvider));

            this.AppManifest = appManifest;
            this.AmbientServices = ambientServices;
            this.ServiceBehaviorProvider = serviceBehaviorProvider;
            this.AppLifecycleBehaviorFactories = appLifecycleBehaviorFactories == null
                                                     ? new List<IExportFactory<IAppLifecycleBehavior, AppServiceMetadata>>()
                                                     : this.ServiceBehaviorProvider.WhereEnabled(appLifecycleBehaviorFactories).ToList();

            this.EnsureMetadataHasFeatureInfo(featureManagerFactories);
            this.FeatureManagerFactories = featureManagerFactories == null
                                               ? new List<IExportFactory<IFeatureManager, FeatureManagerMetadata>>()
                                               : this.SortEnabledFeatureManagerFactories(
                                                   this.ServiceBehaviorProvider.WhereEnabled(featureManagerFactories).ToList());

            this.FeatureLifecycleBehaviorFactories = featureLifecycleBehaviorFactories == null
                                                         ? new List<IExportFactory<IFeatureLifecycleBehavior, AppServiceMetadata>>()
                                                         : this.ServiceBehaviorProvider.WhereEnabled(featureLifecycleBehaviorFactories).ToList();
        }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        public ILogger<IAppManager> Logger { get; set; }

        /// <summary>
        /// Gets the application manifest.
        /// </summary>
        public IAppManifest AppManifest { get; }

        /// <summary>
        /// Gets the ambient services.
        /// </summary>
        /// <value>
        /// The ambient services.
        /// </value>
        public IAmbientServices AmbientServices { get; }

        /// <summary>
        /// Gets the service behavior provider.
        /// </summary>
        public IServiceBehaviorProvider ServiceBehaviorProvider { get; }

        /// <summary>
        /// Gets the application lifecycle behavior factories.
        /// </summary>
        /// <value>
        /// The application lifecycle behavior factories.
        /// </value>
        public ICollection<IExportFactory<IAppLifecycleBehavior, AppServiceMetadata>> AppLifecycleBehaviorFactories { get; }

        /// <summary>
        /// Gets the feature manager factories.
        /// </summary>
        /// <value>
        /// The feature manager factories.
        /// </value>
        public ICollection<IExportFactory<IFeatureManager, FeatureManagerMetadata>> FeatureManagerFactories { get; }

        /// <summary>
        /// Gets the feature lifecycle behavior factories.
        /// </summary>
        /// <value>
        /// The feature lifecycle behavior factories.
        /// </value>
        public ICollection<IExportFactory<IFeatureLifecycleBehavior, AppServiceMetadata>> FeatureLifecycleBehaviorFactories { get; }

        /// <summary>
        /// Initializes the application asynchronously.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A Task.
        /// </returns>
        public virtual async Task InitializeAppAsync(IAppContext appContext, CancellationToken cancellationToken = default)
        {
            try
            {
                // set the features in the app manifest.
                var features = this.FeatureManagerFactories
                    .Select(f => f.Metadata.FeatureInfo)
                    .ToList();
                if (this.AppManifest is AppManifestBase appManifest)
                {
                    appManifest.Features = features;
                }

                // registers the application context as a global service, so that other services can benefit from it.
                this.AmbientServices.RegisterService(appContext);

                await Profiler.WithInfoStopwatchAsync(
                    async () =>
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        var orderedBehaviors = this.AppLifecycleBehaviorFactories
                            .Select(factory => factory.CreateExport())
                            .OrderBy(export => export.Metadata.ProcessingPriority)
                            .ToList();

                        await this.BeforeAppInitializeAsync(orderedBehaviors, appContext, cancellationToken).PreserveThreadContext();
                        cancellationToken.ThrowIfCancellationRequested();

                        await this.InitializeFeaturesAsync(appContext, cancellationToken).PreserveThreadContext();
                        cancellationToken.ThrowIfCancellationRequested();

                        await this.AfterAppInitializeAsync(orderedBehaviors, appContext, cancellationToken).PreserveThreadContext();
                        cancellationToken.ThrowIfCancellationRequested();
                    },
                    this.Logger).PreserveThreadContext();
            }
            catch (OperationCanceledException)
            {
                this.Logger.Error(Strings.DefaultAppManager_InitializeCanceled_Exception, DateTimeOffset.Now);
                throw;
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex, Strings.DefaultAppManager_InitializeFaulted_Exception, DateTimeOffset.Now);
                throw;
            }
        }

        /// <summary>
        /// Finalizes the application asynchronously.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        public virtual async Task FinalizeAppAsync(IAppContext appContext, CancellationToken cancellationToken = default)
        {
            try
            {
                await Profiler.WithInfoStopwatchAsync(
                    async () =>
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        var orderedBehaviors = this.AppLifecycleBehaviorFactories
                            .Select(factory => factory.CreateExport())
                            .OrderByDescending(export => export.Metadata.ProcessingPriority)
                            .ToList();

                        await this.BeforeAppFinalizeAsync(orderedBehaviors, appContext, cancellationToken).PreserveThreadContext();
                        cancellationToken.ThrowIfCancellationRequested();

                        await this.FinalizeFeaturesAsync(appContext, cancellationToken).PreserveThreadContext();
                        cancellationToken.ThrowIfCancellationRequested();

                        await this.AfterAppFinalizeAsync(orderedBehaviors, appContext, cancellationToken).PreserveThreadContext();
                        cancellationToken.ThrowIfCancellationRequested();
                    },
                    this.Logger).PreserveThreadContext();
            }
            catch (OperationCanceledException)
            {
                this.Logger.Error(Strings.DefaultAppManager_FinalizeCanceled_Exception, DateTimeOffset.Now);
                throw;
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex, Strings.DefaultAppManager_FinalizeFaulted_Exception, DateTimeOffset.Now);
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
            ICollection<IExport<IAppLifecycleBehavior, AppServiceMetadata>> behaviors,
            IAppContext appContext,
            CancellationToken cancellationToken)
        {
            foreach (var behavior in behaviors)
            {
                await behavior.Value.BeforeAppInitializeAsync(appContext, cancellationToken).PreserveThreadContext();
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
            ICollection<IExport<IAppLifecycleBehavior, AppServiceMetadata>> behaviors,
            IAppContext appContext,
            CancellationToken cancellationToken)
        {
            foreach (var behavior in behaviors)
            {
                await behavior.Value.AfterAppInitializeAsync(appContext, cancellationToken).PreserveThreadContext();
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
                                          .Select(factory => factory.CreateExport())
                                          .ToList();

            var orderedBehaviors = this.FeatureLifecycleBehaviorFactories
                                          .Select(factory => factory.CreateExport())
                                          .OrderBy(export => export.Metadata.ProcessingPriority)
                                          .ToList();

            var reverseOrderedBehaviors = orderedBehaviors
                                          .OrderByDescending(export => export.Metadata.ProcessingPriority)
                                          .ToList();

            cancellationToken.ThrowIfCancellationRequested();

            foreach (var featureManagerFactory in orderedFeatureManagers)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var featureManager = featureManagerFactory.Value;
                var featureManagerMetadata = featureManagerFactory.Metadata;

                var featureManagerType = featureManager.GetType();
                var featureManagerIdentifier = $"'{featureManagerMetadata.FeatureInfo.Name}' ({featureManagerType})";
                try
                {
                    await Profiler.WithInfoStopwatchAsync(
                        async () =>
                        {
                            await this.BeforeFeatureInitializeAsync(orderedBehaviors, appContext, featureManagerMetadata, cancellationToken).PreserveThreadContext();
                            await this.InitializeFeatureAsync(featureManager, appContext, cancellationToken).PreserveThreadContext();
                            await this.AfterFeatureInitializeAsync(reverseOrderedBehaviors, appContext, featureManagerMetadata, cancellationToken).PreserveThreadContext();
                        },
                        this.Logger,
                        "Initialize feature " + featureManagerIdentifier).PreserveThreadContext();
                }
                catch (OperationCanceledException cex)
                {
                    this.Logger.Error(cex, $"{featureManagerIdentifier} was canceled during initialization. The current operation will be interrupted.");
                    throw;
                }
                catch (Exception ex)
                {
                    var isRequiredFeature = featureManagerMetadata?.FeatureInfo.IsRequired ?? false;
                    var initializerKind = isRequiredFeature ? "required" : "optional";
                    this.Logger.Error(ex, $"{featureManagerIdentifier} ({initializerKind}) failed to initialize. See the inner exception for more details.");

                    // interrupt the initialization if a required feature manager failed to initialize.
                    if (isRequiredFeature)
                    {
                        throw;
                    }
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
            ICollection<IExport<IFeatureLifecycleBehavior, AppServiceMetadata>> behaviors,
            IAppContext appContext,
            FeatureManagerMetadata appServiceMetadata,
            CancellationToken cancellationToken)
        {
            foreach (var behavior in behaviors)
            {
                await behavior.Value.BeforeInitializeAsync(appContext, appServiceMetadata, cancellationToken).PreserveThreadContext();
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
            ICollection<IExport<IFeatureLifecycleBehavior, AppServiceMetadata>> behaviors,
            IAppContext appContext,
            FeatureManagerMetadata appServiceMetadata,
            CancellationToken cancellationToken)
        {
            foreach (var behavior in behaviors)
            {
                await behavior.Value.AfterInitializeAsync(appContext, appServiceMetadata, cancellationToken).PreserveThreadContext();
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
            ICollection<IExport<IAppLifecycleBehavior, AppServiceMetadata>> behaviors,
            IAppContext appContext,
            CancellationToken cancellationToken)
        {
            foreach (var behavior in behaviors)
            {
                await behavior.Value.BeforeAppFinalizeAsync(appContext, cancellationToken).PreserveThreadContext();
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
            ICollection<IExport<IAppLifecycleBehavior, AppServiceMetadata>> behaviors,
            IAppContext appContext,
            CancellationToken cancellationToken)
        {
            foreach (var behavior in behaviors)
            {
                await behavior.Value.AfterAppFinalizeAsync(appContext, cancellationToken).PreserveThreadContext();
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
            var reversedOrderedFeatureManagers = this.FeatureManagerFactories
                                          .Select(factory => factory.CreateExport())
                                          .ToList();
            // the feature manager are in the right order for initializing, now reverse that order
            reversedOrderedFeatureManagers.Reverse();

            var reverseOrderedBehaviors = this.FeatureLifecycleBehaviorFactories
                                          .Select(factory => factory.CreateExport())
                                          .OrderByDescending(export => export.Metadata.ProcessingPriority)
                                          .ToList();

            cancellationToken.ThrowIfCancellationRequested();

            foreach (var featureManagerFactory in reversedOrderedFeatureManagers)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var featureManager = featureManagerFactory.Value;
                var featureManagerMetadata = featureManagerFactory.Metadata;

                var featureManagerType = featureManager.GetType();
                var featureManagerIdentifier = $"Finalize feature '{featureManagerType}' (#{featureManagerMetadata.FeatureInfo.Name})";
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
                        featureManagerIdentifier).PreserveThreadContext();
                }
                catch (OperationCanceledException cex)
                {
                    this.Logger.Error(cex, $"{featureManagerIdentifier} was canceled during finalization. The current operation will be interrupted.");
                    throw;
                }
                catch (Exception ex)
                {
                    var isRequiredFeature = featureManagerMetadata?.FeatureInfo.IsRequired ?? false;
                    var initializerKind = isRequiredFeature ? "required" : "optional";
                    this.Logger.Error(ex, $"{featureManagerIdentifier} ({initializerKind}) failed to finalize. See the inner exception for more details.");

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
            ICollection<IExport<IFeatureLifecycleBehavior, AppServiceMetadata>> behaviors,
            IAppContext appContext,
            FeatureManagerMetadata appServiceMetadata,
            CancellationToken cancellationToken)
        {
            foreach (var behavior in behaviors)
            {
                await behavior.Value.BeforeFinalizeAsync(appContext, appServiceMetadata, cancellationToken).PreserveThreadContext();
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
            ICollection<IExport<IFeatureLifecycleBehavior, AppServiceMetadata>> behaviors,
            IAppContext appContext,
            FeatureManagerMetadata appServiceMetadata,
            CancellationToken cancellationToken)
        {
            foreach (var behavior in behaviors)
            {
                await behavior.Value.AfterFinalizeAsync(appContext, appServiceMetadata, cancellationToken).PreserveThreadContext();
            }
        }

        /// <summary>
        /// Sorts the feature manager factories.
        /// </summary>
        /// <param name="featureManagerFactories">The feature manager factories.</param>
        /// <returns>
        /// The sorted feature manager factories.
        /// </returns>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Reviewed. Suppression is OK here.")]
        protected virtual ICollection<IExportFactory<IFeatureManager, FeatureManagerMetadata>> SortEnabledFeatureManagerFactories(ICollection<IExportFactory<IFeatureManager, FeatureManagerMetadata>> featureManagerFactories)
        {
            var partialOrderedSet = new PartialOrderedSet<IExportFactory<IFeatureManager, FeatureManagerMetadata>>(featureManagerFactories, this.CompareFeatureManagers);
            return partialOrderedSet.ToList();
        }

        /// <summary>
        /// Calculates the feature information based on the <see cref="FeatureManagerMetadata"/>
        /// if not explicitely provided.
        /// </summary>
        /// <param name="featureManagerMetadata">The feature manager metadata.</param>
        /// <returns>
        /// The calculated feature information.
        /// </returns>
        protected virtual FeatureInfo ComputeDefaultFeatureInfo(FeatureManagerMetadata featureManagerMetadata)
        {
            return FeatureInfo.FromMetadata(featureManagerMetadata);
        }

        /// <summary>
        /// Compares two feature managers reagarding to their provessing priority.
        /// </summary>
        /// <param name="fm1">The first fm.</param>
        /// <param name="fm2">The second fm.</param>
        /// <returns>
        /// <c>null</c> if the two are not comparable, -1 if fm1 must be processed before fm2, 1 if fm2 must be processed before fm1.
        /// </returns>
        private int? CompareFeatureManagers(IExportFactory<IFeatureManager, FeatureManagerMetadata> fm1, IExportFactory<IFeatureManager, FeatureManagerMetadata> fm2)
        {
            var fm1Info = fm1.Metadata.FeatureInfo;
            var fm2Info = fm2.Metadata.FeatureInfo;
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

        /// <summary>
        /// Ensures that all feature managers have the FeatureInfo property set.
        /// </summary>
        /// <param name="featureManagerFactories">The feature manager factories.</param>
        private void EnsureMetadataHasFeatureInfo(ICollection<IExportFactory<IFeatureManager, FeatureManagerMetadata>> featureManagerFactories)
        {
            if (featureManagerFactories == null)
            {
                return;
            }

            foreach (var fmFactory in featureManagerFactories)
            {
                if (fmFactory.Metadata.FeatureInfo == null)
                {
                    fmFactory.Metadata.FeatureInfo = this.ComputeDefaultFeatureInfo(fmFactory.Metadata);
                }
            }
        }
    }
}