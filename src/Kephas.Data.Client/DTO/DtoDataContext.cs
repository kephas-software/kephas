// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DtoDataContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the dto data context class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Client.DTO
{
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;

    using Kephas.Data.Behaviors;
    using Kephas.Data.Commands.Factory;
    using Kephas.Data.InMemory;
    using Kephas.Data.Store;
    using Kephas.Injection;
    using Kephas.Injection.AttributedModel;
    using Kephas.Serialization;

    /// <summary>
    /// A data context for DTOs.
    /// </summary>
    [SupportedDataStoreKinds(DataStoreKind.DTO)]
    public class DtoDataContext : InMemoryDataContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DtoDataContext"/> class.
        /// </summary>
        /// <param name="serviceProvider">The injector.</param>
        /// <param name="dataCommandProvider">The data command provider.</param>
        /// <param name="serializationService">The serialization service.</param>
        public DtoDataContext(
            IServiceProvider serviceProvider,
            IDataCommandProvider dataCommandProvider,
            ISerializationService serializationService)
            : base(serviceProvider, dataCommandProvider, new NoneDataBehaviorProvider(), serializationService)
        {
        }

        /// <summary>
        /// A none data behavior provider.
        /// </summary>
        [ExcludeFromInjection]
        internal class NoneDataBehaviorProvider : IDataBehaviorProvider
        {
            /// <summary>
            /// The empty behaviors.
            /// </summary>
            private static readonly ConcurrentDictionary<Type, IEnumerable> EmptyBehaviors = new ConcurrentDictionary<Type, IEnumerable>();

            /// <summary>
            /// Gets the data behaviors of type <typeparamref name="TBehavior" /> for the provided type.
            /// </summary>
            /// <typeparam name="TBehavior">Type of the behavior.</typeparam>
            /// <param name="type">The type.</param>
            /// <returns>
            /// An enumeration of behaviors matching the provided type.
            /// </returns>
            public IEnumerable<TBehavior> GetDataBehaviors<TBehavior>(Type type)
            {
                var behaviors = EmptyBehaviors.GetOrAdd(typeof(TBehavior), t => new TBehavior[0]);
                return (IEnumerable<TBehavior>)behaviors;
            }
        }
    }
}