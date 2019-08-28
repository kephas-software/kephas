// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QueryBehaviorBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the query behavior base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Behaviors
{
    using System;

    using Kephas.Logging;

    /// <summary>
    /// A query behavior base.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    public abstract class QueryBehaviorBase<T> : Loggable, IDataBehavior<T>, IOnQueryBehavior
    {
        /// <summary>
        /// Callback invoked before the query is being created.
        /// </summary>
        /// <param name="entityType">Type of the entity.</param>
        /// <param name="queryOperationContext">Context for the query operation.</param>
        public virtual void BeforeQuery(Type entityType, IQueryOperationContext queryOperationContext)
        {
        }

        /// <summary>
        /// Callback invoked after the query has been created.
        /// </summary>
        /// <param name="entityType">Type of the entity.</param>
        /// <param name="queryOperationContext">Context for the query operation.</param>
        public virtual void AfterQuery(Type entityType, IQueryOperationContext queryOperationContext)
        {
        }
    }
}