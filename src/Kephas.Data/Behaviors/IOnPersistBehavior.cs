// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IOnPersistBehavior.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IOnPersistBehavior interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Behaviors
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Data.Capabilities;

    /// <summary>
    /// Contract for the behavior invoked upon persist operation.
    /// </summary>
    public interface IOnPersistBehavior
    {
        /// <summary>
        /// Callback invoked before an entity is being persisted.
        /// </summary>
        /// <param name="entity">The entity to be persisted.</param>
        /// <param name="entityInfo">The entity information.</param>
        /// <param name="operationContext">The operation context.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A Task.
        /// </returns>
        Task BeforePersistAsync(object entity, IEntityInfo entityInfo, IDataOperationContext operationContext, CancellationToken cancellationToken = default);

        /// <summary>
        /// Callback invoked after an entity was persisted.
        /// </summary>
        /// <param name="entity">The entity to be persisted.</param>
        /// <param name="entityInfo">The entity information.</param>
        /// <param name="operationContext">The operation context.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A Task.
        /// </returns>
        Task AfterPersistAsync(object entity, IEntityInfo entityInfo, IDataOperationContext operationContext, CancellationToken cancellationToken = default);
    }
}