// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataContextProvider.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the IDataContextProvider interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data
{
    using Kephas.Services;

    /// <summary>
    /// Factory service for data contexts.
    /// </summary>
    [AppServiceContract]
    public interface IDataContextProvider
    {
        /// <summary>
        /// Gets a data context for the provided data store name.
        /// </summary>
        /// <param name="dataStoreName">Name of the data store.</param>
        /// <param name="initializationContext">An initialization context (optional).</param>
        /// <returns>
        /// The new data context.
        /// </returns>
        IDataContext GetDataContext(string dataStoreName, IContext initializationContext = null);
    }
}