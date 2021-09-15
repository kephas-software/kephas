// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultServiceBehaviorProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the default service behavior provider class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Injection;

namespace Kephas.Services.Behaviors
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Services.Behaviors.Composition;

    /// <summary>
    /// A default service behavior provider.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class DefaultServiceBehaviorProvider : IServiceBehaviorProvider
    {
        /// <summary>
        /// The composition context.
        /// </summary>
        private readonly IInjector injector;

        /// <summary>
        /// The behavior factories.
        /// </summary>
        private readonly ICollection<IExportFactory<IEnabledServiceBehaviorRule, ServiceBehaviorRuleMetadata>> behaviorFactories;

        /// <summary>
        /// The enabled rules.
        /// </summary>
        private readonly ConcurrentDictionary<Type, IList<IEnabledServiceBehaviorRule>> enabledRules = new ConcurrentDictionary<Type, IList<IEnabledServiceBehaviorRule>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultServiceBehaviorProvider"/> class.
        /// </summary>
        /// <param name="injector">The composition context.</param>
        /// <param name="behaviorFactories">The behavior factories.</param>
        public DefaultServiceBehaviorProvider(
            IInjector injector,
            ICollection<IExportFactory<IEnabledServiceBehaviorRule, ServiceBehaviorRuleMetadata>>? behaviorFactories = null)
        {
            this.injector = injector;
            Requires.NotNull(injector, nameof(injector));

            this.behaviorFactories = behaviorFactories?.Order().ToList()
                                     ?? new List<IExportFactory<IEnabledServiceBehaviorRule, ServiceBehaviorRuleMetadata>>();
        }

        /// <summary>
        /// Filters the enabled services from the provided services collection.
        /// </summary>
        /// <typeparam name="TService">Type of the service.</typeparam>
        /// <param name="services">The services.</param>
        /// <param name="context">Context for the enabled check (optional).</param>
        /// <returns>
        /// An enumeration of enabled services.
        /// </returns>
        public IEnumerable<TService> WhereEnabled<TService>(IEnumerable<TService> services, IContext? context = null)
            where TService : class
        {
            Requires.NotNull(services, nameof(services));

            var rules = this.GetEnabledServiceBehaviorRules<TService>();
            return services.Where(service => this.IsServiceEnabled(new ServiceBehaviorContext<TService>(this.injector, service, context), rules));
        }

        /// <summary>
        /// Filters the enabled services from the provided services collection.
        /// </summary>
        /// <typeparam name="TService">Type of the service.</typeparam>
        /// <param name="serviceFactories">The service export factories.</param>
        /// <param name="context">Context for the enabled check (optional).</param>
        /// <returns>
        /// An enumerator that allows foreach to be used to process where enabled in this collection.
        /// </returns>
        public IEnumerable<IExportFactory<TService>> WhereEnabled<TService>(IEnumerable<IExportFactory<TService>> serviceFactories, IContext? context = null)
            where TService : class
        {
            Requires.NotNull(serviceFactories, nameof(serviceFactories));

            var rules = this.GetEnabledServiceBehaviorRules<TService>();
            return serviceFactories.Where(export => this.IsServiceEnabled(new ServiceBehaviorContext<TService>(this.injector, export, context), rules));
        }

        /// <summary>
        /// Filters the enabled services from the provided services collection.
        /// </summary>
        /// <typeparam name="TService">Type of the service.</typeparam>
        /// <typeparam name="TMetadata">Type of the service metadata.</typeparam>
        /// <param name="serviceFactories">The service export factories.</param>
        /// <param name="context">Context for the enabled check (optional).</param>
        /// <returns>
        /// An enumerator that allows foreach to be used to process where enabled in this collection.
        /// </returns>
        public IEnumerable<IExportFactory<TService, TMetadata>> WhereEnabled<TService, TMetadata>(IEnumerable<IExportFactory<TService, TMetadata>> serviceFactories, IContext? context = null)
            where TService : class
        {
            Requires.NotNull(serviceFactories, nameof(serviceFactories));

            var rules = this.GetEnabledServiceBehaviorRules<TService>();
            return serviceFactories.Where(export => this.IsServiceEnabled(new ServiceBehaviorContext<TService>(this.injector, export, context), rules));
        }

        /// <summary>
        /// Queries if a service is enabled.
        /// </summary>
        /// <typeparam name="TService">Type of the service.</typeparam>
        /// <param name="serviceContext">Context for the service.</param>
        /// <param name="rules">The enabled rules.</param>
        /// <returns>
        /// <c>true</c> if the service is enabled, <c>false</c> if not.
        /// </returns>
        private bool IsServiceEnabled<TService>(IServiceBehaviorContext<TService> serviceContext, IList<IEnabledServiceBehaviorRule> rules)
            where TService : class
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
        /// <typeparam name="TService">Type of the service.</typeparam>
        /// <returns>
        /// The enabled service behavior rules.
        /// </returns>
        private IList<IEnabledServiceBehaviorRule> GetEnabledServiceBehaviorRules<TService>()
        {
            var rules = this.enabledRules.GetOrAdd(typeof(TService), _ => this.ComputeEnabledServiceBehaviorRules<TService>());
            return rules;
        }

        /// <summary>
        /// Calculates the enabled service behavior rules.
        /// </summary>
        /// <typeparam name="TService">Type of the service.</typeparam>
        /// <returns>
        /// The calculated enabled service behavior rules.
        /// </returns>
        private IList<IEnabledServiceBehaviorRule> ComputeEnabledServiceBehaviorRules<TService>()
        {
            return this.behaviorFactories
                .Where(f => f.Metadata.ServiceContractType == typeof(TService))
                .Select(f => f.CreateExportedValue())
                .ToList();
        }
    }
}