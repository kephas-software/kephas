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

    using Kephas.Application.Composition;
    using Kephas.Application.Configuration;
    using Kephas.Application.Reflection;
    using Kephas.Behaviors;
    using Kephas.Composition;
    using Kephas.Configuration;
    using Kephas.Logging;
    using Kephas.Services.Behaviors;
    using Kephas.Sets;

    /// <summary>
    /// A feature enabled service behavior.
    /// </summary>
    public class FeatureEnabledServiceBehavior : EnabledServiceBehaviorRuleBase<IFeatureManager>
    {
        private readonly IAppRuntime appRuntime;
        private readonly IConfiguration<SystemSettings> systemConfiguration;
        private readonly PartialOrderedSet<string> featuresOrderedSet;

        /// <summary>
        /// Initializes a new instance of the <see cref="FeatureEnabledServiceBehavior"/> class.
        /// </summary>
        /// <param name="appRuntime">The application runtime.</param>
        /// <param name="systemConfiguration">The system configuration.</param>
        /// <param name="featureFactories">The feature factories.</param>
        public FeatureEnabledServiceBehavior(
            IAppRuntime appRuntime,
            IConfiguration<SystemSettings> systemConfiguration,
            ICollection<IExportFactory<IFeatureManager, FeatureManagerMetadata>> featureFactories)
        {
            this.appRuntime = appRuntime;
            this.systemConfiguration = systemConfiguration;
            var featuresDictionary = featureFactories
                .Select(f => FeatureInfo.FromMetadata(f.Metadata))
                .ToDictionary(f => f.Name.ToLower(), f => f);
            this.featuresOrderedSet = this.GetFeaturesOrderedSet(featuresDictionary);
        }

        /// <summary>Gets the behavior value.</summary>
        /// <param name="context">The context.</param>
        /// <returns>The behavior value.</returns>
        public override IBehaviorValue<bool> GetValue(IServiceBehaviorContext<IFeatureManager> context)
        {
            var featureInfo = (context.Metadata as FeatureManagerMetadata)?.FeatureInfo;
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

            var appId = this.appRuntime.GetAppId();
            if (this.systemConfiguration.Settings.Instances == null ||
                !this.systemConfiguration.Settings.Instances.TryGetValue(appId, out var appSettings) ||
                appSettings.EnabledFeatures == null)
            {
                // TODO localization
                this.Logger.Warn("Cannot identify the application information in the system settings for {app}, therefore feature '{feature}' will be enabled.", appId, featureInfo.Name);
                return BehaviorValue.True;
            }

            // if enabled or a dependency, enable.
            var enabledFeatures = appSettings.EnabledFeatures.Select(f => f.ToLower()).ToList();
            var featureKey = featureInfo.Name.ToLower();
            return enabledFeatures.Any(f => (this.featuresOrderedSet.Compare(f, featureKey) ?? -1) >= 0)
                ? BehaviorValue.True
                : BehaviorValue.False;
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