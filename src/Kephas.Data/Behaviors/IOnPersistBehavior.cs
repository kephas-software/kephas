// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IOnPersistBehavior.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the IOnPersistBehavior interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Behaviors
{
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Contract for the behavior invoked upon persist operation.
    /// </summary>
    public interface IOnPersistBehavior
    {
        /// <summary>
        /// Callback invoked before an entity is being persisted.
        /// </summary>
        /// <param name="obj">The object to be persisted.</param>
        /// <param name="operationContext">The operation context.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A Task.
        /// </returns>
        Task BeforePersistAsync(object obj, IDataOperationContext operationContext, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Callback invoked after an entity was persisted.
        /// </summary>
        /// <param name="obj">The persisted object.</param>
        /// <param name="operationContext">The operation context.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A Task.
        /// </returns>
        Task AfterPersistAsync(object obj, IDataOperationContext operationContext, CancellationToken cancellationToken = default(CancellationToken));
    }
}