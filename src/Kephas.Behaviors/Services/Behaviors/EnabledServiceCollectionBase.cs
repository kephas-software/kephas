// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnabledServiceCollectionBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services.Behaviors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Base abstract class for filtering enabled services collections.
    /// </summary>
    /// <typeparam name="TContract">The contract type.</typeparam>
    /// <typeparam name="TMetadata">The metadata type.</typeparam>
    public abstract class EnabledServiceCollectionBase<TContract, TMetadata>
        where TContract : class
    {
        private readonly IInjectableFactory injectableFactory;
        private readonly ICollection<IEnabledServiceBehaviorRule> behaviorFactories;

        /// <summary>
        /// Initializes a new instance of the <see cref="EnabledServiceCollectionBase{TContract, TMetadata}"/> class.
        /// </summary>
        /// <param name="injectableFactory">The injectable factory.</param>
        /// <param name="behaviorFactories">The behavior factories.</param>
        protected EnabledServiceCollectionBase(
            IInjectableFactory injectableFactory,
            ICollection<Lazy<IEnabledServiceBehaviorRule, ServiceBehaviorRuleMetadata>>? behaviorFactories = null)
        {
            this.injectableFactory = injectableFactory ?? throw new ArgumentNullException(nameof(injectableFactory));
            this.behaviorFactories = this.ComputeEnabledServiceBehaviorRules(behaviorFactories);
        }

        /// <summary>
        /// Queries if a service is enabled.
        /// </summary>
        /// <param name="serviceContext">Context for the service.</param>
        /// <returns>
        /// <c>true</c> if the service is enabled, <c>false</c> if not.
        /// </returns>
        protected bool IsServiceEnabled(IServiceBehaviorContext<TContract, TMetadata> serviceContext)
        {
            var isEnabled = true;
            foreach (var rule in this.behaviorFactories.Where(r => r.CanApply(serviceContext)))
            {
                if (!rule.GetValue(serviceContext).Value)
                {
                    isEnabled = false;
                    break;
                }

                if (rule.IsEndRule)
                {
                    break;
                }
            }

            return isEnabled;
        }

        /// <summary>
        /// Creates the service behavior context.
        /// </summary>
        /// <param name="serviceFactory">The service factory.</param>
        /// <param name="metadata">The metadata.</param>
        /// <returns>An instance of <see cref="IServiceBehaviorContext{TContract, TMetadata}"/>.</returns>
        protected IServiceBehaviorContext<TContract, TMetadata> CreateServiceBehaviorContext(
            Func<TContract> serviceFactory,
            TMetadata metadata) =>
            this.injectableFactory.Create<ServiceBehaviorContext<TContract, TMetadata>>(serviceFactory, metadata);

        private IList<IEnabledServiceBehaviorRule> ComputeEnabledServiceBehaviorRules(ICollection<Lazy<IEnabledServiceBehaviorRule, ServiceBehaviorRuleMetadata>>? behaviorFactories)
        {
            return behaviorFactories?
                       .Where(f => f.Metadata.ContractType == typeof(TContract) &&
                                   (f.Metadata.MetadataType == null || f.Metadata.MetadataType.IsAssignableFrom(typeof(TMetadata))))
                       .Order()
                       .Select(f => f.Value)
                       .ToList()
                   ?? new List<IEnabledServiceBehaviorRule>();
        }
    }
}