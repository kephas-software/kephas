// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceEnumerableExtensions.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Extension methods for <see cref="IEnumerable{T}"/> regarding services and service behaviors.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services.Behavior
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;

    using Kephas.Composition;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Services.Behavior.Composition;

    /// <summary>
    /// Extension methods for <see cref="IEnumerable{T}"/> regarding services and service behaviors.
    /// </summary>
    public static class ServiceEnumerableExtensions
    {
        /// <summary>
        /// The enabled rules.
        /// </summary>
        private static readonly ConcurrentDictionary<Type, IList<IEnabledServiceBehaviorRule>> EnabledRules = new ConcurrentDictionary<Type, IList<IEnabledServiceBehaviorRule>>();

        /// <summary>
        /// Enumerates the enabled services from the provided services collection.
        /// </summary>
        /// <typeparam name="TService">Type of the service.</typeparam>
        /// <param name="services">The services.</param>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="context">Context for the enabled check (optional).</param>
        /// <returns>
        /// An enumeration of enabled services.
        /// </returns>
        public static IEnumerable<TService> WhereEnabled<TService>(this IEnumerable<TService> services, IAmbientServices ambientServices, IContext context = null)
            where TService : class
        {
            Requires.NotNull(services, nameof(services));
            Requires.NotNull(ambientServices, nameof(ambientServices));

            var rules = GetEnabledServiceBehaviorRules<TService>(ambientServices.CompositionContainer);
            return services.Where(service => IsServiceEnabled(new ServiceBehaviorContext<TService>(ambientServices, service, context), rules));
        }

        /// <summary>
        /// Enumerates the enabled services from the provided services collection.
        /// </summary>
        /// <typeparam name="TService">Type of the service.</typeparam>
        /// <param name="serviceFactories">The service export factories.</param>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="context">Context for the enabled check (optional).</param>
        /// <returns>
        /// An enumerator that allows foreach to be used to process where enabled in this collection.
        /// </returns>
        public static IEnumerable<IExportFactory<TService>> WhereEnabled<TService>(this IEnumerable<IExportFactory<TService>> serviceFactories, IAmbientServices ambientServices, IContext context = null)
            where TService : class
        {
            Requires.NotNull(serviceFactories, nameof(serviceFactories));
            Requires.NotNull(ambientServices, nameof(ambientServices));

            var rules = GetEnabledServiceBehaviorRules<TService>(ambientServices.CompositionContainer);
            return serviceFactories.Where(export => IsServiceEnabled(new ServiceBehaviorContext<TService>(ambientServices, export, context), rules));
        }

        /// <summary>
        /// Enumerates the enabled services from the provided services collection.
        /// </summary>
        /// <typeparam name="TService">Type of the service.</typeparam>
        /// <typeparam name="TMetadata">Type of the service metadata.</typeparam>
        /// <param name="serviceFactories">The service export factories.</param>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="context">Context for the enabled check (optional).</param>
        /// <returns>
        /// An enumerator that allows foreach to be used to process where enabled in this collection.
        /// </returns>
        public static IEnumerable<IExportFactory<TService, TMetadata>> WhereEnabled<TService, TMetadata>(this IEnumerable<IExportFactory<TService, TMetadata>> serviceFactories, IAmbientServices ambientServices, IContext context = null)
            where TService : class
        {
            Requires.NotNull(serviceFactories, nameof(serviceFactories));
            Requires.NotNull(ambientServices, nameof(ambientServices));

            var rules = GetEnabledServiceBehaviorRules<TService>(ambientServices.CompositionContainer);
            return serviceFactories.Where(export => IsServiceEnabled(new ServiceBehaviorContext<TService>(ambientServices, export, context), rules));
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
        private static bool IsServiceEnabled<TService>(IServiceBehaviorContext<TService> serviceContext, IList<IEnabledServiceBehaviorRule> rules)
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
        /// <param name="compositionContext">The composition context where the behaviors for the services
        ///                                  can be retrieved.</param>
        /// <returns>
        /// The enabled service behavior rules.
        /// </returns>
        private static IList<IEnabledServiceBehaviorRule> GetEnabledServiceBehaviorRules<TService>(ICompositionContext compositionContext)
        {
            var rules = EnabledRules.GetOrAdd(
                typeof(TService),
                _ =>
                    {
                        return compositionContext
                            .GetExportFactories<IEnabledServiceBehaviorRule, ServiceBehaviorRuleMetadata>()
                            .Where(f => f.Metadata.ServiceContractType == typeof(TService))
                            .OrderBy(f => f.Metadata.ProcessingPriority)
                            .Select(f => f.CreateExportedValue())
                            .ToList();
                    });

            return rules;
        }
    }
}