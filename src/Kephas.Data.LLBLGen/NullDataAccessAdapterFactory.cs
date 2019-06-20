// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NullDataAccessAdapterFactory.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the null llbl generate data access adapter factory class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.LLBLGen
{
    using System;

    using Kephas.Services;

    using SD.LLBLGen.Pro.ORMSupportClasses;

    /// <summary>
    /// A null LLBLGen data access adapter factory.
    /// </summary>
    [OverridePriority(Priority.Lowest)]
    public class NullDataAccessAdapterFactory : IDataAccessAdapterFactory
    {
        /// <summary>
        /// Creates data access adapter.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>
        /// The new data access adapter.
        /// </returns>
        public DataAccessAdapterBase CreateDataAccessAdapter(IContext context)
        {
            throw new NotSupportedException($"Please provide a proper implementation of the {typeof(IDataAccessAdapterFactory)} service.");
        }
    }
}