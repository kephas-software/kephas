// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FeatureEnabledServiceBehavior.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Kephas.Application.Configuration;
    using Kephas.Application.Reflection;
    using Kephas.Behaviors;
    using Kephas.Configuration;
    using Kephas.Injection;
    using Kephas.Logging;
    using Kephas.Services.Behaviors;
    using Kephas.Sets;

    /// <summary>
    /// A feature enabled service behavior.
    /// </summary>
    public class FeatureEnabledServiceBehavior : EnabledServiceBehaviorRuleBase<IFeatureManager, FeatureManagerMetadata>
    {
        private readonly IAppRuntime appRuntime;
        private readonly IAppContext appContext;
        private readonly IConfiguration<AppSettings> appConfiguration;
        private readonly PartialOrderedSet<string> featuresOrderedSet;

        /// <summary>
        /// Initializes a new instance of the <see cref="FeatureEnabledServiceBehavior"/> class.
        /// </summary>
        /// <param name="appRuntime">The application runtime.</param>
        /// <param name="appContext">The application context.</param>
        /// <param name="appConfiguration">The provider for <see cref="AppSettings"/>.</param>
        /// <param name="featureFactories">The feature factories.</param>
        public FeatureEnabledServiceBehavior(
            IAppRuntime appRuntime,
            IAppContext appContext,
            IConfiguration<AppSettings> appConfiguration,
            ICollection<Lazy<IFeatureManager, FeatureManagerMetadata>> featureFactories)
        {
            this.appRuntime = appRuntime;
            this.appContext = appContext;
            this.appConfiguration = appConfiguration;
            var featuresDictionary = featureFactories
                .Select(f => FeatureInfo.FromMetadata(f.Metadata))
                .ToDictionary(f => f.Name.ToLower(), f => f);
            this.featuresOrderedSet = this.GetFeaturesOrderedSet(featuresDictionary);
        }

        /// <summary>Gets the behavior value.</summary>
        /// <param name="context">The context.</param>
        /// <returns>The behavior value.</returns>
        public override IBehaviorValue<bool> GetValue(IServiceBehaviorContext<IFeatureManager, FeatureManagerMetadata> context)
        {
            var featureInfo = context.Metadata?.FeatureInfo;
            if (featureInfo == null)
            {
                // TODO localization
                throw new InvalidOperationException($"Cannot identify the feature info from metadata {context.Metadata}.");
            }

            // if required, enable.
            if (featureInfo.IsRequired)
            {
                return BehaviorValue.True;
            }

            var appSettings = this.appConfiguration.GetSettings(this.appContext);
            if (appSettings == null)
            {
                // TODO localization
                this.Logger.Warn("Cannot identify the application information for '{app}', therefore feature '{feature}' will be enabled.", this.appRuntime.GetAppId(), featureInfo.Name);
                return BehaviorValue.True;
            }

            // if the feature targets a specific app, enable it.
            var targetApps = featureInfo.TargetApps ?? Array.Empty<string>();
            var appId = this.appRuntime.GetAppId();
            if (targetApps.Contains(appId, StringComparer.OrdinalIgnoreCase))
            {
                // TODO localization
                this.Logger.Info("Enabling feature '{feature}' for '{app}', as it targets the app.", featureInfo.Name, this.appRuntime.GetAppId());
                return BehaviorValue.True;
            }

            // if enabled or a dependency, enable.
            var enabledFeatures = appSettings.EnabledFeatures?.Select(f => f.ToLower()).ToArray() ?? Array.Empty<string>();
            var featureKey = featureInfo.Name.ToLower();
            var enabledFeature =
                enabledFeatures.FirstOrDefault(f => (this.featuresOrderedSet.Compare(f, featureKey) ?? -1) >= 0);
            if (enabledFeature != null)
            {
                // TODO localization
                this.Logger.Info($"Enabling feature '{{feature}}' for '{{app}}', as it is required by the '{{enabledFeature}}'.", featureInfo.Name, this.appRuntime.GetAppId(), enabledFeature);
                return BehaviorValue.True;
            }

            return BehaviorValue.False;
        }

        /// <summary>
        /// Gets the features ordered set.
        /// </summary>
        /// <param name="features">The features' dictionary.</param>
        /// <returns>
        /// The features graph.
        /// </returns>
        private PartialOrderedSet<string> GetFeaturesOrderedSet(IDictionary<string, FeatureInfo> features)
        {
            var graph = new PartialOrderedSet<string>(
                features.Select(f => f.Key),
                (featureName1, featureName2) =>
                    {
                        if (featureName1 == featureName2)
                        {
                            return 0;
                        }

                        var feature2 = features[featureName2];
                        if (feature2.Dependencies.Any(
                            d => string.Compare(d, featureName1, StringComparison.InvariantCultureIgnoreCase) == 0))
                        {
                            return -1;
                        }

                        var feature1 = features[featureName1];
                        if (feature1.Dependencies.Any(
                            d => string.Compare(d, featureName2, StringComparison.InvariantCultureIgnoreCase) == 0))
                        {
                            return 1;
                        }

                        return null;
                    });

            return graph;
        }
    }
}