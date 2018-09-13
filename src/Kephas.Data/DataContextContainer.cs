// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataContextContainer.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the data context collection class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using Kephas.Collections;
    using Kephas.Data.Capabilities;
    using Kephas.Data.Store;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Dynamic;
    using Kephas.Services;

    /// <summary>
    /// Container class for data contexts indexed by contained entity types.
    /// </summary>
    public class DataContextContainer : Expando, IDataContextContainer
    {
        /// <summary>
        /// The data context factory.
        /// </summary>
        private readonly IDataContextFactory dataContextFactory;

        /// <summary>
        /// The data store selector.
        /// </summary>
        private readonly IDataStoreSelector dataStoreSelector;

        /// <summary>
        /// Context for the processing.
        /// </summary>
        private readonly IContext context;

        /// <summary>
        /// The data contexts.
        /// </summary>
        private readonly IDictionary<string, IDataContext> dataContexts;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataContextContainer"/> class.
        /// </summary>
        /// <param name="dataContextFactory">The data context factory.</param>
        /// <param name="dataStoreSelector">The data store selector.</param>
        /// <param name="context">Optional. Context for the data processing.</param>
        /// <param name="entityInfos">Optional. The entity infos for initial data.</param>
        public DataContextContainer(
            IDataContextFactory dataContextFactory,
            IDataStoreSelector dataStoreSelector,
            IContext context = null,
            IEnumerable<IChangeStateTrackableEntityInfo> entityInfos = null)
        {
            Requires.NotNull(dataContextFactory, nameof(dataContextFactory));
            Requires.NotNull(dataStoreSelector, nameof(dataStoreSelector));

            this.dataContextFactory = dataContextFactory;
            this.dataStoreSelector = dataStoreSelector;
            this.context = context;
            this.dataContexts = entityInfos == null
                                    ? new Dictionary<string, IDataContext>()
                                    : entityInfos
                                        .GroupBy(e => this.dataStoreSelector.GetDataStoreName(e.Entity.GetType(), this.context), e => e)
                                        .ToDictionary(
                                            g => g.Key,
                                            g =>
                                                {
                                                    var initializationContext =
                                                        new Context(context?.CompositionContext)
                                                        {
                                                            Identity = context?.Identity,
                                                        };
                                                    initializationContext.WithInitialData(
                                                        g.Select(
                                                            entry => new EntityInfo(entry.Entity)
                                                            {
                                                                ChangeState = entry.ChangeState
                                                            }));
                                                    return dataContextFactory.CreateDataContext(
                                                        g.Key,
                                                        initializationContext);
                                                });
        }

        /// <summary>Gets the number of elements in the collection.</summary>
        /// <returns>The number of elements in the collection. </returns>
        public int Count => this.dataContexts.Count;

        /// <summary>
        /// Gets the data context for the provided entity type.
        /// </summary>
        /// <param name="entityType">Type of the entity.</param>
        /// <returns>
        /// The data context.
        /// </returns>
        public IDataContext this[Type entityType]
        {
            get
            {
                var dataStoreName = this.dataStoreSelector.GetDataStoreName(entityType, this.context);
                var dataContext = this.dataContexts.TryGetValue(dataStoreName);
                if (dataContext == null)
                {
                    dataContext = this.dataContextFactory.CreateDataContext(dataStoreName, this.context);
                    this.dataContexts.Add(dataStoreName, dataContext);
                }

                return dataContext;
            }
        }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            foreach (var dataContext in this.dataContexts.Values)
            {
                dataContext.Dispose();
            }

            this.dataContexts.Clear();
        }

        /// <summary>Returns an enumerator that iterates through the collection.</summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<IDataContext> GetEnumerator()
        {
            return this.dataContexts.Values.GetEnumerator();
        }

        /// <summary>Returns an enumerator that iterates through a collection.</summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}