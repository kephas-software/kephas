// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAsyncInitializable.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the IAsyncInitializable interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Behaviors
{
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Contract for an entity's ability of being initialized asynchronously.
    /// </summary>
    public interface IAsyncInitializable
    {
        /// <summary>
        /// Initializes the entity asynchronously.
        /// </summary>
        /// <param name="operationContext">The operation context.</param>
        /// <param name="cancellationToken">(Optional) the cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        Task InitializeAsync(IDataOperationContext operationContext, CancellationToken cancellationToken = default(CancellationToken));
    }
}