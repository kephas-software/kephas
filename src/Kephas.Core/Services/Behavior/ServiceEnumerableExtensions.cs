﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceEnumerableExtensions.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Extension methods for <see cref="IEnumerable{T}"/> regarding services and service behaviors.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services.Behavior
{
    using System.Collections.Generic;
    using System.Linq;

    using Kephas.Application;
    using Kephas.Composition;
    using Kephas.Diagnostics.Contracts;

    /// <summary>
    /// Extension methods for <see cref="IEnumerable{T}"/> regarding services and service behaviors.
    /// </summary>
    public static class ServiceEnumerableExtensions
    {
        /// <summary>
        /// Enumerates the enabled services from the provided services collection.
        /// </summary>
        /// <typeparam name="TService">Type of the service.</typeparam>
        /// <param name="services">The services.</param>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="appContext">Context for the application (optional).</param>
        /// <returns>
        /// An enumeration of enabled services.
        /// </returns>
        public static IEnumerable<TService> WhereEnabled<TService>(this IEnumerable<TService> services, IAmbientServices ambientServices, IAppContext appContext = null)
            where TService : class
        {
            Requires.NotNull(services, nameof(services));
            Requires.NotNull(ambientServices, nameof(ambientServices));

            var enabledRules = GetEnabledServiceBehaviorRules<TService>(ambientServices.CompositionContainer);
            return services.Where(service => IsServiceEnabled(new ServiceBehaviorContext<TService>(service, ambientServices, appContext), enabledRules));
        }

        /// <summary>
        /// Enumerates the enabled services from the provided services collection.
        /// </summary>
        /// <typeparam name="TService">Type of the service.</typeparam>
        /// <param name="serviceExports">The services.</param>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="appContext">Context for the application (optional).</param>
        /// <returns>
        /// An enumerator that allows foreach to be used to process where enabled in this collection.
        /// </returns>
        public static IEnumerable<IExport<TService>> WhereEnabled<TService>(this IEnumerable<IExport<TService>> serviceExports, IAmbientServices ambientServices, IAppContext appContext = null)
            where TService : class
        {
            Requires.NotNull(serviceExports, nameof(serviceExports));
            Requires.NotNull(ambientServices, nameof(ambientServices));

            var enabledRules = GetEnabledServiceBehaviorRules<TService>(ambientServices.CompositionContainer);
            return serviceExports.Where(export => IsServiceEnabled(new ServiceBehaviorContext<TService>(export.Value, ambientServices, appContext), enabledRules));
        }

        /// <summary>
        /// Enumerates the enabled services from the provided services collection.
        /// </summary>
        /// <typeparam name="TService">Type of the service.</typeparam>
        /// <typeparam name="TMetadata">Type of the service metadata.</typeparam>
        /// <param name="serviceExports">The services.</param>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="appContext">Context for the application (optional).</param>
        /// <returns>
        /// An enumerator that allows foreach to be used to process where enabled in this collection.
        /// </returns>
        public static IEnumerable<IExport<TService, TMetadata>> WhereEnabled<TService, TMetadata>(this IEnumerable<IExport<TService, TMetadata>> serviceExports, IAmbientServices ambientServices, IAppContext appContext = null)
            where TService : class
        {
            Requires.NotNull(serviceExports, nameof(serviceExports));
            Requires.NotNull(ambientServices, nameof(ambientServices));

            var enabledRules = GetEnabledServiceBehaviorRules<TService>(ambientServices.CompositionContainer);
            return serviceExports.Where(export => IsServiceEnabled(new ServiceBehaviorContext<TService>(export.Value, ambientServices, appContext), enabledRules));
        }

        /// <summary>
        /// Queries if a service is enabled.
        /// </summary>
        /// <typeparam name="TService">Type of the service.</typeparam>
        /// <param name="serviceContext">Context for the service.</param>
        /// <param name="enabledRules">The enabled rules.</param>
        /// <returns>
        /// <c>true</c> if the service is enabled, <c>false</c> if not.
        /// </returns>
        private static bool IsServiceEnabled<TService>(IServiceBehaviorContext<TService> serviceContext, IList<IEnabledServiceBehaviorRule<TService>> enabledRules)
        {
            var isEnabled = true;
            foreach (var rule in enabledRules.Where(r => r.CanApply(serviceContext)))
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
        private static IList<IEnabledServiceBehaviorRule<TService>> GetEnabledServiceBehaviorRules<TService>(ICompositionContext compositionContext)
        {
            var enabledRules =
                compositionContext.GetExports<IEnabledServiceBehaviorRule<TService>>()
                    .OrderBy(r => r.ProcessingPriority)
                    .ToList();
            return enabledRules;
        }
    }
}