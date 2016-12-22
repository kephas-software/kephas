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
    public interface IDataContextProvider
    {
        /// <summary>
        /// Gets data context for the provided context object.
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns>
        /// The new data context.
        /// </returns>
        IDataContext GetDataContext(IDataContextConfiguration configuration = null);
    }

    /// <summary>
    /// Generic factory service for data contexts.
    /// </summary>
    /// <typeparam name="TDataContext">Type of the data context.</typeparam>
    [SharedAppServiceContract(AsOpenGeneric = true)]
    public interface IDataContextProvider<TDataContext> : IDataContextProvider
        where TDataContext : IDataContext
    {
    }
}