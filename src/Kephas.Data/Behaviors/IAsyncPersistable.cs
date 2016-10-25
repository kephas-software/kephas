// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAsyncPersistable.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the IAsyncPersistable interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Behaviors
{
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Contract for an entity's ability of being persisted asynchronously.
    /// </summary>
    public interface IAsyncPersistable
    {
        /// <summary>
        /// Callback invoked before an entity is being persisted.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">(Optional) the cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        Task BeforePersistAsync(IDataContext context, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Callback invoked after an entity was persisted.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">(Optional) the cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        Task AfterPersistAsync(IDataContext context, CancellationToken cancellationToken = default(CancellationToken));
    }
}