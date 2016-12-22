// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultDataContextProvider.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the data context provider base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data
{
    using System.Diagnostics.Contracts;

    using Kephas.Composition;

    /// <summary>
    /// Base implementation of a <see cref="IDataContextProvider"/>.
    /// </summary>
    /// <typeparam name="TDataContext">Type of the data context.</typeparam>
    public class DefaultDataContextProvider<TDataContext> : IDataContextProvider<TDataContext>
        where TDataContext : IDataContext
    {
        /// <summary>
        /// The data context factory.
        /// </summary>
        private readonly IExportFactory<TDataContext> dataContextFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultDataContextProvider{TDataContext}"/> class.
        /// </summary>
        /// <param name="dataContextFactory">The client data context factory.</param>
        protected DefaultDataContextProvider(IExportFactory<TDataContext> dataContextFactory)
        {
            Contract.Requires(dataContextFactory != null);

            this.dataContextFactory = dataContextFactory;
        }

        /// <summary>
        /// Gets data context for the provided context object.
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns>
        /// The new data context.
        /// </returns>
        public virtual IDataContext GetDataContext(IDataContextConfiguration configuration = null)
        {
            var dataContext = this.dataContextFactory.CreateExport().Value;
            dataContext.Initialize(configuration);
            return dataContext;
        }
    }
}