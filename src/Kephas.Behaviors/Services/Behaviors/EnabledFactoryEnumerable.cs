﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnabledFactoryEnumerable.cs" company="Kephas Software SRL">
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

    using Kephas.Services;

    /// <summary>
    /// Service for filtering enabled service factories collections (with metadata).
    /// </summary>
    /// <typeparam name="TContract">The contract type.</typeparam>
    /// <typeparam name="TMetadata">The metadata type.</typeparam>
    [OverridePriority(Priority.Low)]
    public class EnabledFactoryEnumerable<TContract, TMetadata> : EnabledEnumerableBase<TContract, TMetadata>, IEnabledFactoryEnumerable<TContract, TMetadata>
        where TContract : class
    {
        private readonly IFactoryEnumerable<TContract, TMetadata> services;

        /// <summary>
        /// Initializes a new instance of the <see cref="EnabledFactoryEnumerable{TContract,TMetadata}"/> class.
        /// </summary>
        /// <param name="services">The services to filter.</param>
        /// <param name="injectableFactory">The injectable factory.</param>
        /// <param name="behaviorFactories">The behavior factories.</param>
        public EnabledFactoryEnumerable(
            IFactoryEnumerable<TContract, TMetadata> services,
            IInjectableFactory injectableFactory,
            ILazyEnumerable<IEnabledServiceBehaviorRule, ServiceBehaviorRuleMetadata>? behaviorFactories = null)
            : base(injectableFactory, behaviorFactories)
        {
            this.services = services;
        }

        /// <summary>Returns an enumerator that iterates through the collection.</summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        /// <footer><a href="https://docs.microsoft.com/en-us/dotnet/api/System.Collections.Generic.IEnumerable-1.GetEnumerator?view=netcore-5.0">`IEnumerable.GetEnumerator` on docs.microsoft.com</a></footer>
        public IEnumerator<IExportFactory<TContract, TMetadata>> GetEnumerator()
        {
            return this.services
                .Where(s => this.IsServiceEnabled(this.CreateServiceBehaviorContext(s.CreateExportedValue, s.Metadata)))
                .GetEnumerator();
        }

        /// <summary>Returns an enumerator that iterates through a collection.</summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        /// <footer><a href="https://docs.microsoft.com/en-us/dotnet/api/System.Collections.IEnumerable.GetEnumerator?view=netcore-5.0">`IEnumerable.GetEnumerator` on docs.microsoft.com</a></footer>
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}