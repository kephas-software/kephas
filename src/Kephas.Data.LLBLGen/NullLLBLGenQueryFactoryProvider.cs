// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NullLLBLGenQueryFactoryProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the null llbl generate query factory provider class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.LLBLGen
{
    using System;

    using Kephas.Data.LLBLGen.Entities;
    using Kephas.Services;

    using SD.LLBLGen.Pro.ORMSupportClasses;

    using Context = SD.LLBLGen.Pro.ORMSupportClasses.Context;

    /// <summary>
    /// A null llbl generate query factory provider.
    /// </summary>
    [OverridePriority(Priority.Lowest)]
    public class NullLLBLGenQueryFactoryProvider : ILLBLGenQueryFactoryProvider
    {
        /// <summary>
        /// Creates query factory.
        /// </summary>
        /// <param name="adapter">The adapter.</param>
        /// <param name="contextToUse">The context to use.</param>
        /// <returns>
        /// The new query factory.
        /// </returns>
        public IQueryFactory CreateQueryFactory(DataAccessAdapterBase adapter, Context contextToUse)
        {
            throw new NotSupportedException($"Please provide a proper implementation of the {typeof(ILLBLGenQueryFactoryProvider)} service.");
        }
    }
}