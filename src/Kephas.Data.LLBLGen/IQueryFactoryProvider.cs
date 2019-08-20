// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IQueryFactoryProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the illbl generate query factory provider class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.LLBLGen
{
    using Kephas.Data.LLBLGen.Entities;
    using Kephas.Services;

    using SD.LLBLGen.Pro.ORMSupportClasses;

    using Context = SD.LLBLGen.Pro.ORMSupportClasses.Context;

    /// <summary>
    /// Singleton application service contract for the query factory provider.
    /// </summary>
    [SingletonAppServiceContract]
    public interface IQueryFactoryProvider
    {
        /// <summary>
        /// Creates query factory based on the data access adapter and the context to use.
        /// </summary>
        /// <param name="adapter">The adapter.</param>
        /// <param name="contextToUse">The context to use.</param>
        /// <returns>
        /// The new query factory.
        /// </returns>
        IQueryFactory CreateQueryFactory(DataAccessAdapterBase adapter, Context contextToUse);
    }
}