// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IOnQueryBehavior.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IOnQueryBehavior interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Behaviors
{
    using System;

    /// <summary>
    /// Contract for the behavior invoked upon building entity queries.
    /// </summary>
    public interface IOnQueryBehavior
    {
        /// <summary>
        /// Callback invoked before the query is being created.
        /// </summary>
        /// <param name="entityType">Type of the entity.</param>
        /// <param name="queryOperationContext">Context for the query operation.</param>
        void BeforeQuery(Type entityType, IQueryOperationContext queryOperationContext);

        /// <summary>
        /// Callback invoked after the query has been created.
        /// </summary>
        /// <param name="entityType">Type of the entity.</param>
        /// <param name="queryOperationContext">Context for the query operation.</param>
        void AfterQuery(Type entityType, IQueryOperationContext queryOperationContext);
    }
}