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
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;

    using Kephas.Application;
    using Kephas.Composition;

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
        /// <param name="compositionContext">The composition context where the behaviors for the services
        ///                                  can be retrieved.</param>
        /// <param name="appContext">Context for the application.</param>
        /// <returns>
        /// An enumerator that allows foreach to be used to process where enabled in this collection.
        /// </returns>
        public static IEnumerable<TService> WhereEnabled<TService>(this IEnumerable<TService> services, ICompositionContext compositionContext, IAppContext appContext = null)
        {
            Contract.Requires(services != null);
            Contract.Requires(compositionContext != null);

            var enabledRules = compositionContext.GetExports<IEnabledServiceBehaviorRule<TService>>().OrderBy(r => r.ProcessingPriority).ToList();
            foreach (var service in services)
            {
                var serviceContext = new ServiceBehaviorContext<TService>(service);
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

                if (isEnabled)
                {
                    yield return service;
                }
            }
        } 
    }
}