// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataContextFactory.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the IDataContextFactory interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data
{
    using Kephas.Services;

    /// <summary>
    /// Factory service for data contexts.
    /// </summary>
    [SharedAppServiceContract]
    public interface IDataContextFactory
    {
        /// <summary>
        /// Creates a data context for the provided data store name.
        /// </summary>
        /// <param name="dataStoreName">Name of the data store.</param>
        /// <param name="initializationContext">An initialization context (optional).</param>
        /// <returns>
        /// The newly created data context.
        /// </returns>
        IDataContext CreateDataContext(string dataStoreName, IContext initializationContext = null);
    }
}