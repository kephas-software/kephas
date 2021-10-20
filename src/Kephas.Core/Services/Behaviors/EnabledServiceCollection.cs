// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnabledServiceCollection.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services.Behaviors
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Service for filtering enabled services collections.
    /// </summary>
    /// <typeparam name="TContract">The contract type.</typeparam>
    [OverridePriority(Priority.Low)]
    public class EnabledServiceCollection<TContract> : EnabledServiceCollectionBase<TContract, AppServiceMetadata>, IEnabledServiceCollection<TContract>
        where TContract : class
    {
        private readonly ICollection<Lazy<TContract, AppServiceMetadata>> services;

        /// <summary>
        /// Initializes a new instance of the <see cref="EnabledServiceCollection{TContract}"/> class.
        /// </summary>
        /// <param name="services">The services to filter.</param>
        /// <param name="contextFactory">The context factory.</param>
        /// <param name="behaviorFactories">The behavior factories.</param>
        public EnabledServiceCollection(
            ICollection<Lazy<TContract, AppServiceMetadata>> services,
            IContextFactory contextFactory,
            ICollection<Lazy<IEnabledServiceBehaviorRule, ServiceBehaviorRuleMetadata>>? behaviorFactories = null)
            : base(contextFactory, behaviorFactories)
        {
            this.services = services;
        }

        /// <summary>Returns an enumerator that iterates through the collection.</summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        /// <footer><a href="https://docs.microsoft.com/en-us/dotnet/api/System.Collections.Generic.IEnumerable-1.GetEnumerator?view=netcore-5.0">`IEnumerable.GetEnumerator` on docs.microsoft.com</a></footer>
        public IEnumerator<TContract> GetEnumerator()
        {
            return this.services
                .Where(s => this.IsServiceEnabled(this.CreateServiceBehaviorContext(() => s.Value, s.Metadata)))
                .Select(s => s.Value)
                .GetEnumerator();
        }

        /// <summary>Returns an enumerator that iterates through a collection.</summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        /// <footer><a href="https://docs.microsoft.com/en-us/dotnet/api/System.Collections.IEnumerable.GetEnumerator?view=netcore-5.0">`IEnumerable.GetEnumerator` on docs.microsoft.com</a></footer>
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}