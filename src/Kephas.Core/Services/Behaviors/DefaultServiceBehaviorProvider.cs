// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultServiceBehaviorProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the default service behavior provider class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services.Behaviors
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;

    using Kephas.Injection;

    /// <summary>
    /// A default service behavior provider.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class DefaultServiceBehaviorProvider : IServiceBehaviorProvider
    {
        private readonly IInjector injector;
        private readonly ICollection<Lazy<IEnabledServiceBehaviorRule, ServiceBehaviorRuleMetadata>> behaviorFactories;
        private readonly ConcurrentDictionary<Type, IList<IEnabledServiceBehaviorRule>> enabledRules = new ConcurrentDictionary<Type, IList<IEnabledServiceBehaviorRule>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultServiceBehaviorProvider"/> class.
        /// </summary>
        /// <param name="injector">The injector.</param>
        /// <param name="behaviorFactories">The behavior factories.</param>
        public DefaultServiceBehaviorProvider(
            IInjector injector,
            ICollection<Lazy<IEnabledServiceBehaviorRule, ServiceBehaviorRuleMetadata>>? behaviorFactories = null)
        {
            this.injector = injector;
            injector = injector ?? throw new ArgumentNullException(nameof(injector));

            this.behaviorFactories = behaviorFactories?.Order().ToList()
                                     ?? new List<Lazy<IEnabledServiceBehaviorRule, ServiceBehaviorRuleMetadata>>();
        }

        /// <summary>
        /// Gets the enabled service behavior rules.
        /// </summary>
        /// <typeparam name="TContract">Type of the service contract.</typeparam>
        /// <returns>
        /// The enabled service behavior rules.
        /// </returns>
        IList<IEnabledServiceBehaviorRule> IServiceBehaviorProvider.GetEnabledServiceBehaviorRules<TContract>()
        {
            var rules = this.enabledRules.GetOrAdd(typeof(TContract), _ => this.ComputeEnabledServiceBehaviorRules<TContract>());
            return rules;
        }

        /// <summary>
        /// Creates the service behavior context.
        /// </summary>
        /// <typeparam name="TContract">The service contract type.</typeparam>
        /// <param name="serviceFactory">The service factory.</param>
        /// <param name="metadata">The metadata.</param>
        /// <param name="context">The context.</param>
        /// <returns>The service behavior context.</returns>
        IServiceBehaviorContext<TContract> IServiceBehaviorProvider.CreateServiceBehaviorContext<TContract>(
            Func<TContract> serviceFactory,
            object? metadata,
            IContext? context)
            where TContract : class =>
            new ServiceBehaviorContext<TContract>(this.injector, serviceFactory, metadata, context);

        /// <summary>
        /// Calculates the enabled service behavior rules.
        /// </summary>
        /// <typeparam name="TContract">Type of the service contract.</typeparam>
        /// <returns>
        /// The calculated enabled service behavior rules.
        /// </returns>
        private IList<IEnabledServiceBehaviorRule> ComputeEnabledServiceBehaviorRules<TContract>()
        {
            return this.behaviorFactories
                .Where(f => f.Metadata.ContractType == typeof(TContract))
                .Select(f => f.Value)
                .ToList();
        }
    }
}