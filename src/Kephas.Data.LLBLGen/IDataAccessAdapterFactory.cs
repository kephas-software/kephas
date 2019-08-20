// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataAccessAdapterFactory.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the illbl generate data access adapter provider class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.LLBLGen
{
    using Kephas.Services;

    using SD.LLBLGen.Pro.ORMSupportClasses;

    /// <summary>
    /// Singleton application service contract for the data access adapter factory.
    /// </summary>
    [SingletonAppServiceContract]
    public interface IDataAccessAdapterFactory
    {
        /// <summary>
        /// Creates the data access adapter.
        /// </summary>
        /// <param name="context">Optional. The creation context.</param>
        /// <returns>
        /// The new data access adapter.
        /// </returns>
        DataAccessAdapterBase CreateDataAccessAdapter(IContext context = null);
    }
}