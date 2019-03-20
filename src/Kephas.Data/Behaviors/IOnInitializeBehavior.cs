// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IOnInitializeBehavior.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IOnInitializeBehavior interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Behaviors
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Data.Capabilities;

    /// <summary>
    /// Contract for the behavior invoked upon entity intialization.
    /// </summary>
    public interface IOnInitializeBehavior
    {
        /// <summary>
        /// Initializes the entity asynchronously.
        /// </summary>
        /// <param name="entity">The entitiy to be initialized.</param>
        /// <param name="entityEntry">The entity entry.</param>
        /// <param name="operationContext">The operation context.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A Task.
        /// </returns>
        Task InitializeAsync(object entity, IEntityEntry entityEntry, IDataOperationContext operationContext, CancellationToken cancellationToken = default);
    }
}