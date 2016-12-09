// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultClientDataContextProvider.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the default client data context provider class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.ClientEntities
{
    using System.Diagnostics.Contracts;

    using Kephas.Composition;
    using Kephas.Services;

    /// <summary>
    /// A default client data context provider.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class DefaultClientDataContextProvider : IClientDataContextProvider
    {
        /// <summary>
        /// The client data context factory.
        /// </summary>
        private readonly IExportFactory<ClientDataContext> clientDataContextFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultClientDataContextProvider"/> class.
        /// </summary>
        /// <param name="clientDataContextFactory">The client data context factory.</param>
        public DefaultClientDataContextProvider(IExportFactory<ClientDataContext> clientDataContextFactory)
        {
            Contract.Requires(clientDataContextFactory != null);

            this.clientDataContextFactory = clientDataContextFactory;
        }

        /// <summary>
        /// Gets data context for the provided context object.
        /// </summary>
        /// <param name="context">(Optional) the context.</param>
        /// <returns>
        /// The new data context.
        /// </returns>
        public IDataContext GetDataContext(IContext context = null)
        {
            return this.clientDataContextFactory.CreateExport().Value;
        }
    }
}