﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultDataBehaviorProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the default data behavior provider class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Services;

namespace Kephas.Data.Behaviors
{
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Kephas.Services;

    /// <summary>
    /// A default data behavior provider.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class DefaultDataBehaviorProvider : IDataBehaviorProvider
    {
        /// <summary>
        /// The data behavior factories.
        /// </summary>
        private readonly ICollection<IExportFactory<IDataBehavior, DataBehaviorMetadata>> dataBehaviorFactories;

        /// <summary>
        /// The behaviors cache.
        /// </summary>
        private readonly ConcurrentDictionary<Type, ConcurrentDictionary<Type, IEnumerable>> behaviorsCache =
            new ConcurrentDictionary<Type, ConcurrentDictionary<Type, IEnumerable>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultDataBehaviorProvider"/> class.
        /// </summary>
        /// <param name="dataBehaviorFactories">The data behavior factories.</param>
        public DefaultDataBehaviorProvider(ICollection<IExportFactory<IDataBehavior, DataBehaviorMetadata>> dataBehaviorFactories)
        {
            dataBehaviorFactories = dataBehaviorFactories ?? throw new System.ArgumentNullException(nameof(dataBehaviorFactories));

            this.dataBehaviorFactories = dataBehaviorFactories;
        }

        /// <summary>
        /// Gets the data behaviors of type <typeparamref name="TBehavior"/>
        ///  for the provided type.
        /// </summary>
        /// <typeparam name="TBehavior">Type of the behavior.</typeparam>
        /// <param name="type">The type.</param>
        /// <returns>
        /// An enumeration of behaviors matching the provided type.
        /// </returns>
        public IEnumerable<TBehavior> GetDataBehaviors<TBehavior>(Type type)
        {
            type = type ?? throw new ArgumentNullException(nameof(type));

            var typeBehaviors = this.behaviorsCache.GetOrAdd(type, _ => new ConcurrentDictionary<Type, IEnumerable>());
            var behaviors = typeBehaviors.GetOrAdd(typeof(TBehavior), _ => this.ComputeDataBehaviors<TBehavior>(type) as IEnumerable);
            return (IEnumerable<TBehavior>)behaviors;
        }

        /// <summary>
        /// Calculates the data behaviors.
        /// </summary>
        /// <typeparam name="TBehavior">Type of the behavior.</typeparam>
        /// <param name="type">The type.</param>
        /// <returns>
        /// An enumeration of behaviors of the specified type.
        /// </returns>
        private IEnumerable<TBehavior> ComputeDataBehaviors<TBehavior>(Type type)
        {
            var behaviorType = typeof(TBehavior);
            var behaviors = (from f in this.dataBehaviorFactories
                                 let entityType = f.Metadata.EntityType
                                 let serviceType = f.Metadata.ServiceType
                                 where
                                     entityType.IsAssignableFrom(type)
                                     && behaviorType.IsAssignableFrom(serviceType)
                                 orderby f.Metadata.ProcessingPriority
                                 select f.CreateExportedValue())
                             .Cast<TBehavior>()
                             .ToList();
            return behaviors;
        }
    }
}