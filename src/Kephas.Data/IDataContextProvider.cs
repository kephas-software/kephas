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
        /// <param name="context">(Optional) the context.</param>
        /// <returns>
        /// The new data context.
        /// </returns>
        IDataContext GetDataContext(IContext context = null);
    }
}