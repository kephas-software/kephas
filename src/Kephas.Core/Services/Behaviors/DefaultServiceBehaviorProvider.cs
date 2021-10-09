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

    using Kephas.Diagnostics.Contracts;
    using Kephas.Injection;

    /// <summary>
    /// A default service behavior provider.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class DefaultServiceBehaviorProvider : IServiceBehaviorProvider
    {
        private readonly IInjector injector;
        private readonly ICollection<IExportFactory<IEnabledServiceBehaviorRule, ServiceBehaviorRuleMetadata>> behaviorFactories;
        private readonly ConcurrentDictionary<Type, IList<IEnabledServiceBehaviorRule>> enabledRules = new ConcurrentDictionary<Type, IList<IEnabledServiceBehaviorRule>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultServiceBehaviorProvider"/> class.
        /// </summary>
        /// <param name="injector">The injector.</param>
        /// <param name="behaviorFactories">The behavior factories.</param>
        public DefaultServiceBehaviorProvider(
            IInjector injector,
            ICollection<IExportFactory<IEnabledServiceBehaviorRule, ServiceBehaviorRuleMetadata>>? behaviorFactories = null)
        {
            this.injector = injector;
            injector = injector ?? throw new ArgumentNullException(nameof(injector));

            this.behaviorFactories = behaviorFactories?.Order().ToList()
                                     ?? new List<IExportFactory<IEnabledServiceBehaviorRule, ServiceBehaviorRuleMetadata>>();
        }

        /// <summary>
        /// Filters the enabled services from the provided services collection.
        /// </summary>
        /// <typeparam name="TContract">Type of the service contract.</typeparam>
        /// <param name="services">The services.</param>
        /// <param name="context">Optional. Context for the enabled check.</param>
        /// <returns>
        /// An enumeration of enabled services.
        /// </returns>
        public IEnumerable<TContract> WhereEnabled<TContract>(IEnumerable<TContract> services, IContext? context = null)
            where TContract : class
        {
            services = services ?? throw new ArgumentNullException(nameof(services));

            var rules = this.GetEnabledServiceBehaviorRules<TContract>();
            return services.Where(service => this.IsServiceEnabled(new ServiceBehaviorContext<TContract>(this.injector, service, context), rules));
        }

        /// <summary>
        /// Filters the enabled services from the provided services collection.
        /// </summary>
        /// <typeparam name="TContract">Type of the service contract.</typeparam>
        /// <param name="serviceFactories">The service export factories.</param>
        /// <param name="context">Optional. Context for the enabled check.</param>
        /// <returns>
        /// An enumerator that allows foreach to be used to process where enabled in this collection.
        /// </returns>
        public IEnumerable<IExportFactory<TContract>> WhereEnabled<TContract>(IEnumerable<IExportFactory<TContract>> serviceFactories, IContext? context = null)
            where TContract : class
        {
            serviceFactories = serviceFactories ?? throw new ArgumentNullException(nameof(serviceFactories));

            var rules = this.GetEnabledServiceBehaviorRules<TContract>();
            return serviceFactories.Where(export => this.IsServiceEnabled(new ServiceBehaviorContext<TContract>(this.injector, export, context), rules));
        }

        /// <summary>
        /// Filters the enabled services from the provided services collection.
        /// </summary>
        /// <typeparam name="TContract">Type of the service contract.</typeparam>
        /// <typeparam name="TMetadata">Type of the service metadata.</typeparam>
        /// <param name="serviceFactories">The service export factories.</param>
        /// <param name="context">Optional. Context for the enabled check.</param>
        /// <returns>
        /// An enumerator that allows foreach to be used to process where enabled in this collection.
        /// </returns>
        public IEnumerable<IExportFactory<TContract, TMetadata>> WhereEnabled<TContract, TMetadata>(IEnumerable<IExportFactory<TContract, TMetadata>> serviceFactories, IContext? context = null)
            where TContract : class
        {
            serviceFactories = serviceFactories ?? throw new ArgumentNullException(nameof(serviceFactories));

            var rules = this.GetEnabledServiceBehaviorRules<TContract>();
            return serviceFactories.Where(export => this.IsServiceEnabled(new ServiceBehaviorContext<TContract>(this.injector, export, context), rules));
        }

        /// <summary>
        /// Queries if a service is enabled.
        /// </summary>
        /// <typeparam name="TContract">Type of the service contract.</typeparam>
        /// <param name="serviceContext">Context for the service.</param>
        /// <param name="rules">The enabled rules.</param>
        /// <returns>
        /// <c>true</c> if the service is enabled, <c>false</c> if not.
        /// </returns>
        private bool IsServiceEnabled<TContract>(IServiceBehaviorContext<TContract> serviceContext, IList<IEnabledServiceBehaviorRule> rules)
            where TContract : class
        {
            var isEnabled = true;
            foreach (var rule in rules.Where(r => r.CanApply(serviceContext)))
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
        /// Gets the enabled service behavior rules.
        /// </summary>
        /// <typeparam name="TContract">Type of the service contract.</typeparam>
        /// <returns>
        /// The enabled service behavior rules.
        /// </returns>
        private IList<IEnabledServiceBehaviorRule> GetEnabledServiceBehaviorRules<TContract>()
        {
            var rules = this.enabledRules.GetOrAdd(typeof(TContract), _ => this.ComputeEnabledServiceBehaviorRules<TContract>());
            return rules;
        }

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
                .Select(f => f.CreateExportedValue())
                .ToList();
        }
    }
}