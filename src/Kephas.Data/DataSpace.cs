// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataSpace.cs" company="Kephas Software SRL">
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
    using Kephas.Data.Store;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Injection;
    using Kephas.Reflection;
    using Kephas.Services;

    /// <summary>
    /// Container class for data contexts indexed by contained entity types.
    /// </summary>
    public class DataSpace : Context, IDataSpace
    {
        private readonly IDataContextFactory dataContextFactory;
        private readonly IDataStoreProvider dataStoreProvider;
        private IContext? initializationContext;
        private IDictionary<string, IDataContext> dataContextMap = new Dictionary<string, IDataContext>();

        /// <summary>
        /// Initializes a new instance of the <see cref="DataSpace"/> class.
        /// </summary>
        /// <param name="injector">The injector.</param>
        /// <param name="dataContextFactory">The data context factory.</param>
        /// <param name="dataStoreProvider">The data store provider.</param>
        public DataSpace(
            IInjector injector,
            IDataContextFactory dataContextFactory,
            IDataStoreProvider dataStoreProvider)
            : this(injector, dataContextFactory, dataStoreProvider, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataSpace"/> class.
        /// </summary>
        /// <param name="injector">The injector.</param>
        /// <param name="dataContextFactory">The data context factory.</param>
        /// <param name="dataStoreProvider">The data store provider.</param>
        /// <param name="isThreadSafe">True if this object is thread safe.</param>
        protected DataSpace(
            IInjector injector,
            IDataContextFactory dataContextFactory,
            IDataStoreProvider dataStoreProvider,
            bool isThreadSafe)
            : base(injector, isThreadSafe)
        {
            Requires.NotNull(dataContextFactory, nameof(dataContextFactory));
            Requires.NotNull(dataStoreProvider, nameof(dataStoreProvider));

            this.dataContextFactory = dataContextFactory;
            this.dataStoreProvider = dataStoreProvider;
        }

        /// <summary>Gets the number of elements in the collection.</summary>
        /// <returns>The number of elements in the collection. </returns>
        public int Count => this.dataContextMap.Count;

        /// <summary>
        /// Gets the data context for the provided entity type.
        /// </summary>
        /// <param name="entityType">Type of the entity.</param>
        /// <returns>
        /// The data context.
        /// </returns>
        IDataContext IDataSpace.this[Type entityType] => this.GetDataContext(entityType);

        /// <summary>
        /// Gets the data context for the provided entity type.
        /// </summary>
        /// <param name="entityType">Type of the entity.</param>
        /// <returns>
        /// The data context.
        /// </returns>
        IDataContext IDataSpace.this[ITypeInfo entityType] => this.GetDataContext(entityType);

        /// <summary>Returns an enumerator that iterates through the collection.</summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<IDataContext> GetEnumerator()
        {
            return this.dataContextMap.Values.GetEnumerator();
        }

        /// <summary>Returns an enumerator that iterates through a collection.</summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// Initializes the service.
        /// </summary>
        /// <param name="context">An optional context for initialization.</param>
        public virtual void Initialize(IContext? context = null)
        {
            this.initializationContext = context;
            if (this.Identity == null)
            {
                this.Identity = context?.Identity;
            }

            this.ClearDataContextMap();
            var entityEntries = context?.InitialData();
            if (entityEntries != null)
            {
                this.dataContextMap = entityEntries
                                          .GroupBy(e => this.dataStoreProvider.GetDataStoreName(e.Entity.GetType(), this.initializationContext), e => e)
                                          .ToDictionary(
                                              g => g.Key,
                                              g =>
                                                  {
                                                      var initContext = new Context(this.Injector)
                                                                                      {
                                                                                          Identity = this.Identity,
                                                                                      }.InitialData(g);
                                                      return this.dataContextFactory.CreateDataContext(
                                                          g.Key,
                                                          initContext);
                                                  });
            }
        }

        /// <summary>
        /// Releases the unmanaged resources used by the Kephas.Services.Context and optionally releases
        /// the managed resources.
        /// </summary>
        /// <param name="disposing">True to release both managed and unmanaged resources; false to
        ///                         release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            this.ClearDataContextMap();
            base.Dispose(disposing);
        }

        /// <summary>
        /// Gets the data context for the provided entity type.
        /// </summary>
        /// <param name="entityType">Type of the entity.</param>
        /// <returns>
        /// The data context.
        /// </returns>
        protected virtual IDataContext GetDataContext(ITypeInfo entityType) =>
            this.GetDataContext(entityType.AsType());

        /// <summary>
        /// Gets the data context for the provided entity type.
        /// </summary>
        /// <param name="entityType">Type of the entity.</param>
        /// <returns>
        /// The data context.
        /// </returns>
        protected virtual IDataContext GetDataContext(Type entityType)
        {
            var dataStoreName = this.dataStoreProvider.GetDataStoreName(entityType, this.initializationContext);
            var dataContext = this.dataContextMap.TryGetValue(dataStoreName);
            if (dataContext == null)
            {
                dataContext = this.dataContextFactory.CreateDataContext(dataStoreName, this.initializationContext);
                this.dataContextMap.Add(dataStoreName, dataContext);
            }

            return dataContext;
        }

        /// <summary>
        /// Clears the data context map.
        /// </summary>
        protected virtual void ClearDataContextMap()
        {
            foreach (var dataContext in this.dataContextMap.Values)
            {
                dataContext.Dispose();
            }

            this.dataContextMap.Clear();
        }
    }
}