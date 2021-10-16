// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IServiceBehaviorProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IServiceBehaviorProvider interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services.Behaviors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Kephas.Injection;

    /// <summary>
    /// Interface for service behavior provider.
    /// </summary>
    [SingletonAppServiceContract]
    public interface IServiceBehaviorProvider
    {

        /// <summary>
        /// Gets the enabled service behavior rules.
        /// </summary>
        /// <typeparam name="TContract">Type of the service contract.</typeparam>
        /// <returns>
        /// The enabled service behavior rules.
        /// </returns>
        protected IList<IEnabledServiceBehaviorRule> GetEnabledServiceBehaviorRules<TContract>();

        /// <summary>
        /// Creates the service behavior context.
        /// </summary>
        /// <typeparam name="TContract">The service contract type.</typeparam>
        /// <param name="serviceFactory">The service factory.</param>
        /// <param name="metadata">The metadata.</param>
        /// <param name="context">The context.</param>
        /// <returns>The service behavior context.</returns>
        protected IServiceBehaviorContext<TContract> CreateServiceBehaviorContext<TContract>(
            Func<TContract> serviceFactory,
            object? metadata,
            IContext? context)
            where TContract : class;

        /// <summary>
        /// Filters the enabled services from the provided services collection.
        /// </summary>
        /// <typeparam name="TContract">Type of the service contract.</typeparam>
        /// <param name="services">The services.</param>
        /// <param name="context">Context for the enabled check (optional).</param>
        /// <returns>
        /// An enumeration of enabled services.
        /// </returns>
        IEnumerable<TContract> WhereEnabled<TContract>(
            IEnumerable<TContract> services,
            IContext? context = null)
            where TContract : class
        {
            services = services ?? throw new ArgumentNullException(nameof(services));

            var rules = this.GetEnabledServiceBehaviorRules<TContract>();
            return services.Where(service => this.IsServiceEnabled(this.CreateServiceBehaviorContext<TContract>(() => service, null, context), rules));
        }

        /// <summary>
        /// Filters the enabled services from the provided services collection.
        /// </summary>
        /// <typeparam name="TContract">Type of the service contract.</typeparam>
        /// <param name="serviceFactories">The service export factories.</param>
        /// <param name="context">Context for the enabled check (optional).</param>
        /// <returns>
        /// An enumeration of enabled services.
        /// </returns>
        IEnumerable<Lazy<TContract>> WhereEnabled<TContract>(
            IEnumerable<Lazy<TContract>> serviceFactories,
            IContext? context = null)
            where TContract : class
        {
            serviceFactories = serviceFactories ?? throw new ArgumentNullException(nameof(serviceFactories));

            var rules = this.GetEnabledServiceBehaviorRules<TContract>();
            return serviceFactories.Where(export => this.IsServiceEnabled(this.CreateServiceBehaviorContext(() => export.Value, null, context), rules));
        }

        /// <summary>
        /// Filters the enabled services from the provided services collection.
        /// </summary>
        /// <typeparam name="TContract">Type of the service contract.</typeparam>
        /// <param name="serviceFactories">The service export factories.</param>
        /// <param name="context">Context for the enabled check (optional).</param>
        /// <returns>
        /// An enumeration of enabled services.
        /// </returns>
        IEnumerable<IExportFactory<TContract>> WhereEnabled<TContract>(
            IEnumerable<IExportFactory<TContract>> serviceFactories,
            IContext? context = null)
            where TContract : class
        {
            serviceFactories = serviceFactories ?? throw new ArgumentNullException(nameof(serviceFactories));

            var rules = this.GetEnabledServiceBehaviorRules<TContract>();
            return serviceFactories.Where(export => this.IsServiceEnabled(this.CreateServiceBehaviorContext(export.CreateExportedValue, null, context), rules));
        }

        /// <summary>
        /// Enumerates the enabled services from the provided services collection.
        /// </summary>
        /// <typeparam name="TContract">Type of the service contract.</typeparam>
        /// <typeparam name="TMetadata">Type of the service metadata.</typeparam>
        /// <param name="serviceFactories">The service export factories.</param>
        /// <param name="context">Context for the enabled check (optional).</param>
        /// <returns>
        /// An enumerator that allows foreach to be used to process where enabled in this collection.
        /// </returns>
        IEnumerable<Lazy<TContract, TMetadata>> WhereEnabled<TContract, TMetadata>(
            IEnumerable<Lazy<TContract, TMetadata>> serviceFactories,
            IContext? context = null)
            where TContract : class
        {
            serviceFactories = serviceFactories ?? throw new ArgumentNullException(nameof(serviceFactories));

            var rules = this.GetEnabledServiceBehaviorRules<TContract>();
            return serviceFactories.Where(export => this.IsServiceEnabled(this.CreateServiceBehaviorContext(() => export.Value, export.Metadata, context), rules));
        }

        /// <summary>
        /// Enumerates the enabled services from the provided services collection.
        /// </summary>
        /// <typeparam name="TContract">Type of the service contract.</typeparam>
        /// <typeparam name="TMetadata">Type of the service metadata.</typeparam>
        /// <param name="serviceFactories">The service export factories.</param>
        /// <param name="context">Context for the enabled check (optional).</param>
        /// <returns>
        /// An enumerator that allows foreach to be used to process where enabled in this collection.
        /// </returns>
        IEnumerable<IExportFactory<TContract, TMetadata>> WhereEnabled<TContract, TMetadata>(
            IEnumerable<IExportFactory<TContract, TMetadata>> serviceFactories,
            IContext? context = null)
            where TContract : class
        {
            serviceFactories = serviceFactories ?? throw new ArgumentNullException(nameof(serviceFactories));

            var rules = this.GetEnabledServiceBehaviorRules<TContract>();
            return serviceFactories.Where(export => this.IsServiceEnabled(this.CreateServiceBehaviorContext(export.CreateExportedValue, export.Metadata, context), rules));
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
    }
}